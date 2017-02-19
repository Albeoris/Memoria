using System;

public class EndGameDef
{
	static EndGameDef()
	{
		// Note: this type is marked as 'beforefieldinit'.
		EndGameDef.SuitDirection[,] array = new EndGameDef.SuitDirection[9, 10];
		array[0, 0] = EndGameDef.SuitDirection.Normal;
		array[0, 1] = EndGameDef.SuitDirection.Invert;
		array[1, 0] = EndGameDef.SuitDirection.Normal;
		array[1, 1] = EndGameDef.SuitDirection.Normal;
		array[1, 2] = EndGameDef.SuitDirection.Invert;
		array[2, 0] = EndGameDef.SuitDirection.Normal;
		array[2, 1] = EndGameDef.SuitDirection.Invert;
		array[2, 2] = EndGameDef.SuitDirection.Normal;
		array[2, 3] = EndGameDef.SuitDirection.Invert;
		array[3, 0] = EndGameDef.SuitDirection.Normal;
		array[3, 1] = EndGameDef.SuitDirection.Invert;
		array[3, 2] = EndGameDef.SuitDirection.Normal;
		array[3, 3] = EndGameDef.SuitDirection.Normal;
		array[3, 4] = EndGameDef.SuitDirection.Invert;
		array[4, 0] = EndGameDef.SuitDirection.Normal;
		array[4, 1] = EndGameDef.SuitDirection.Normal;
		array[4, 2] = EndGameDef.SuitDirection.Invert;
		array[4, 3] = EndGameDef.SuitDirection.Normal;
		array[4, 4] = EndGameDef.SuitDirection.Normal;
		array[4, 5] = EndGameDef.SuitDirection.Invert;
		array[5, 0] = EndGameDef.SuitDirection.Normal;
		array[5, 1] = EndGameDef.SuitDirection.Normal;
		array[5, 2] = EndGameDef.SuitDirection.Invert;
		array[5, 3] = EndGameDef.SuitDirection.Normal;
		array[5, 4] = EndGameDef.SuitDirection.Normal;
		array[5, 5] = EndGameDef.SuitDirection.Normal;
		array[5, 6] = EndGameDef.SuitDirection.Invert;
		array[6, 0] = EndGameDef.SuitDirection.Normal;
		array[6, 1] = EndGameDef.SuitDirection.Normal;
		array[6, 2] = EndGameDef.SuitDirection.Invert;
		array[6, 3] = EndGameDef.SuitDirection.Normal;
		array[6, 4] = EndGameDef.SuitDirection.Invert;
		array[6, 5] = EndGameDef.SuitDirection.Normal;
		array[6, 6] = EndGameDef.SuitDirection.Normal;
		array[6, 7] = EndGameDef.SuitDirection.Invert;
		array[7, 0] = EndGameDef.SuitDirection.Normal;
		array[7, 1] = EndGameDef.SuitDirection.Normal;
		array[7, 2] = EndGameDef.SuitDirection.Invert;
		array[7, 3] = EndGameDef.SuitDirection.Invert;
		array[7, 4] = EndGameDef.SuitDirection.Normal;
		array[7, 5] = EndGameDef.SuitDirection.Normal;
		array[7, 6] = EndGameDef.SuitDirection.Normal;
		array[7, 7] = EndGameDef.SuitDirection.Invert;
		array[7, 8] = EndGameDef.SuitDirection.Invert;
		array[8, 0] = EndGameDef.SuitDirection.Normal;
		array[8, 1] = EndGameDef.SuitDirection.Normal;
		array[8, 2] = EndGameDef.SuitDirection.Invert;
		array[8, 3] = EndGameDef.SuitDirection.Invert;
		array[8, 4] = EndGameDef.SuitDirection.Normal;
		array[8, 5] = EndGameDef.SuitDirection.Invert;
		array[8, 6] = EndGameDef.SuitDirection.Normal;
		array[8, 7] = EndGameDef.SuitDirection.Normal;
		array[8, 8] = EndGameDef.SuitDirection.Invert;
		array[8, 9] = EndGameDef.SuitDirection.Invert;
		EndGameDef.ff9endingGameSuitDirection = array;
	}

	public static Int64 FF9WIPE_SETRGB(Int64 r, Int64 g, Int64 b)
	{
		return r | g << 8 | b << 16;
	}

