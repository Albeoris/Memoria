using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleJSON
{
	public class JSONClass : JSONNode, IEnumerable
	{
		public Dictionary<String, JSONNode> Dict
		{
			get
			{
				return this.m_Dict;
			}
			set
			{
				this.m_Dict = value;
			}
		}

		public override JSONNode this[String aKey]
		{
			get
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					return this.m_Dict[aKey];
				}
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = value;
				}
				else
				{
					this.m_Dict.Add(aKey, value);
				}
			}
		}

		public override JSONNode this[Int32 aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return (JSONNode)null;
				}
				return this.m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return;
				}
				String key = this.m_Dict.ElementAt(aIndex).Key;
				this.m_Dict[key] = value;
			}
		}

		public override Int32 Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		public override void Add(String aKey, JSONNode aItem)
		{
			if (!String.IsNullOrEmpty(aKey))
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = aItem;
				}
				else
				{
					this.m_Dict.Add(aKey, aItem);
				}
			}
			else
			{
				this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
			}
		}

		public override JSONNode Remove(String aKey)
		{
			if (!this.m_Dict.ContainsKey(aKey))
			{
				return (JSONNode)null;
			}
			JSONNode result = this.m_Dict[aKey];
			this.m_Dict.Remove(aKey);
			return result;
		}

		public override JSONNode Remove(Int32 aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_Dict.Count)
			{
				return (JSONNode)null;
			}
			KeyValuePair<String, JSONNode> keyValuePair = this.m_Dict.ElementAt(aIndex);
			this.m_Dict.Remove(keyValuePair.Key);
			return keyValuePair.Value;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			JSONNode result;
			try
			{
				KeyValuePair<String, JSONNode> keyValuePair = (from k in this.m_Dict
															   where k.Value == aNode
															   select k).First<KeyValuePair<String, JSONNode>>();
				this.m_Dict.Remove(keyValuePair.Key);
				result = aNode;
			}
			catch
			{
				result = (JSONNode)null;
			}
			return result;
		}

		public override IEnumerable<JSONNode> Childs
		{
			get
			{
				foreach (KeyValuePair<String, JSONNode> N in this.m_Dict)
				{
					yield return N.Value;
				}
				yield break;
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<String, JSONNode> N in this.m_Dict)
			{
				yield return N;
			}
			yield break;
		}

		public override String ToString()
		{
			String text = "{";
			foreach (KeyValuePair<String, JSONNode> keyValuePair in this.m_Dict)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				String text2 = text;
				text = String.Concat(new String[]
				{
					text2,
					"\"",
					JSONNode.Escape(keyValuePair.Key),
					"\":",
					keyValuePair.Value.ToString()
				});
			}
			text += "}";
			return text;
		}

		public override String ToString(String aPrefix)
		{
			String text = "{ ";
			foreach (KeyValuePair<String, JSONNode> keyValuePair in this.m_Dict)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				String text2 = text;
				text = String.Concat(new String[]
				{
					text2,
					"\"",
					JSONNode.Escape(keyValuePair.Key),
					"\" : ",
					keyValuePair.Value.ToString(aPrefix + "   ")
				});
			}
			text = text + "\n" + aPrefix + "}";
			return text;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(2);
			aWriter.Write(this.m_Dict.Count);
			foreach (String text in this.m_Dict.Keys)
			{
				aWriter.Write(text);
				this.m_Dict[text].Serialize(aWriter);
			}
		}

		private Dictionary<String, JSONNode> m_Dict = new Dictionary<String, JSONNode>();
	}
}
