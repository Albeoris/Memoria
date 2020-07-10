using System;

namespace NCalc
{
    public class ParameterArgs : EventArgs
    {
        private object result;

        public object Result
        {
            get { return result; }
            set
            {
                result = value;
                HasResult = true;
            }
        }

        public bool HasResult { get; set; }
    }
}