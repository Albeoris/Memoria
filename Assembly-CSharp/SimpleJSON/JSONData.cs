using System;
using System.IO;

namespace SimpleJSON
{
    public class JSONData : JSONNode
    {
        public JSONData(String aData)
        {
            this.m_Data = aData;
        }

        public JSONData(Single aData)
        {
            this.AsFloat = aData;
        }

        public JSONData(Double aData)
        {
            this.AsDouble = aData;
        }

        public JSONData(Boolean aData)
        {
            this.AsBool = aData;
        }

        public JSONData(Int32 aData)
        {
            this.AsInt = aData;
        }

        public override String Value
        {
            get
            {
                return this.m_Data;
            }
            set
            {
                this.m_Data = value;
            }
        }

        public override String ToString()
        {
            return "\"" + JSONNode.Escape(this.m_Data) + "\"";
        }

        public override String ToString(String aPrefix)
        {
            return "\"" + JSONNode.Escape(this.m_Data) + "\"";
        }

        public override void Serialize(BinaryWriter aWriter)
        {
            JSONData jsondata = new JSONData(String.Empty);
            jsondata.AsInt = this.AsInt;
            if (jsondata.m_Data == this.m_Data)
            {
                aWriter.Write(4);
                aWriter.Write(this.AsInt);
                return;
            }
            jsondata.AsFloat = this.AsFloat;
            if (jsondata.m_Data == this.m_Data)
            {
                aWriter.Write(7);
                aWriter.Write(this.AsFloat);
                return;
            }
            jsondata.AsDouble = this.AsDouble;
            if (jsondata.m_Data == this.m_Data)
            {
                aWriter.Write(5);
                aWriter.Write(this.AsDouble);
                return;
            }
            jsondata.AsBool = this.AsBool;
            if (jsondata.m_Data == this.m_Data)
            {
                aWriter.Write(6);
                aWriter.Write(this.AsBool);
                return;
            }
            aWriter.Write(3);
            aWriter.Write(this.m_Data);
        }

        private String m_Data;
    }
}
