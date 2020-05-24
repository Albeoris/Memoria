using System.Collections;
using System.Collections.Generic;
using System.IO;
using Memoria.EventEngine.EV;

namespace Memoria.Test
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
	    public static void Main(string[] args)
	    {
		    // foreach (var file in Directory.GetFiles(@"EventEngine", "*.eb.bytes"))
		    // {
		    //     using var input = File.OpenRead(file);
		    //     EVFileReader reader = new EVFileReader(input);
		    //     reader.Read();
		    // }

		    using (var input = File.OpenRead(@"EventEngine\EVT_ALEX1_TS_CARGO_0.eb.bytes"))
		    {
			    EVFileReader reader = new EVFileReader(input);
			    EVObject[] objects = reader.Read();
		    }
	    }
    };
}