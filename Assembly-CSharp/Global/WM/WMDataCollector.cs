using System;
using System.Collections.Generic;

public class WMDataCollector : Singleton<WMDataCollector>
{
	public List<Int32> VisitedBlockNumbers = new List<Int32>();
}