	public static Int32 ENDGAME_CARD(Int32 n)
	{
		return n / 4;
	}

	public static Int32 ENDGAME_SUIT(Int32 n)
	{
		return n % 4;
	}

	public static Single ENDGAME_FIXEDDIV(Single a, Single b)
	{
		return a / b;
	}

	public static UInt32 ENDGAME_FIXEDMULT(Int32 a, Int32 b)
	{
		return (UInt32)((a >> 8) * (b >> 8));
	}

	public const Int32 NONE = -1;

	public const UInt32 ENDGAME_DMSID_CARD = 20300u;

	public const UInt32 ENDGAME_DMSID_DENOM = 20301u;

	public const UInt32 ENDGAME_DMSID_JACK = 20302u;

	public const UInt32 ENDGAME_DMSID_KING = 20303u;

	public const UInt32 ENDGAME_DMSID_QUEEN = 20304u;

	public const UInt32 ENDGAME_DMSID_RESULTS = 20305u;

	public const UInt32 ENDGAME_DEBUG = 0u;

	public const UInt32 ENDGAME_SELECT_PLAYER = 0u;

	public const UInt32 ENDGAME_PARM_DEPTH = 10u;

	public const Int32 ENDGAME_PARM_BGZ = 16;

	public const UInt32 ENDGAME_PARM_RED = 0u;

	public const UInt32 ENDGAME_PARM_GRN = 64u;

	public const UInt32 ENDGAME_PARM_BLU = 0u;

	public const UInt32 ENDGAME_PARM_CARDVRAMX = 448u;

	public const UInt32 ENDGAME_PARM_CARDVRAMY = 384u;

	public const UInt32 ENDGAME_PARM_DENOMVRAMX = 492u;

	public const UInt32 ENDGAME_PARM_DENOMVRAMY = 256u;

	public const UInt32 ENDGAME_PARM_JACKVRAMX = 448u;

	public const UInt32 ENDGAME_PARM_JACKVRAMY = 256u;

	public const UInt32 ENDGAME_PARM_QUEENVRAMX = 470u;

	public const UInt32 ENDGAME_PARM_QUEENVRAMY = 256u;

	public const UInt32 ENDGAME_PARM_KINGVRAMX = 448u;

	public const UInt32 ENDGAME_PARM_KINGVRAMY = 320u;

	public const UInt32 ENDGAME_PARM_ACEVRAMX = 470u;

	public const UInt32 ENDGAME_PARM_ACEVRAMY = 320u;

	public const UInt32 ENDGAME_PARM_RESULT_VRAMX = 640u;

	public const UInt32 ENDGAME_PARM_RESULT_VRAMY = 256u;

	public const UInt32 ENDGAME_PARM_RESULT_CLUTX = 0u;

	public const UInt32 ENDGAME_PARM_RESULT_CLUTY = 233u;

	public const Int32 ENDGAME_PARM_COUNTX = 10;

	public const Int32 ENDGAME_PARM_COUNTYOFFSET = 32;

	public const Int32 ENDGAME_PARM_SHOEX = 240;

	public const Int32 ENDGAME_PARM_SHOEY = 10;

	public const Int32 ENDGAME_PARM_DEALERX = 30;

	public const Int32 ENDGAME_PARM_DEALERY = 10;

	public const Int32 ENDGAME_PARM_PLAYERX = 30;

	public const Int32 ENDGAME_PARM_PLAYERY = 120;

	public const Int32 ENDGAME_PARM_PLAYERSPLITY = 140;

	public const Int32 ENDGAME_PARM_SPLITX = 30;

	public const Int32 ENDGAME_PARM_SPLITY = 96;

	public const Int32 ENDGAME_PARM_OFFSETX = 13;

	public const Int32 ENDGAME_PARM_OFFSETY = 0;

	public const UInt32 ENDGAME_PARM_BUZZTIME = 7u;

	public const UInt32 SFX_EVENT_SYSTEM_SE000001 = 103u;

	public const UInt32 ENDGAME_PARM_SFX_WIN = 108u;

	public const UInt32 ENDGAME_PARM_SFX_LOSE = 107u;

	public const Int32 ENDGAME_PARM_DEALFRAMES = 30;

	public const Int32 ENDGAME_PARM_FADEINFRAMES = 60;

