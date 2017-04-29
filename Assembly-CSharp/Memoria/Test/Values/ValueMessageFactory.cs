using System;
using System.IO;
using UnityEngine;

namespace Memoria.Test
{
    public static class ValueMessageFactory
    {
        public static IValueMessage Instantiate(ValueMessageType type)
        {
            switch (type)
            {
                case ValueMessageType.Boolean:
                    return new BooleanMessage(false);
                case ValueMessageType.Int8:
                    return new Int8Message(0);
                case ValueMessageType.UInt8:
                    return new UInt8Message(0);
                case ValueMessageType.Int16:
                    return new Int16Message(0);
                case ValueMessageType.UInt16:
                    return new UInt16Message(0);
                case ValueMessageType.Int32:
                    return new Int32Message(0);
                case ValueMessageType.UInt32:
                    return new UInt32Message(0);
                case ValueMessageType.Int64:
                    return new Int64Message(0);
                case ValueMessageType.UInt64:
                    return new UInt64Message(0);
                case ValueMessageType.Single:
                    return new SingleMessage(0);
                case ValueMessageType.Double:
                    return new DoubleMessage(0);
                case ValueMessageType.Decimal:
                    return new DecimalMessage(0);
                case ValueMessageType.String:
                    return new StringMessage(String.Empty);

                case ValueMessageType.Vector2:
                    return new Vector2Message(Vector2.zero);
                case ValueMessageType.Vector3:
                    return new Vector3Message(Vector3.zero);
                case ValueMessageType.Vector4:
                    return new Vector4Message(Vector4.zero);
                case ValueMessageType.Quaternion:
                    return new QuaternionMessage(Quaternion.identity);

                case ValueMessageType.UiRectPosition:
                    return new UiRectPositionMessage(new UIRect.Position());

                default:
                    throw new NotSupportedException(type.ToString());
            }
        }

        public static void Serialize(BinaryWriter bw, IValueMessage message)
        {
            bw.Write(message.ValueType);
            message.Serialize(bw);
        }

        public static IValueMessage Deserialize(BinaryReader br)
        {
            ValueMessageType type = br.ReadValueMessageType();
            IValueMessage value = Instantiate(type);
            value.Deserialize(br);
            return value;
        }
    }
}