using System;

namespace SimpleJSON
{
    internal class JSONLazyCreator : JSONNode
    {
        public JSONLazyCreator(JSONNode aNode)
        {
            this.m_Node = aNode;
            this.m_Key = (String)null;
        }

        public JSONLazyCreator(JSONNode aNode, String aKey)
        {
            this.m_Node = aNode;
            this.m_Key = aKey;
        }

        private void Set(JSONNode aVal)
        {
            if (this.m_Key == null)
            {
                this.m_Node.Add(aVal);
            }
            else
            {
                this.m_Node.Add(this.m_Key, aVal);
            }
            this.m_Node = (JSONNode)null;
        }

        public override JSONNode this[Int32 aIndex]
        {
            get
            {
                return new JSONLazyCreator(this);
            }
            set
            {
                this.Set(new JSONArray
                {
                    value
                });
            }
        }

        public override JSONNode this[String aKey]
        {
            get
            {
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                this.Set(new JSONClass
                {
                    {
                        aKey,
                        value
                    }
                });
            }
        }

        public override void Add(JSONNode aItem)
        {
            this.Set(new JSONArray
            {
                aItem
            });
        }

        public override void Add(String aKey, JSONNode aItem)
        {
            this.Set(new JSONClass
            {
                {
                    aKey,
                    aItem
                }
            });
        }

        public override Boolean Equals(Object obj)
        {
            return obj == null || Object.ReferenceEquals(this, obj);
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        public override String ToString()
        {
            return String.Empty;
        }

        public override String ToString(String aPrefix)
        {
            return String.Empty;
        }

        public override Int32 AsInt
        {
            get
            {
                JSONData aVal = new JSONData(0);
                this.Set(aVal);
                return 0;
            }
            set
            {
                JSONData aVal = new JSONData(value);
                this.Set(aVal);
            }
        }

        public override Single AsFloat
        {
            get
            {
                JSONData aVal = new JSONData(0f);
                this.Set(aVal);
                return 0f;
            }
            set
            {
                JSONData aVal = new JSONData(value);
                this.Set(aVal);
            }
        }

        public override Double AsDouble
        {
            get
            {
                JSONData aVal = new JSONData(0.0);
                this.Set(aVal);
                return 0.0;
            }
            set
            {
                JSONData aVal = new JSONData(value);
                this.Set(aVal);
            }
        }

        public override Boolean AsBool
        {
            get
            {
                JSONData aVal = new JSONData(false);
                this.Set(aVal);
                return false;
            }
            set
            {
                JSONData aVal = new JSONData(value);
                this.Set(aVal);
            }
        }

        public override JSONArray AsArray
        {
            get
            {
                JSONArray jsonarray = new JSONArray();
                this.Set(jsonarray);
                return jsonarray;
            }
        }

        public override JSONClass AsObject
        {
            get
            {
                JSONClass jsonclass = new JSONClass();
                this.Set(jsonclass);
                return jsonclass;
            }
        }

        public static Boolean operator ==(JSONLazyCreator a, Object b)
        {
            return b == null || Object.ReferenceEquals(a, b);
        }

        public static Boolean operator !=(JSONLazyCreator a, Object b)
        {
            return !(a == b);
        }

        private JSONNode m_Node;

        private String m_Key;
    }
}
