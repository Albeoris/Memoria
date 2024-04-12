using Antlr.Runtime;
using NCalc.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

// Original author: sebastienros https://archive.codeplex.com/?p=ncalc
// Author of this version: pitermarx https://github.com/pitermarx/NCalc-Edge

namespace NCalc
{
	public class Expression
	{
		public EvaluateOptions Options { get; set; }

		private bool IgnoreCase { get { return (Options & EvaluateOptions.IgnoreCase) == EvaluateOptions.IgnoreCase; } }

		/// <summary>
		/// Textual representation of the expression to evaluate.
		/// </summary>
		protected string OriginalExpression;

		public Expression(string expression, EvaluateOptions options = EvaluateOptions.None)
		{
			if (String.IsNullOrEmpty(expression))
				throw new
					ArgumentException("Expression can't be empty", "expression");

			OriginalExpression = expression;
			Options = options;
		}

		public Expression(LogicalExpression expression, EvaluateOptions options = EvaluateOptions.None)
		{
			if (expression == null)
				throw new
					ArgumentException("Expression can't be null", "expression");

			ParsedExpression = expression;
			Options = options;
		}

		#region Cache management

		private static bool cacheEnabled = true;
		private static Dictionary<string, WeakReference> compiledExpressions = new Dictionary<string, WeakReference>();
		private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();

		public static bool CacheEnabled
		{
			get { return cacheEnabled; }
			set
			{
				cacheEnabled = value;

				if (!CacheEnabled)
				{
					// Clears cache
					compiledExpressions = new Dictionary<string, WeakReference>();
				}
			}
		}

		/// <summary>
		/// Removed unused entries from cached compiled expression
		/// </summary>
		private static void CleanCache()
		{
			var keysToRemove = new List<string>();

			try
			{
				Rwl.AcquireWriterLock(Timeout.Infinite);
				keysToRemove.AddRange(compiledExpressions.Where(de => !de.Value.IsAlive).Select(de => de.Key));

				foreach (string key in keysToRemove)
				{
					compiledExpressions.Remove(key);
					Trace.TraceInformation("Cache entry released: " + key);
				}
			}
			finally
			{
				Rwl.ReleaseReaderLock();
			}
		}

		#endregion Cache management

		public static LogicalExpression Compile(string expression, bool nocache)
		{
			LogicalExpression logicalExpression = null;

			if (cacheEnabled && !nocache)
			{
				try
				{
					Rwl.AcquireReaderLock(Timeout.Infinite);

					if (compiledExpressions.ContainsKey(expression))
					{
						Trace.TraceInformation("Expression retrieved from cache: " + expression);
						var wr = compiledExpressions[expression];
						logicalExpression = wr.Target as LogicalExpression;

						if (wr.IsAlive && logicalExpression != null)
						{
							return logicalExpression;
						}
					}
				}
				finally
				{
					Rwl.ReleaseReaderLock();
				}
			}

			if (logicalExpression == null)
			{
				var lexer = new NCalcLexer(new ANTLRStringStream(expression));
				var parser = new NCalcParser(new CommonTokenStream(lexer));

				logicalExpression = parser.ncalcExpression().value;

				if (parser.Errors != null && parser.Errors.Count > 0)
				{
					throw new EvaluationException(String.Join(Environment.NewLine, parser.Errors.ToArray()));
				}

				if (cacheEnabled && !nocache)
				{
					try
					{
						Rwl.AcquireWriterLock(Timeout.Infinite);
						compiledExpressions[expression] = new WeakReference(logicalExpression);
					}
					finally
					{
						Rwl.ReleaseWriterLock();
					}

					CleanCache();

					Trace.TraceInformation("Expression added to cache: " + expression);
				}
			}

			return logicalExpression;
		}

		/// <summary>
		/// Pre-compiles the expression in order to check syntax errors.
		/// If errors are detected, the Error property contains the message.
		/// </summary>
		/// <returns>True if the expression syntax is correct, otherwiser False</returns>
		public bool HasErrors()
		{
			try
			{
				if (ParsedExpression == null)
				{
					ParsedExpression = Compile(OriginalExpression, Options.NoCache());
				}

				// In case HasErrors() is called multiple times for the same expression
				return ParsedExpression != null && Error != null;
			}
			catch (Exception e)
			{
				Error = e.Message;
				return true;
			}
		}

		public string Error { get; private set; }

		public LogicalExpression ParsedExpression { get; private set; }

		protected Dictionary<string, IEnumerator> ParameterEnumerators;
		protected Dictionary<string, object> ParametersBackup;

		public object Evaluate()
		{
			if (HasErrors())
			{
				throw new EvaluationException(Error);
			}

			if (ParsedExpression == null)
			{
				ParsedExpression = Compile(OriginalExpression, Options.NoCache());
			}

			var visitor = new EvaluationVisitor(Options);
			visitor.EvaluateFunction += EvaluateFunction;
			visitor.EvaluateParameter += EvaluateParameter;
			visitor.Parameters = Parameters;

			// Add a "null" parameter which returns null if configured to do so
			// Configured as an option to ensure no breaking changes for historical use
			if (Options.AllowNullParameter() && !visitor.Parameters.ContainsKey("null"))
			{
				visitor.Parameters["null"] = null;
			}

			// if array evaluation, execute the same expression multiple times
			if (Options.IterateParameters())
			{
				int size = -1;
				ParametersBackup = new Dictionary<string, object>();
				foreach (string key in Parameters.Keys)
				{
					ParametersBackup.Add(key, Parameters[key]);
				}

				ParameterEnumerators = new Dictionary<string, IEnumerator>();

				foreach (object parameter in Parameters.Values)
				{
					var enumerable = parameter as IEnumerable;
					if (enumerable != null)
					{
						int localsize = enumerable.Cast<object>().Count();

						if (size == -1)
						{
							size = localsize;
						}
						else if (localsize != size)
						{
							throw new EvaluationException("When IterateParameters option is used, IEnumerable parameters must have the same number of items");
						}
					}
				}

				foreach (string key in Parameters.Keys)
				{
					var parameter = Parameters[key] as IEnumerable;
					if (parameter != null)
					{
						ParameterEnumerators.Add(key, parameter.GetEnumerator());
					}
				}

				var results = new List<object>();
				for (int i = 0; i < size; i++)
				{
					foreach (string key in ParameterEnumerators.Keys)
					{
						IEnumerator enumerator = ParameterEnumerators[key];
						enumerator.MoveNext();
						Parameters[key] = enumerator.Current;
					}

					ParsedExpression.Accept(visitor);
					results.Add(visitor.Result);
				}

				return results;
			}

			ParsedExpression.Accept(visitor);
			return visitor.Result;
		}

		public event EvaluateFunctionHandler EvaluateFunction;

		public event EvaluateParameterHandler EvaluateParameter;

		private Dictionary<string, object> parameters;

		public Dictionary<string, object> Parameters
		{
			get { return parameters ?? (parameters = new Dictionary<string, object>(IgnoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture)); }
			set { parameters = value; }
		}
	}
}
