using System;
using System.Collections.Generic;

public class MatchResult
{
	public MatchResult()
	{
		this.selectable = new List<Int32>();
	}

	public MatchResult.Type type;

	public Boolean perfect;

	public QuadMistCard selectedCard;

	public List<Int32> selectable;

	public enum Type
	{
		WIN,
		LOSE,
		DRAW
	}
}
