namespace FF8.JSM
{
    public static partial class Jsm
    {
        public static partial class Expression
        {
            public enum VariableSource
            {
                Global = 0,
                Map = 1,
                Instance = 2,
                Object = 4,
                System = 5,
                Member = 6,
                Const = 7
            }
        }
    }
}