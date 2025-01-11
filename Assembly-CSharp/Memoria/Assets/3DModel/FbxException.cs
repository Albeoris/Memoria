using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    /// <summary>An error with the FBX data input</summary>
    public class FbxException : Exception
    {
        /// <summary>An error at a binary stream offset</summary>
        /// <param name="position"></param>
        /// <param name="message"></param>
        public FbxException(long position, string message) :
            base($"{message}, near offset {position}")
        {
        }

        /// <summary>An error in a text file</summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="message"></param>
        public FbxException(int line, int column, string message) :
            base($"{message}, near line {line} column {column}")
        {
        }

        /// <summary>An error in a node object</summary>
        /// <param name="nodePath"></param>
        /// <param name="propertyID"></param>
        /// <param name="message"></param>
        public FbxException(Stack<string> nodePath, int propertyID, string message) :
            base(message + ", at " + string.Join("/", nodePath.ToArray()) + (propertyID < 0 ? "" : $"[{propertyID}]"))
        {
        }
    }
}
