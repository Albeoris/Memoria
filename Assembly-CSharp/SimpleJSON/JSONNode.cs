using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Memoria.Prime.Text;

namespace SimpleJSON
{
	public class JSONNode
	{
		public virtual void Add(String aKey, JSONNode aItem)
		{
		}

		public virtual JSONNode this[Int32 aIndex]
		{
			get
			{
				return (JSONNode)null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[String aKey]
		{
			get
			{
				return (JSONNode)null;
			}
			set
			{
			}
		}

		public virtual String Value
		{
			get
			{
				return String.Empty;
			}
			set
			{
			}
		}

		public virtual Int32 Count
		{
			get
			{
				return 0;
			}
		}

		public virtual void Add(JSONNode aItem)
		{
			this.Add(String.Empty, aItem);
		}

		public virtual JSONNode Remove(String aKey)
		{
			return (JSONNode)null;
		}

		public virtual JSONNode Remove(Int32 aIndex)
		{
			return (JSONNode)null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public virtual IEnumerable<JSONNode> Childs
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChilds
		{
			get
			{
				foreach (JSONNode C in this.Childs)
				{
					foreach (JSONNode D in C.DeepChilds)
					{
						yield return D;
					}
				}
				yield break;
			}
		}

		public override String ToString()
		{
			return "JSONNode";
		}

		public virtual String ToString(String aPrefix)
		{
			return "JSONNode";
		}

		public virtual Int32 AsInt
		{
			get
			{
				Int32 result = 0;
				if (Int32.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual UInt32 AsUInt
		{
			get
			{
				UInt32 result = 0u;
				if (UInt32.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0u;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual Int64 AsLong
		{
			get
			{
				Int64 result = 0L;
				if (Int64.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0L;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual UInt64 AsULong
		{
			get
			{
				UInt64 result = 0UL;
				if (UInt64.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0UL;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual Single AsFloat
		{
			get
			{
				Single result = 0f;
				if (Single.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0f;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual Double AsDouble
		{
			get
			{
				Double result = 0.0;
				if (Double.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual Boolean AsBool
		{
			get
			{
				Boolean result = false;
				if (Boolean.TryParse(this.Value, out result))
				{
					return result;
				}
				return !String.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = ((!value) ? "false" : "true");
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}

		public override Boolean Equals(Object obj)
		{
			return Object.ReferenceEquals(this, obj);
		}

		public override Int32 GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static String Escape(String aText)
		{
			String text = String.Empty;
			foreach (Char c in aText)
			{
				Char c2 = c;
				switch (c2)
				{
				case '\b':
					text += "\\b";
					break;
				case '\t':
					text += "\\t";
					break;
				case '\n':
					text += "\\n";
					break;
				case '\v':
					IL_3B:
					if (c2 != '"')
					{
						if (c2 != '\\')
						{
							text += c;
						}
						else
						{
							text += "\\\\";
						}
					}
					else
					{
						text += "\\\"";
					}
					break;
				case '\f':
					text += "\\f";
					break;
				case '\r':
					text += "\\r";
					break;
				default:
					goto IL_3B;
				}
			}
			return text;
		}

		public static JSONNode Parse(String json)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode ctx = null;
			Int32 i = 0;
			StringBuilder nameBuilder = new StringBuilder();
			String name = String.Empty;
			Boolean quoteMode = false;
			while (i < json.Length)
			{
				switch (json[i])
				{
					case '{':
						if (quoteMode)
						{
							nameBuilder.Append(json[i]);
							break;
						}

						stack.Push(new JSONClass());
						if (ctx != null)
						{
							name = name.Trim();
							if (ctx is JSONArray)
								ctx.Add(stack.Peek());
							else if (name != String.Empty)
								ctx.Add(name, stack.Peek());
						}

						name = String.Empty;
						nameBuilder.Clear();
						ctx = stack.Peek();
						break;

					case '[':
						if (quoteMode)
						{
							nameBuilder.Append(json[i]);
							break;
						}

						stack.Push(new JSONArray());
						if (ctx != null)
						{
							name = name.Trim();
							if (ctx is JSONArray)
								ctx.Add(stack.Peek());
							else if (name != String.Empty)
								ctx.Add(name, stack.Peek());
						}

						name = String.Empty;
						nameBuilder.Clear();
						ctx = stack.Peek();
						break;

					case '}':
					case ']':
						if (quoteMode)
						{
							nameBuilder.Append(json[i]);
							break;
						}

						if (stack.Count == 0)
							throw new Exception("JSON Parse: Too many closing brackets");

						stack.Pop();
						if (nameBuilder.Length > 0)
						{
							name = name.Trim();
							if (ctx is JSONArray)
								ctx.Add(nameBuilder.ToString());
							else if (name != String.Empty)
								ctx.Add(name, nameBuilder.ToString());
						}

						name = String.Empty;
						nameBuilder.Clear();
						if (stack.Count > 0)
							ctx = stack.Peek();
						break;

					case ':':
						if (quoteMode)
						{
							nameBuilder.Append(json[i]);
							break;
						}

						name = nameBuilder.ToString();
						nameBuilder.Clear();
						break;

					case '"':
						quoteMode ^= true;
						break;

					case ',':
						if (quoteMode)
						{
							nameBuilder.Append(json[i]);
							break;
						}

						if (nameBuilder.Length > 0)
						{
							if (ctx is JSONArray)
								ctx.Add(nameBuilder.ToString());
							else if (name != String.Empty)
								ctx.Add(name, nameBuilder.ToString());
						}

						name = String.Empty;
						nameBuilder.Clear();
						break;

					case '\r':
					case '\n':
						break;

					case ' ':
					case '\t':
						if (quoteMode)
							nameBuilder.Append(json[i]);
						break;

					case '\\':
						++i;
						if (quoteMode)
						{
							Char ch = json[i];
							switch (ch)
							{
								case 't':
									nameBuilder.Append('\t');
									break;
								case 'r':
									nameBuilder.Append('\r');
									break;
								case 'n':
									nameBuilder.Append('\n');
									break;
								case 'b':
									nameBuilder.Append('\b');
									break;
								case 'f':
									nameBuilder.Append('\f');
									break;
								case 'u':
								{
									String s = json.Substring(i + 1, 4);
									nameBuilder.Append((Char) Int32.Parse(s, NumberStyles.AllowHexSpecifier));
									i += 4;
									break;
								}
								default:
									nameBuilder.Append(ch);
									break;
							}
						}

						break;

					default:
						nameBuilder.Append(json[i]);
						break;
				}

				++i;
			}

			if (quoteMode)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}

			return ctx;
		}

		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(String aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public String SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToFile(String aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		public String SaveToBase64()
		{
			String result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONBinaryTag jsonbinaryTag = (JSONBinaryTag)aReader.ReadByte();
			switch (jsonbinaryTag)
			{
			case JSONBinaryTag.Array:
			{
				Int32 num = aReader.ReadInt32();
				JSONArray jsonarray = new JSONArray();
				for (Int32 i = 0; i < num; i++)
				{
					jsonarray.Add(JSONNode.Deserialize(aReader));
				}
				return jsonarray;
			}
			case JSONBinaryTag.Class:
			{
				Int32 num2 = aReader.ReadInt32();
				JSONClass jsonclass = new JSONClass();
				for (Int32 j = 0; j < num2; j++)
				{
					String aKey = aReader.ReadString();
					JSONNode aItem = JSONNode.Deserialize(aReader);
					jsonclass.Add(aKey, aItem);
				}
				return jsonclass;
			}
			case JSONBinaryTag.Value:
				return new JSONData(aReader.ReadString());
			case JSONBinaryTag.IntValue:
				return new JSONData(aReader.ReadInt32());
			case JSONBinaryTag.DoubleValue:
				return new JSONData(aReader.ReadDouble());
			case JSONBinaryTag.BoolValue:
				return new JSONData(aReader.ReadBoolean());
			case JSONBinaryTag.FloatValue:
				return new JSONData(aReader.ReadSingle());
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jsonbinaryTag);
			}
		}

		public static JSONNode LoadFromCompressedFile(String aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedBase64(String aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		public static JSONNode LoadFromFile(String aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		public static JSONNode LoadFromBase64(String aBase64)
		{
			Byte[] buffer = Convert.FromBase64String(aBase64);
			return JSONNode.LoadFromStream(new MemoryStream(buffer)
			{
				Position = 0L
			});
		}

		public static implicit operator JSONNode(String s)
		{
			return new JSONData(s);
		}

		public static implicit operator String(JSONNode d)
		{
			return (!(d == null)) ? d.Value : ((String)null);
		}

		public static Boolean operator ==(JSONNode a, Object b)
		{
			return (b == null && a is JSONLazyCreator) || Object.ReferenceEquals(a, b);
		}

		public static Boolean operator !=(JSONNode a, Object b)
		{
			return !(a == b);
		}
	}
}