	public const Int32 ENDGAME_PARM_FADEOUTFRAMES = 60;

	public const Int32 ENDGAME_PARM_BUSTFRAMES = 10;

	public const Int32 ENDGAME_PARM_SPLITFRAMES = 148;

	public const Int32 ENDGAME_PARM_BLACKJACKFRAMES = 148;

	public const Int32 ENDGAME_PARM_WINFRAMES = 148;

	public const Int32 ENDGAME_PARM_PUSHFRAMES = 148;

	public const Int32 ENDGAME_PARM_LOSSFRAMES = 148;

	public const Int32 ENDGAME_PARM_SONG_FADEINFRAME = 90;

	public const UInt32 ENDGAME_PARM_TIMCOUNT = 6u;

	public const UInt32 ENDGAME_PARM_TPAGECOUNT = 16u;

	public const UInt32 ENDGAME_PARM_SPRITECOUNT = 256u;

	public const Int32 ENDGAME_PARM_DECKCOUNT = 6;

	public const Int32 ENDGAME_PARM_CARDSPERDECK = 52;

	public const Int32 ENDGAME_PARM_CARDSTOTAL = 312;

	public const Int32 ENDGAME_PARM_CUTCARD = 249;

	public const UInt32 ENDGAME_PARM_DEFAULT_WAGER = 10u;

	public const UInt16 ENDGAME_FLAG_NEWDECK = 1;

	public const UInt16 ENDGAME_FLAG_SHUFFLED = 2;

	public const UInt16 ENDGAME_FLAG_REDEAL = 4;

	public const UInt32 ENDGAME_FLAG_HAND_SOFT = 1u;

	public const UInt32 ENDGAME_FLAG_HAND_BUST = 2u;

	public const UInt32 ENDGAME_FLAG_HAND_DOUBLED = 4u;

	public const UInt32 ENDGAME_FLAG_HAND_SPLITACES = 8u;

	public const UInt32 ENDGAME_FLAG_HAND_HIDEHOLECARD = 16u;

	public const UInt64 ENDGAME_FLAG_HAND_CANSPLIT = 1UL;

	public const UInt32 ENDGAME_FLAG_HAND_CANDOUBLE = 2u;

	public static UInt16[,] ff9endingGameTIMNoArray = new UInt16[,]
	{
		{
			0,
			1,
			2,
			3,
			4,
			5
		},
		{
			0,
			1,
			2,
			3,
			4,
			5
		},
		{
			0,
			1,
			2,
			3,
			4,
			5
		},
		{
			0,
			1,
			2,
			3,
			4,
			5
		},
		{
			0,
			1,
			2,
			3,
			4,
			5
		},
		{
			0,
			1,
			2,
			3,
			4,
			5
		}
	};

	public static UInt16[] ff9endingGameWhitePaletteStrip = new UInt16[]
	{
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue,
		UInt16.MaxValue
	};

	public static Int32[,,] ff9endingGameSuitLoc = new Int32[,,]
	{
		{
			{
				22,
				10
			},
			{
				22,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				22,
				10
			},
			{
				22,
				30
			},
			{
				22,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				50
			},
			{
				32,
				10
			},
			{
				32,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				50
			},
			{
				22,
				30
			},
			{
				32,
				10
			},
			{
				32,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				30
			},
			{
				12,
				50
			},
			{
				32,
				10
			},
			{
				32,
				30
			},
			{
				32,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				30
			},
			{
				12,
				50
			},
			{
				22,
				19
			},
			{
				32,
				10
			},
			{
				32,
				30
			},
			{
				32,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				30
			},
			{
				12,
				50
			},
			{
				22,
				19
			},
			{
				22,
				39
			},
			{
				32,
				10
			},
			{
				32,
				30
			},
			{
				32,
				50
			},
			{
				0,
				0
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				24
			},
			{
				12,
				37
			},
			{
				12,
				50
			},
			{
				22,
				30
			},
			{
				32,
				10
			},
			{
				32,
				24
			},
			{
				32,
				37
			},
			{
				32,
				50
			},
			{
				0,
				0
			}
		},
		{
			{
				12,
				10
			},
			{
				12,
				24
			},
			{
				12,
				37
			},
			{
				12,
				50
			},
			{
				22,
				19
			},
			{
				22,
				39
			},
			{
				32,
				10
			},
			{
				32,
				24
			},
			{
				32,
				37
			},
			{
				32,
				50
			}
		}
	};

