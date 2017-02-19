using System;

public class EndGameScore
{
	public void ClearScores()
	{
		this.dealerCardTotal = (String)null;
		this.splitCardTotal = (String)null;
		this.splitMinTotal = (String)null;
		this.playerCardTotal = (String)null;
		this.playerMinTotal = (String)null;
	}

	public String dealerCardTotal;

	public String splitCardTotal;

	public String splitMinTotal;

	public String playerCardTotal;

	public String playerMinTotal;
}
