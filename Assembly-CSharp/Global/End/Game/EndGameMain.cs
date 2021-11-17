using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using UnityEngine;
using Object = System.Object;

public class EndGameMain : MonoBehaviour
{
	private void Exit()
	{
		FF9Snd.ff9endsnd_song_vol_intpl(156, 60, 0);
		SoundLib.GetAllSoundDispatchPlayer().StopCurrentSong(60);
		SceneDirector.Replace(PersistenSingleton<SceneDirector>.Instance.LastScene, SceneTransition.FadeOutToBlack_FadeIn, true);
	}

	public void IncreaseWager()
	{
		this.wager += 10L;
        if (this.wager > EndGame.ff9endingGameBankroll)
        {
            this.wager = EndGame.ff9endingGameBankroll;
        }
        else if (this.wager > 1000L)
		{
			this.wager = 1000L;
		}
		else
		{
			this.endGame.FF9SFX_Play(103u);
		}
	}

	public void DecreaseWager()
	{
		this.wager -= 10L;
		if (this.wager < 10L)
		{
			this.wager = 10L;
		}
		else
		{
			this.endGame.FF9SFX_Play(103u);
		}
	}

	public void ValidateWager()
	{
        if (this.wager > EndGame.ff9endingGameBankroll)
        {
            this.wager = EndGame.ff9endingGameBankroll;
        }
    }

