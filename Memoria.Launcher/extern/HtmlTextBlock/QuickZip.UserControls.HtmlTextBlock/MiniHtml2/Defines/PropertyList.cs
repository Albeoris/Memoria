using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;


namespace QuickZip.MiniHtml2
{
    /// <summary>
    /// An Item entry, can store a string and an object (Pie)
    /// </summary>
    public class PropertyItemType 
    {
    	public string key;
        public string value;
        public object attachment;
        public PropertyItemType(string aKey, string aValue)
        {
        	key = aKey;
            value = aValue;
        }
        public PropertyItemType(string aKey, object anAttachment)
        {   
        	key = aKey;
            attachment = anAttachment;
        }
        public PropertyItemType(string aKey, string aValue, object anAttachment)
        {
        	key = aKey;
            value = aValue;
            attachment = anAttachment;
        }
    }
    /// <summary>
    /// Variable List that use "Key" to store PropertyItemType
    /// </summary>
    public class PropertyList : ListDictionary
    {
        public bool createIfNotExist = true;

        /// <summary>
        /// Get an item from the list
        /// </summary>
        private PropertyItemType getPropertyInfo(string aKey)
        {
            if (Contains(aKey))
            {
                foreach (DictionaryEntry de in this)
                {
                    if ((string)de.Key == aKey)
                    {
                        return (PropertyItemType)de.Value;
                    }
                }
            }

            PropertyItemType retVal = new PropertyItemType(aKey, "");
            if (createIfNotExist) { Add(aKey, retVal); }
            return retVal;
        }
        /// <summary>
        /// Get an item from the list
        /// </summary>
        private PropertyItemType getPropertyInfo(Int32 anId)
        {            
            IDictionaryEnumerator Enum = GetEnumerator();
            if (Count >= anId)
            {
                for (int i = 0; i <= anId; i++) { Enum.MoveNext(); }
                return (PropertyItemType)Enum.Value;
            }
            return new PropertyItemType(anId.ToString(),"");
        }
        /// <summary>
        /// Set or Add an item to the list
        /// </summary>
        private void setPropertyInfo(string aKey, PropertyItemType aValue)
        {
            VerifyType(aValue);
            if (Contains(aKey))
            {
                this.Remove(aKey);
            }
            Add(aKey, aValue);
        }
        /// <summary>
        /// Check item before add.
        /// </summary>        
        private void VerifyType(object value)
        {
            if (!(value is PropertyItemType))
            { throw new ArgumentException("Invalid Type."); }
        }
        /// <summary>
        /// Add a new PropertyInfo
        /// </summary>
        public void Add(string aKey, string aValue)
        {
            Add(aKey, new PropertyItemType(aKey, aValue));
        }
        /// <summary>
        /// Add a new PropertyInfo
        /// </summary>
        public void Add(string aKey, string aValue, object anAttachment)
        {
            Add(aKey, new PropertyItemType(aKey, aValue,anAttachment));
        }
        /// <summary>
        /// Retrieve a PropertyItem using a key
        /// </summary>
        public PropertyItemType this[String aKey]
        {
            get
            {
                return getPropertyInfo(aKey);
            }
            set
            {
                setPropertyInfo(aKey, value);
            }
        }
        /// <summary>
        /// Retrieve a PropertyItem using an id
        /// </summary>
        public PropertyItemType this[Int32 anId]
        {
            get
            {
                return getPropertyInfo(anId);
            }

        }
        
        public string Html()
        {
        	string retVal = "";
        	for (Int32 i = 0; i < this.Count; i++)
        	{
        		PropertyItemType item = this[i];
        		retVal += " " + item.key + "=\"" + item.value + "\"";
        	}
        		
        	return retVal;
        }
        
        
        public PropertyList Clone()
        {
        	PropertyList retVal = new PropertyList();
        	foreach (PropertyItemType item in this)
        		retVal.Add(item.key, item.value);
        	return retVal;
        }
        
		public override string ToString()
		{
			string retVal = "";
			for (int i = 0; i < Count; i++)
				retVal += String.Format(" {0}=\"{1}\"; ", this[i].key, this[i].value);
			if (retVal == "")
				return " ";
			else return retVal;
		}
        
    
        //public static void DebugUnit()
        //{
        //    PropertyList list = new PropertyList();
        //    list["abc"].value = "abcd";
        //    list["bcd"].value = "bcde";
        //    list["cde"].value = "cdef";
        //    list.Remove("abc");

        //    Debug.Assert((list["bcd"].value == "bcde"), "PropertyList Failed.");
        //    Debug.Assert((list["abc"].value == ""), "PropertyList Failed.");
        //}

        public static PropertyList FromString(string s)
        {
            return Utils.ExtravtVariables(s);
        }

        

    }
}
