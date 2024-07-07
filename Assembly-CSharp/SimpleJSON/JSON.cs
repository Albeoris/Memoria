using System;

namespace SimpleJSON
{
    public static class JSON
    {
        public static JSONNode Parse(String aJSON)
        {
            return JSONNode.Parse(aJSON);
        }
    }
}