	public static EndGameDef.SuitDirection[,] ff9endingGameSuitDirection;

	public enum FF9WIPE_TYPE_ENUM
	{
		FF9WIPE_TYPE_NONE,
		FF9WIPE_TYPE_FADE,
		FF9WIPE_TYPE_ANGLE,
		FF9WIPE_TYPE_MAX
	}

	public enum FF9WIPE_MODE_ENUM
	{
		FF9WIPE_MODE_ADD,
		FF9WIPE_MODE_SUB,
		FF9WIPE_MODE_MAX
	}

	public enum FF9FONT_COL_ENUM
	{
		FF9FONT_COL_WHITE,
		FF9FONT_COL_BLACK,
		FF9FONT_COL_GRAY,
		FF9FONT_COL_RED,
		FF9FONT_COL_YELLOW,
		FF9FONT_COL_CYAN,
		FF9FONT_COL_MAGENTA,
		FF9FONT_COL_GREEN,
		FF9FONT_COL_HELP,
		FF9FONT_COL_HELPRED,
		FF9FONT_COL_MAX
	}

	public enum ENDGAME_SUIT_ENUM
	{
		ENDGAME_SUIT_DIAMONDS,
		ENDGAME_SUIT_CLUBS,
		ENDGAME_SUIT_HEARTS,
		ENDGAME_SUIT_SPADES,
		ENDGAME_SUIT_COUNT
	}

	public enum ENDGAME_CARD_ENUM
	{
		ENDGAME_CARD_TWO,
		ENDGAME_CARD_THREE,
		ENDGAME_CARD_FOUR,
		ENDGAME_CARD_FIVE,
		ENDGAME_CARD_SIX,
		ENDGAME_CARD_SEVEN,
		ENDGAME_CARD_EIGHT,
		ENDGAME_CARD_NINE,
		ENDGAME_CARD_TEN,
		ENDGAME_CARD_JACK,
		ENDGAME_CARD_QUEEN,
		ENDGAME_CARD_KING,
		ENDGAME_CARD_ACE,
		ENDGAME_CARD_COUNT
	}

	public enum ENDGAME_RESULT_ENUM
	{
		ENDGAME_RESULT_WIN,
		ENDGAME_RESULT_LOSE,
		ENDGAME_RESULT_DRAW,
		ENDGAME_RESULT_COUNT
	}

	public class DECK_DEF
	{
		public DECK_DEF()
		{
			this.ndxList = new Int32[312];
		}

		public UInt16 flags;

		public UInt16 cardNdx;

		public Int32[] ndxList;
	}

	public class HAND_DEF
	{
		public HAND_DEF()
		{
			this.pos = new Int32[2];
			this.cardNdx = new UInt64[22];
		}

		public UInt32 flags;

		public UInt32 cardCount;

		public UInt32 cardTotal;

		public UInt32 minTotal;

		public Int32[] pos;

		public UInt64[] cardNdx;
	}

	public enum FF9EndingGameState
	{
		ENDGAME_STATE_INIT,
		ENDGAME_STATE_SHUFFLE,
		ENDGAME_STATE_WAGER,
		ENDGAME_STATE_DEAL,
		ENDGAME_STATE_PROCESSPLAYER,
		ENDGAME_STATE_PROCESSDEALER,
		ENDGAME_STATE_SPLITCHECK,
		ENDGAME_STATE_BUST,
		ENDGAME_STATE_BLACKJACK,
		ENDGAME_STATE_WIN,
		ENDGAME_STATE_PUSH,
		ENDGAME_STATE_LOSS,
		ENDGAME_STATE_NEXTHAND,
		ENDGAME_STATE_LEAVEGAME,
		ENDGAME_STATE_END
	}

	public enum ENDGAME_DMSID_ENUM
	{
		ENDGAME_DMSID_CARD,
		ENDGAME_DMSID_DENOM,
		ENDGAME_DMSID_JACK,
		ENDGAME_DMSID_KING,
		ENDGAME_DMSID_QUEEN,
		ENDGAME_DMSID_RESULTS,
		ENDGAME_DMSID_COUNT
	}

	public enum SuitDirection
	{
		None,
		Normal,
		Invert
	}
}
