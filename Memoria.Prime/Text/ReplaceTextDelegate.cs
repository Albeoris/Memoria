using System;
using System.Text;

namespace Memoria.Prime.Text
{
	public delegate String ReplaceTextDelegate(String str, StringBuilder word, ref Int32 index, ref Int32 length);
}