	public void CommitBet()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_BET)
		{
			this.endGame.ff9endingGameWager = this.wager;
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
			this.endGame.FF9SFX_Play(103u);
		}
	}

	public void Stand()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_STAND);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
			this.endGame.FF9SFX_Play(103u);
		}
	}

	public void Hit()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_HIT);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
			this.endGame.FF9SFX_Play(103u);
		}
	}

	public void Double()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_DOUBLE);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
			this.endGame.FF9SFX_Play(103u);
		}
	}

	public void Split()
	{
		if (EndSys.Endsys_GetMode() == EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_ACTION)
		{
			EndAct.Endact_SetMode(EndAct.ENDACT_MODE_ENUM.ENDACT_MODE_SPLIT);
			EndSys.Endsys_SetMode(EndSys.ENDSYS_MODE_ENUM.ENDSYS_MODE_QUIT);
			this.endGame.FF9SFX_Play(103u);
		}
	}

	private void Awake()
	{
		EndGameMain.Instance = this;
	}

	private void Start()
	{
		this.endGameScore = new EndGameScore();
		this.InHandGameCardCounts = new List<Int32>();
		for (Int32 i = 0; i < 13; i++)
		{
			for (Int32 j = 0; j < 4; j++)
			{
				this.InHandGameCardCounts.Add(0);
			}
		}
		this.AssambleCards();
		this.endGame = new EndGame();
		this.endGame.Start(this);
        this.bankRoll = EndGame.ff9endingGameBankroll;
        this.wager = this.endGame.ff9endingGameWager;
		this.aspectFit = new AspectFit(1543f, 1080f, this.CanvasCamera);
		this.CanvasPlane.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
		RenderSettings.ambientLight = Color.white;
		if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.Initial)
		{
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.EndGame);
		}
		else
		{
			PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
			PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
			{
				PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.EndGame);
			});
		}
	}

	private void AssambleCards()
	{
		String[] array = new String[]
		{
			"two",
			"three",
			"four",
			"five",
			"six",
			"seven",
			"eight",
			"nine",
			"ten",
			"jack",
			"queen",
			"king",
			"ace"
		};
		String[] array2 = new String[]
		{
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"10",
			"j",
			"q",
			"k",
			"a"
		};
		String[] array3 = new String[]
		{
			"diamonds",
			"clubs",
			"hearts",
			"spades"
		};
		// Todo: allow AssetManager.LoadFromDisc if a mod folder changes it
		Sprite[] array4 = Resources.LoadAll<Sprite>("EmbeddedAsset/EndGame/card_image");
		Dictionary<String, Sprite> dictionary = new Dictionary<String, Sprite>();
		Sprite[] array5 = array4;
		for (Int32 i = 0; i < (Int32)array5.Length; i++)
		{
			Sprite sprite = array5[i];
			dictionary.Add(sprite.name, sprite);
		}
		this.GameCards = new List<List<GameObject>>();
		for (Int32 j = 0; j < 13; j++)
		{
			for (Int32 k = 0; k < 4; k++)
			{
				List<GameObject> list = new List<GameObject>();
				for (Int32 l = 0; l < 6; l++)
				{
					String name = String.Concat(new Object[]
					{
						"card_",
						array[j],
						"_",
						array3[k],
						"_",
						l
					});
					GameObject gameObject = new GameObject(name);
					GameObject gameObject2 = new GameObject("background");
					gameObject2.AddComponent<SpriteRenderer>();
					gameObject2.GetComponent<SpriteRenderer>().sprite = dictionary["card_white.png"];
					gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
					gameObject2.transform.parent = gameObject.transform;
					String str = String.Empty;
					String text = String.Empty;
					switch (k)
					{
					case 0:
						str = "diamond";
						text = "red";
						break;
					case 1:
						str = "club";
						text = "black";
						break;
					case 2:
						str = "heart";
						text = "red";
						break;
					case 3:
						str = "spade";
						text = "black";
						break;
					}
					String text2 = String.Concat(new String[]
					{
						"card_digit_",
						array2[j],
						"_",
						text,
						"_normal.png"
					});
					String text3 = String.Concat(new String[]
					{
						"card_digit_",
						array2[j],
						"_",
						text,
						"_invert.png"
					});
					String text4 = "card_" + str + "_normal.png";
					String text5 = "card_" + str + "_invert.png";
					String[] array6 = new String[]
					{
						text2,
						text3,
						text4,
						text5
					};
					Vector3[] array7 = new Vector3[]
					{
						new Vector3(3.49f, -3.89f, -2f),
						new Vector3(22.4f, -30.3f, -2f),
						new Vector3(3.5f, -10.5f, -2f),
						new Vector3(22.4f, -24.4f, -2f)
					};
					for (Int32 m = 0; m < 4; m++)
					{
						GameObject gameObject3 = new GameObject("symbol_" + array6[m]);
						gameObject3.AddComponent<SpriteRenderer>();
						gameObject3.GetComponent<SpriteRenderer>().sprite = dictionary[array6[m]];
						gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
						gameObject3.transform.localPosition = array7[m];
						gameObject3.transform.parent = gameObject.transform;
					}
					Int32 num = j;
					if (num >= 9)
					{
						if (num == 12)
						{
							Int32[] array8 = new Int32[]
							{
								22,
								30
							};
							Single x = (Single)array8[0] * 0.42f + 3.4f;
							Single y = (Single)array8[1] * -0.45f - 3.9f;
							GameObject gameObject4 = new GameObject("card_face_" + text4);
							gameObject4.AddComponent<SpriteRenderer>();
							gameObject4.GetComponent<SpriteRenderer>().sprite = dictionary[text4];
							gameObject4.transform.localScale = new Vector3(1f, 1f, 1f);
							gameObject4.transform.localPosition = new Vector3(x, y, -2f);
							gameObject4.transform.parent = gameObject.transform;
						}
						else
						{
							String text6 = String.Empty;
							if (num == 9)
							{
								text6 = "card_jack.png";
							}
							else if (num == 10)
							{
								text6 = "card_queen.png";
							}
							else if (num == 11)
							{
								text6 = "card_king.png";
							}
							Int32[] array9 = new Int32[]
							{
								7,
								10
							};
							Single x2 = (Single)array9[0] * 0.42f + 0.7f;
							Single y2 = (Single)array9[1] * -0.45f - 0.7f;
							GameObject gameObject5 = new GameObject("card_face_" + text6);
							gameObject5.AddComponent<SpriteRenderer>();
							gameObject5.GetComponent<SpriteRenderer>().sprite = dictionary[text6];
							gameObject5.transform.localScale = new Vector3(1f, 1f, 1f);
							gameObject5.transform.localPosition = new Vector3(x2, y2, -1.5f);
							gameObject5.transform.parent = gameObject.transform;
						}
					}
					else
					{
						for (Int32 n = 0; n <= num + 1; n++)
						{
							Int32 num2 = EndGameDef.ff9endingGameSuitLoc[num, n, 0];
							Int32 num3 = EndGameDef.ff9endingGameSuitLoc[num, n, 1];
							Single x3 = (Single)num2 * 0.42f + 3.4f;
							Single y3 = (Single)num3 * -0.45f - 3.9f;
							EndGameDef.SuitDirection suitDirection = EndGameDef.ff9endingGameSuitDirection[num, n];
							String text7 = text4;
							if (suitDirection == EndGameDef.SuitDirection.Invert)
							{
								text7 = text5;
							}
							GameObject gameObject6 = new GameObject("card_face_" + text7);
							gameObject6.AddComponent<SpriteRenderer>();
							gameObject6.GetComponent<SpriteRenderer>().sprite = dictionary[text7];
							gameObject6.transform.localScale = new Vector3(1f, 1f, 1f);
							gameObject6.transform.localPosition = new Vector3(x3, y3, -2f);
							gameObject6.transform.parent = gameObject.transform;
						}
					}
					gameObject.transform.parent = this.CanvasPlane.transform;
					gameObject.transform.position = default(Vector3);
					gameObject.transform.localScale = new Vector3(0.069f, 0.1f, 0.1f);
					list.Add(gameObject);
				}
				this.GameCards.Add(list);
			}
		}
	}

	private void Update()
	{
		if (this.aspectFit != null)
		{
			this.aspectFit.setAspectFit();
		}
		this.cumulativeDeltaTime += Time.deltaTime;
		while (this.cumulativeDeltaTime >= 0.0166666675f)
		{
			this.endGame.ResetFrameBasedRenderingObject();
			this.UpdateGameLogic();
			this.cumulativeDeltaTime -= 0.0166666675f;
		}
		this.endGame.ResetTimeBasedAnimation();
		this.endGame.UpdateTimeBasedAnimation(Time.deltaTime);
	}

	private void UpdateGameLogic()
	{
		EndGameState gameState = this.endGame.GetGameState();
		if (gameState != EndGameState.None)
		{
			if (gameState == EndGameState.OnUpdate)
			{
				this.endGame.OnUpdate();
                this.bankRoll = EndGame.ff9endingGameBankroll;
            }
			else if (gameState == EndGameState.Finish)
			{
				this.endGame.Finish();
				this.Exit();
			}
		}
	}

	public const Single canvasWidth = 1543f;

	public const Single canvasHeight = 1080f;

	public const Single originalScreenWidth = 320f;

	public const Single originalScreenHeight = 224f;

	public const Single gameNormalizeTop = -5f;

	public const Single gameNormalizeLeft = 5f;

	public const Single gameNormalizeWidth = -10f;

	public const Single gameNormalizeHeight = 10f;

	public const Single Fps = 60f;

	public const Single FrameTime = 0.0166666675f;

	public static EndGameMain Instance;

	public Camera CanvasCamera;

	public GameObject CanvasPlane;

	public EndGame endGame;

	public Int64 bankRoll;

	public Int64 wager;

	private AspectFit aspectFit;

	public List<GameObject> CardFaceDown;

	public List<GameObject> CardShoe;

	public List<List<GameObject>> GameCards;

	public List<Int32> InHandGameCardCounts;

	public EndGameScore endGameScore;

	public GameObject WinResult;

	public GameObject DrawResult;

	public GameObject LoseResult;

	public GameObject WinResultShadow;

	public GameObject DrawResultShadow;

	public GameObject LoseResultShadow;

	public List<GameObject> CardFaceDownMove;

	private Single cumulativeDeltaTime;
}
