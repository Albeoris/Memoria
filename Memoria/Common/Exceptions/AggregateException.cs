using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace Memoria
{
    /// <summary>Represents one or more errors that occur during application execution.</summary> 
    /// <remarks>
    /// <see cref="AggregateException"/> is used to consolidate multiple failures into a single, throwable
    /// exception object.
    /// </remarks> 
    [Serializable]
    [DebuggerDisplay("Count = {InnerExceptions.Count}")]
    public class AggregateException : Exception
    {
        private readonly ReadOnlyCollection<Exception> _innerExceptions; // Complete set of exceptions.

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class. 
        /// </summary>
        public AggregateException()
            : base("One or more errors occurred.")
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class with 
        /// a specified error message.
        /// </summary> 
        /// <param name="message">The error message that explains the reason for the exception.</param> 
        public AggregateException(string message)
            : base(message)
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class with a specified error 
        /// message and a reference to the inner exception that is the cause of this exception. 
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param> 
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerException"/> argument
        /// is null.</exception>
        public AggregateException(string message, Exception innerException)
            : base(message, innerException)
        {
            if (innerException == null)
            {
                throw new ArgumentNullException(nameof(innerException));
            }

            _innerExceptions = new ReadOnlyCollection<Exception>(new[] {innerException});
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="AggregateException"/> class with 
        /// references to the inner exceptions that are the cause of this exception.
        /// </summary> 
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument
        /// is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is 
        /// null.</exception>
        public AggregateException(IEnumerable<Exception> innerExceptions) :
            this("One or more errors occurred.", innerExceptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class with
        /// references to the inner exceptions that are the cause of this exception. 
        /// </summary>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param> 
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument 
        /// is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is 
        /// null.</exception>
        public AggregateException(params Exception[] innerExceptions) :
            this("One or more errors occurred.", innerExceptions)
        {
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="AggregateException"/> class with a specified error
        /// message and references to the inner exceptions that are the cause of this exception. 
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument 
        /// is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is 
        /// null.</exception> 
        public AggregateException(string message, IEnumerable<Exception> innerExceptions)
            : this(message, innerExceptions == null ? null : new List<Exception>(innerExceptions))
        {
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="AggregateException"/> class with a specified error
        /// message and references to the inner exceptions that are the cause of this exception. 
        /// </summary> 
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param> 
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument
        /// is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is
        /// null.</exception> 
        public AggregateException(string message, params Exception[] innerExceptions) :
            this(message, (IList<Exception>)innerExceptions)
        {
        }

        /// <summary>
        /// Allocates a new aggregate exception with the specified message and list of inner exceptions.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param> 
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument 
        /// is null.</exception> 
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is
        /// null.</exception> 
        private AggregateException(string message, IList<Exception> innerExceptions)
            : base(message, innerExceptions != null && innerExceptions.Count > 0 ? innerExceptions[0] : null)
        {
            if (innerExceptions == null)
            {
                throw new ArgumentNullException(nameof(innerExceptions));
            }

            // Copy exceptions to our internal array and validate them. We must copy them, 
            // because we're going to put them into a ReadOnlyCollection which simply reuses
            // the list passed in to it. We don't want callers subsequently mutating.
            Exception[] exceptionsCopy = new Exception[innerExceptions.Count];

            for (int i = 0; i < exceptionsCopy.Length; i++)
            {
                exceptionsCopy[i] = innerExceptions[i];

                if (exceptionsCopy[i] == null)
                {
                    throw new ArgumentException("Inner exception cannot be null.");
                }
            }

            _innerExceptions = new ReadOnlyCollection<Exception>(exceptionsCopy);
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="AggregateException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds
        /// the serialized object data about the exception being thrown.</param> 
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that
        /// contains contextual information about the source or destination. </param> 
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception> 
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The exception could not be deserialized correctly.</exception>
        [SecurityCritical]
        protected AggregateException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Exception[] innerExceptions = info.GetValue("InnerExceptions", typeof(Exception[])) as Exception[];
            if (innerExceptions == null)
            {
                throw new SerializationException("Deserialization failure.");
            }

            _innerExceptions = new ReadOnlyCollection<Exception>(innerExceptions);
        }

        /// <summary> 
        /// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about
        /// the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds 
        /// the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that 
        /// contains contextual information about the source or destination. </param> 
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);

            Exception[] innerExceptions = new Exception[_innerExceptions.Count];
            _innerExceptions.CopyTo(innerExceptions, 0);
            info.AddValue("InnerExceptions", innerExceptions, typeof(Exception[]));
        }

        /// <summary> 
        /// Returns the <see cref="AggregateException"/> that is the root cause of this exception. 
        /// </summary>
        public override Exception GetBaseException()
        {
            // Returns the first inner AggregateException that contains more or less than one inner exception

            // Recursively traverse the inner exceptions as long as the inner exception of type AggregateException and has only one inner exception 
            Exception back = this;
            AggregateException backAsAggregate = this;
            while (backAsAggregate != null && backAsAggregate.InnerExceptions.Count == 1)
            {
                back = back.InnerException;
                backAsAggregate = back as AggregateException;
            }
            return back;
        }

        /// <summary> 
        /// Gets a read-only collection of the <see cref="T:System.Exception"/> instances that caused the 
        /// current exception.
        /// </summary> 
        public ReadOnlyCollection<Exception> InnerExceptions => _innerExceptions;

#if !FEATURE_CORECLR
        /// <summary> 
        /// Invokes a handler on each <see cref="T:System.Exception"/> contained by this <see
        /// cref="AggregateException"/>. 
        /// </summary>
        /// <param name="predicate">The predicate to execute for each exception. The predicate accepts as an
        /// argument the <see cref="T:System.Exception"/> to be processed and returns a Boolean to indicate
        /// whether the exception was handled.</param> 
        /// <remarks>
        /// Each invocation of the <paramref name="predicate"/> returns true or false to indicate whether the 
        /// <see cref="T:System.Exception"/> was handled. After all invocations, if any exceptions went 
        /// unhandled, all unhandled exceptions will be put into a new <see cref="AggregateException"/>
        /// which will be thrown. Otherwise, the <see cref="Handle"/> method simply returns. If any 
        /// invocations of the <paramref name="predicate"/> throws an exception, it will halt the processing
        /// of any more exceptions and immediately propagate the thrown exception as-is.
        /// </remarks>
        /// <exception cref="AggregateException">An exception contained by this <see 
        /// cref="AggregateException"/> was not handled.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="predicate"/> argument is 
        /// null.</exception> 
        public void Handle(Func<Exception, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            List<Exception> unhandledExceptions = null;
            for (int i = 0; i < _innerExceptions.Count; i++)
            {
                // If the exception was not handled, lazily allocate a list of unhandled 
                // exceptions (to be rethrown later) and add it.
                if (!predicate(_innerExceptions[i]))
                {
                    if (unhandledExceptions == null)
                    {
                        unhandledExceptions = new List<Exception>();
                    }

                    unhandledExceptions.Add(_innerExceptions[i]);
                }
            }

            // If there are unhandled exceptions remaining, throw them. 
            if (unhandledExceptions != null)
            {
                throw new AggregateException(Message, unhandledExceptions);
            }
        }


#endif //!FEATURE_CORECLR



        /// <summary> 
        /// Flattens an <see cref="AggregateException"/> instances into a single, new instance.
        /// </summary> 
        /// <returns>A new, flattened <see cref="AggregateException"/>.</returns>
        /// <remarks>
        /// If any inner exceptions are themselves instances of
        /// <see cref="AggregateException"/>, this method will recursively flatten all of them. The 
        /// inner exceptions returned in the new <see cref="AggregateException"/>
        /// will be the union of all of the the inner exceptions from exception tree rooted at the provided 
        /// <see cref="AggregateException"/> instance. 
        /// </remarks>
        public AggregateException Flatten()
        {
            // Initialize a collection to contain the flattened exceptions.
            List<Exception> flattenedExceptions = new List<Exception>();

            // Create a list to remember all aggregates to be flattened, this will be accessed like a FIFO queue
            List<AggregateException> exceptionsToFlatten = new List<AggregateException> {this};
            int nDequeueIndex = 0;

            // Continue removing and recursively flattening exceptions, until there are no more.
            while (exceptionsToFlatten.Count > nDequeueIndex)
            {
                // dequeue one from exceptionsToFlatten 
                IList<Exception> currentInnerExceptions = exceptionsToFlatten[nDequeueIndex++].InnerExceptions;

                for (int i = 0; i < currentInnerExceptions.Count; i++)
                {
                    Exception currentInnerException = currentInnerExceptions[i];

                    if (currentInnerException == null)
                    {
                        continue;
                    }

                    AggregateException currentInnerAsAggregate = currentInnerException as AggregateException;

                    // If this exception is an aggregate, keep it around for later.  Otherwise, 
                    // simply add it to the list of flattened exceptions to be returned.
                    if (currentInnerAsAggregate != null)
                    {
                        exceptionsToFlatten.Add(currentInnerAsAggregate);
                    }
                    else
                    {
                        flattenedExceptions.Add(currentInnerException);
                    }
                }
            }


            return new AggregateException(Message, flattenedExceptions);
        }

        /// <summary>
        /// Creates and returns a string representation of the current <see cref="AggregateException"/>. 
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            string text = base.ToString();

            for (int i = 0; i < _innerExceptions.Count; i++)
            {
                text = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}---> (Inner Exception #{2}) {3}{4}{5}",
                    text, Environment.NewLine, i, _innerExceptions[i], "<---", Environment.NewLine);
            }

            return text;
        }
    }
}