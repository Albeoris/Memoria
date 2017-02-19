using System;

public class DEF_PARAMS
{
	public DEF_PARAMS()
	{
		this.p_def = 0;
		this.p_ev = 0;
		this.m_def = 0;
		this.m_ev = 0;
	}

	public DEF_PARAMS(Byte p_def, Byte p_ev, Byte m_def, Byte m_ev)
	{
		this.p_def = p_def;
		this.p_ev = p_ev;
		this.m_def = m_def;
		this.m_ev = m_ev;
	}

	public Byte p_def;

	public Byte p_ev;

	public Byte m_def;

	public Byte m_ev;
}
