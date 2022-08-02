using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ini
{
    public class IniFile
    {
        public String path;

        public IniFile(String INIPath)
        {
            this.path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern Int64 WritePrivateProfileString(String section, String key, String val, String filePath);

        [DllImport("kernel32")]
        private static extern Int32 GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, Int32 size, String filePath);

        public void WriteValue(String Section, String Key, String Value)
        {
            IniFile.WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public String ReadValue(String Section, String Key)
        {
            _retBuffer.Clear();
            IniFile.GetPrivateProfileString(Section, Key, "", _retBuffer, _bufferSize, this.path);
            return _retBuffer.ToString();
        }

        private const Int32 _bufferSize = 10000;
        private StringBuilder _retBuffer = new StringBuilder(_bufferSize);
    }
}
