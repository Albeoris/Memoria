using System;
using System.Collections.Generic;

public class FF9SAVE_MINIGAME
{
	public const Int32 MiniGameCardMax = 100;

	public Int16 sWin;

	public Int16 sLose;

	public Int16 sDraw;

	public List<QuadMistCard> MiniGameCard = new List<QuadMistCard>();
}
