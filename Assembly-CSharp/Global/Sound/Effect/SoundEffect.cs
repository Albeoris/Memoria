using System;
using System.Collections.Generic;

public static class SoundEffect
{
	public static void Play(QuadMistSoundID type)
	{
		FF9Snd.ff9minisnd_sndeffect_play(SoundEffect.soundIdMap[type], 0, 127, 128);
	}

	public static void Stop(QuadMistSoundID type)
	{
		FF9Snd.ff9minisnd_sndeffect_stop(SoundEffect.soundIdMap[type], 0);
	}

	public static Dictionary<QuadMistSoundID, Int32> soundIdMap = new Dictionary<QuadMistSoundID, Int32>
	{
		{ QuadMistSoundID.MINI_SE_CURSOL,		1853 },
		{ QuadMistSoundID.MINI_SE_WARNING,		1854 },
		{ QuadMistSoundID.MINI_SE_CANCEL,		1855 },
		{ QuadMistSoundID.MINI_SE_CARD_GET,		1856 },
		{ QuadMistSoundID.MINI_SE_CARD_MOVE,	1857 },
		{ QuadMistSoundID.MINI_SE_COIN,			1858 },
		{ QuadMistSoundID.MINI_SE_WALL,			1859 },
		{ QuadMistSoundID.MINI_SE_PERFECT,		1852 },
		{ QuadMistSoundID.MINI_SE_FLASH,		1861 },
		{ QuadMistSoundID.MINI_SE_COMB,			1862 },
		{ QuadMistSoundID.MINI_SE_BOMB,			1863 },
		{ QuadMistSoundID.MINI_SE_WINDOW,		1864 },
		{ QuadMistSoundID.MINI_SE_WIN,			2331 },
		{ QuadMistSoundID.MINI_SE_LOSE,			2332 },
		{ QuadMistSoundID.MINI_SE_DRAW,			2333 }
	};
}
