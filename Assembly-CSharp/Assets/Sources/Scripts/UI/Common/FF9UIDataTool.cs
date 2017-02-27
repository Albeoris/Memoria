using System;
using System.Collections.Generic;
using FF9;
using Memoria.Assets;
using UnityEngine;
using XInputDotNetPure;

namespace Assets.Sources.Scripts.UI.Common
{
	public class FF9UIDataTool
	{
		public static void DisplayItem(Int32 itemId, UISprite itemIcon, UILabel itemName, Boolean isEnable)
		{
			if (itemId != 255)
			{
				FF9ITEM_DATA ff9ITEM_DATA = ff9item._FF9Item_Data[itemId];
				Byte b = (Byte)((!isEnable) ? 15 : ff9ITEM_DATA.color);
				if (itemIcon != (UnityEngine.Object)null)
				{
					itemIcon.spriteName = "item" + ff9ITEM_DATA.shape.ToString("0#") + "_" + b.ToString("0#");
					itemIcon.alpha = ((!isEnable) ? 0.5f : 1f);
				}
				if (itemName != (UnityEngine.Object)null)
				{
					itemName.text = FF9TextTool.ItemName(itemId);
					itemName.color = ((!isEnable) ? FF9TextTool.Gray : FF9TextTool.White);
				}
			}
			else
			{
				if (itemIcon != (UnityEngine.Object)null)
				{
					itemIcon.spriteName = String.Empty;
				}
				if (itemName != (UnityEngine.Object)null)
				{
					itemName.text = String.Empty;
				}
			}
		}

		public static void DisplayCharacterDetail(PLAYER player, CharacterDetailHUD charHud)
		{
			charHud.Self.SetActive(true);
			charHud.NameLabel.text = player.name;
			charHud.LvLabel.text = player.level.ToString();
			Color color = (player.cur.hp != 0) ? ((player.cur.hp > player.max.hp / 6) ? FF9TextTool.White : FF9TextTool.Yellow) : FF9TextTool.Red;
			charHud.HPLabel.text = player.cur.hp.ToString();
			charHud.HPMaxLabel.text = player.max.hp.ToString();
			charHud.HPTextColor = color;
			color = ((player.cur.mp > player.max.mp / 6) ? FF9TextTool.White : FF9TextTool.Yellow);
			charHud.MPLabel.text = player.cur.mp.ToString();
			charHud.MPMaxLabel.text = player.max.mp.ToString();
			charHud.MPTextColor = color;
			if (charHud.MagicStoneLabel != (UnityEngine.Object)null)
			{
				charHud.MagicStoneLabel.text = player.cur.capa.ToString();
				charHud.MagicStoneMaxLabel.text = player.max.capa.ToString();
				charHud.MagicStoneTextColor = ((player.cur.capa != 0) ? FF9TextTool.White : FF9TextTool.Yellow);
			}
			if (charHud.StatusesSpriteList != null)
			{
				Int32 num = 0;
				UISprite[] statusesSpriteList = charHud.StatusesSpriteList;
				for (Int32 i = 0; i < (Int32)statusesSpriteList.Length; i++)
				{
					UISprite uisprite = statusesSpriteList[i];
					if (((Int32)player.status & 1 << num) != 0)
					{
						uisprite.spriteName = FF9UIDataTool.IconSpriteName[FF9UIDataTool.status_id[num]];
						uisprite.alpha = 1f;
					}
					else
					{
						uisprite.alpha = 0f;
					}
					num++;
				}
			}
		}

		public static void DisplayCharacterAvatar(PLAYER player, Vector3 frontPos, Vector3 backPos, UISprite avatarSprite, Boolean rowUpdate)
		{
			avatarSprite.spriteName = FF9UIDataTool.AvatarSpriteName(player.info.serial_no);
			avatarSprite.alpha = ((player.cur.hp != 0) ? 1f : 0.5f);
			if (rowUpdate)
			{
				if (player.info.row == 1)
				{
					avatarSprite.transform.localPosition = frontPos;
				}
				else if (player.info.row == 0)
				{
					avatarSprite.transform.localPosition = backPos;
				}
			}
		}

		public static void DisplayCharacterAvatar(Int32 serialId, Vector3 frontPos, Vector3 backPos, UISprite avatarSprite, Boolean rowUpdate)
		{
			avatarSprite.spriteName = FF9UIDataTool.AvatarSpriteName((Byte)serialId);
		}

		public static void DisplayCard(QuadMistCard card, CardDetailHUD cardHud, Boolean subCard = false)
		{
			for (Int32 i = 0; i < 8; i++)
			{
				Boolean flag = ((Int32)card.arrow & (Int32)Mathf.Pow(2f, (Single)i)) != 0;
				flag = (!subCard && flag);
				cardHud.CardArrowList[i].SetActive(flag);
			}
			cardHud.CardImageSprite.spriteName = "card_" + card.id.ToString("0#");
			if (!subCard)
			{
				cardHud.AtkParamSprite.gameObject.SetActive(true);
				cardHud.PhysicDefParamSprite.gameObject.SetActive(true);
				cardHud.MagicDefParamSprite.gameObject.SetActive(true);
				cardHud.AtkTypeParamSprite.gameObject.SetActive(true);
				cardHud.AtkParamSprite.spriteName = "card_point_" + (card.atk >> 4).ToString("x");
				cardHud.PhysicDefParamSprite.spriteName = "card_point_" + (card.pdef >> 4).ToString("x");
				cardHud.MagicDefParamSprite.spriteName = "card_point_" + (card.mdef >> 4).ToString("x");
				switch (card.type)
				{
				case QuadMistCard.Type.PHYSICAL:
					cardHud.AtkTypeParamSprite.spriteName = "card_point_p";
					break;
				case QuadMistCard.Type.MAGIC:
					cardHud.AtkTypeParamSprite.spriteName = "card_point_m";
					break;
				case QuadMistCard.Type.FLEXIABLE:
					cardHud.AtkTypeParamSprite.spriteName = "card_point_x";
					break;
				case QuadMistCard.Type.ASSAULT:
					cardHud.AtkTypeParamSprite.spriteName = "card_point_a";
					break;
				}
			}
			else
			{
				cardHud.AtkParamSprite.gameObject.SetActive(false);
				cardHud.PhysicDefParamSprite.gameObject.SetActive(false);
				cardHud.MagicDefParamSprite.gameObject.SetActive(false);
				cardHud.AtkTypeParamSprite.gameObject.SetActive(false);
			}
		}

		public static void DisplayAPBar(PLAYER player, Int32 abilityId, Boolean isShowText, APBarHUD apBar)
		{
			Int32 num = ff9abil.FF9Abil_GetAp((Int32)player.info.slot_no, abilityId);
			Int32 num2 = ff9abil.FF9Abil_GetMax((Int32)player.info.slot_no, abilityId);
			if (num >= num2)
			{
				apBar.TextPanel.SetActive(false);
				apBar.APLable.text = num.ToString();
				apBar.APMaxLable.text = num2.ToString();
				apBar.ForegroundSprite.spriteName = "ap_bar_complete";
				apBar.MasterSprite.spriteName = "ap_bar_complete_star";
				apBar.Slider.value = 1f;
			}
			else
			{
				apBar.TextPanel.SetActive(isShowText);
				apBar.APLable.text = num.ToString();
				apBar.APMaxLable.text = num2.ToString();
				apBar.ForegroundSprite.spriteName = "ap_bar_progress";
				apBar.MasterSprite.spriteName = String.Empty;
				apBar.Slider.value = (Single)num / (Single)num2;
			}
		}

		public static void DisplayTextLocalize(GameObject go, String key)
		{
			go.GetComponent<UILocalize>().key = key;
			go.GetComponent<UILabel>().text = Localization.Get(key);
		}

		public static UIAtlas WindowAtlas
		{
			get
			{
				if (FF9StateSystem.Settings.cfg.win_type == 0UL)
				{
					if (FF9UIDataTool.grayAtlas == (UnityEngine.Object)null)
					{
						FF9UIDataTool.grayAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/Gray Atlas", typeof(UIAtlas)) as UIAtlas);
					}
					return FF9UIDataTool.grayAtlas;
				}
				if (FF9UIDataTool.blueAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.blueAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/Blue Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.blueAtlas;
			}
		}

		public static UIAtlas GrayAtlas
		{
			get
			{
				if (FF9UIDataTool.grayAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.grayAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/Gray Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.grayAtlas;
			}
		}

		public static UIAtlas BlueAtlas
		{
			get
			{
				if (FF9UIDataTool.blueAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.blueAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/Blue Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.blueAtlas;
			}
		}

		public static UIAtlas IconAtlas
		{
			get
			{
				if (FF9UIDataTool.iconAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.iconAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/Icon Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.iconAtlas;
			}
		}

		public static UIAtlas GeneralAtlas
		{
			get
			{
				if (FF9UIDataTool.generalAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.generalAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/General Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.generalAtlas;
			}
		}

		public static UIAtlas ScreenButtonAtlas
		{
			get
			{
				if (FF9UIDataTool.screenButtonAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.screenButtonAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/Screen Button Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.screenButtonAtlas;
			}
		}

		public static UIAtlas TutorialAtlas
		{
			get
			{
				if (FF9UIDataTool.tutorialAtlas == (UnityEngine.Object)null)
				{
					FF9UIDataTool.tutorialAtlas = (Resources.Load("EmbeddedAsset/UI/Atlas/TutorialUI Atlas", typeof(UIAtlas)) as UIAtlas);
				}
				return FF9UIDataTool.tutorialAtlas;
			}
		}

		public static GameObject IconGameObject(Int32 id)
		{
			GameObject result = (GameObject)null;
			String spriteName = String.Empty;
			if (id == FF9UIDataTool.NewIconId)
			{
				result = FF9UIDataTool.DrawButton(BitmapIconType.New);
			}
			else if (FF9UIDataTool.TutorialIconSpriteName.ContainsKey(id))
			{
				spriteName = FF9UIDataTool.TutorialIconSpriteName[id];
				result = FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.TutorialAtlas, spriteName);
			}
			else if (FF9UIDataTool.IconSpriteName.ContainsKey(id))
			{
				spriteName = FF9UIDataTool.IconSpriteName[id];
				result = FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.IconAtlas, spriteName);
			}
			return result;
		}

		public static Vector2 GetIconSize(Int32 id)
		{
			String spriteName = String.Empty;
			Vector2 spriteSize = new Vector2(0f, 0f);
			if (id == FF9UIDataTool.NewIconId)
			{
				spriteSize = new Vector2(115f, 64f);
			}
			else if (FF9UIDataTool.IconSpriteName.ContainsKey(id))
			{
				spriteName = FF9UIDataTool.IconSpriteName[id];
				spriteSize = FF9UIDataTool.GetSpriteSize(spriteName);
			}
			return spriteSize;
		}

		public static GameObject ButtonGameObject(Control key, Boolean checkFromConfig, String tag)
		{
			GameObject result = (GameObject)null;
			if (tag == NGUIText.JoyStickButtonIcon)
			{
				result = FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.IconAtlas, FF9UIDataTool.DialogButtonSpriteName(key, checkFromConfig, tag));
			}
			else if (PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect && tag != NGUIText.KeyboardButtonIcon)
			{
				result = FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.IconAtlas, FF9UIDataTool.DialogButtonSpriteName(key, checkFromConfig, tag));
			}
			else
			{
				switch (key)
				{
				case Control.Confirm:
				case Control.Cancel:
				case Control.Menu:
				case Control.Special:
				case Control.LeftBumper:
				case Control.RightBumper:
				case Control.LeftTrigger:
				case Control.RightTrigger:
				{
					KeyCode keycode;
					if (tag == NGUIText.KeyboardButtonIcon)
					{
						keycode = PersistenSingleton<HonoInputManager>.Instance.DefaultInputKeys[(Int32)key];
					}
					else if (checkFromConfig)
					{
						keycode = PersistenSingleton<HonoInputManager>.Instance.InputKeysPrimary[(Int32)key];
					}
					else
					{
						keycode = PersistenSingleton<HonoInputManager>.Instance.DefaultInputKeys[(Int32)key];
					}
					result = FF9UIDataTool.DrawButton(BitmapIconType.Keyboard, keycode);
					break;
				}
				case Control.Pause:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.IconAtlas, "keyboard_button_backspace");
					break;
				case Control.Select:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Keyboard, KeyCode.Alpha1);
					break;
				case Control.Up:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Keyboard, KeyCode.W);
					break;
				case Control.Down:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Keyboard, KeyCode.S);
					break;
				case Control.Left:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Keyboard, KeyCode.A);
					break;
				case Control.Right:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Keyboard, KeyCode.D);
					break;
				case Control.DPad:
					result = FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.IconAtlas, "ps_dpad");
					break;
				}
			}
			return result;
		}

		private static GameObject GetMobileButtonGameObject(Control key)
		{
			GameObject result = (GameObject)null;
			Int32 key2 = 0;
			Int32 num = EventEngineUtils.eventIDToMESID[(Int32)FF9StateSystem.Common.FF9.fldMapNo];
			if (num != 2)
			{
				if (num != 3)
				{
				}
			}
			else if (key == Control.Up)
			{
				key2 = 268;
			}
			FF9UIDataTool.DrawButton(BitmapIconType.Sprite, FF9UIDataTool.IconAtlas, FF9UIDataTool.IconSpriteName[key2]);
			return result;
		}

		public static Vector2 GetButtonSize(Control key, Boolean checkFromConfig, String tag)
		{
			String spriteName = String.Empty;
			if (tag == NGUIText.JoyStickButtonIcon)
			{
				spriteName = FF9UIDataTool.DialogButtonSpriteName(key, checkFromConfig, tag);
			}
			else if (PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect && tag != NGUIText.KeyboardButtonIcon)
			{
				spriteName = FF9UIDataTool.DialogButtonSpriteName(key, checkFromConfig, tag);
			}
			else
			{
				switch (key)
				{
				case Control.Confirm:
				case Control.Cancel:
				case Control.Menu:
				case Control.Special:
				case Control.LeftBumper:
				case Control.RightBumper:
				case Control.LeftTrigger:
				case Control.RightTrigger:
				case Control.Pause:
				case Control.Select:
				case Control.Up:
				case Control.Down:
				case Control.Left:
				case Control.Right:
					spriteName = "keyboard_button";
					break;
				case Control.DPad:
					spriteName = "ps_dpad";
					break;
				}
			}
            if (!checkFromConfig && (FF9StateSystem.PCPlatform || FF9StateSystem.AndroidPlatform))
            {
                bool flag = !global::GamePad.GetState(PlayerIndex.One).IsConnected;
                if (flag)
                {
                    if (key == Control.Pause)
                    {
                        spriteName = "keyboard_button_backspace";
                    }
                }
            }
            return FF9UIDataTool.GetSpriteSize(spriteName);
		}

		private static Vector2 GetSpriteSize(String spriteName)
		{
			UISpriteData sprite = FF9UIDataTool.IconAtlas.GetSprite(spriteName);
			Vector2 result;
			if (sprite == null)
			{
				result = new Vector2(64f, 64f);
			}
			else
			{
				Int32 num = sprite.width + sprite.paddingLeft + sprite.paddingRight;
				Int32 num2 = sprite.height + sprite.paddingTop + sprite.paddingBottom;
				result = new Vector2((Single)num, (Single)num2);
			}
			return result;
		}

		private static GameObject DrawButton(BitmapIconType bitmapIconType)
		{
			return FF9UIDataTool.GetControllerGameObject(bitmapIconType);
		}

		private static GameObject DrawButton(BitmapIconType bitmapIconType, UIAtlas atlas, String spriteName)
		{
			spriteName = FF9UIDataTool.CheckIconLocalize(spriteName);
			GameObject controllerGameObject = FF9UIDataTool.GetControllerGameObject(bitmapIconType);
			FF9UIDataTool.DrawSprite(controllerGameObject, atlas, spriteName);
			return controllerGameObject;
		}

		private static GameObject DrawButton(BitmapIconType bitmapIconType, KeyCode keycode)
		{
			GameObject controllerGameObject = FF9UIDataTool.GetControllerGameObject(bitmapIconType);
			FF9UIDataTool.DrawLabel(controllerGameObject.GetChild(0), keycode);
			return controllerGameObject;
		}

		private static GameObject GetControllerGameObject(BitmapIconType bitmapIconType)
		{
			GameObject gameObject = (GameObject)null;
			switch (bitmapIconType)
			{
			case BitmapIconType.Sprite:
				gameObject = FF9UIDataTool.GetGameObjectFromPool(FF9UIDataTool.bitmapSpritePool);
				if (gameObject == (UnityEngine.Object)null)
				{
					if (FF9UIDataTool.controllerSpritePrefab == (UnityEngine.Object)null)
					{
						FF9UIDataTool.controllerSpritePrefab = (Resources.Load("EmbeddedAsset/UI/Prefabs/Controller Sprite") as GameObject);
					}
					gameObject = UnityEngine.Object.Instantiate<GameObject>(FF9UIDataTool.controllerSpritePrefab);
					gameObject.tag = "BitmapSprite";
				}
				gameObject.SetActive(false);
				FF9UIDataTool.activeBitmapSpriteList.Push(gameObject);
				break;
			case BitmapIconType.Keyboard:
				gameObject = FF9UIDataTool.GetGameObjectFromPool(FF9UIDataTool.bitmapKeyboardPool);
				if (gameObject == (UnityEngine.Object)null)
				{
					if (FF9UIDataTool.controllerKeyboardPrefab == (UnityEngine.Object)null)
					{
						FF9UIDataTool.controllerKeyboardPrefab = (Resources.Load("EmbeddedAsset/UI/Prefabs/Controller Keyboard") as GameObject);
					}
					gameObject = UnityEngine.Object.Instantiate<GameObject>(FF9UIDataTool.controllerKeyboardPrefab);
					gameObject.tag = "BitmapKeyboard";
				}
				gameObject.SetActive(false);
				FF9UIDataTool.activeBitmapKeyboardList.Push(gameObject);
				break;
			case BitmapIconType.New:
				gameObject = FF9UIDataTool.GetGameObjectFromPool(FF9UIDataTool.bitmapNewIconPool);
				if (gameObject == (UnityEngine.Object)null)
				{
					if (FF9UIDataTool.newIconPrefab == (UnityEngine.Object)null)
					{
						FF9UIDataTool.newIconPrefab = (Resources.Load("EmbeddedAsset/UI/Prefabs/New Icon") as GameObject);
					}
					gameObject = UnityEngine.Object.Instantiate<GameObject>(FF9UIDataTool.newIconPrefab);
					gameObject.tag = "BitmapNewIcon";
				}
				gameObject.SetActive(false);
				FF9UIDataTool.activeBitmapNewIconList.Push(gameObject);
				break;
			}
			return gameObject;
		}

		private static GameObject GetGameObjectFromPool(List<GameObject> currentPool)
		{
			GameObject gameObject = (GameObject)null;
			if (currentPool.Count > 0)
			{
				gameObject = currentPool.Pop<GameObject>();
				gameObject.SetActive(false);
			}
			return gameObject;
		}

		private static String CheckIconLocalize(String spriteName)
		{
			String key = spriteName + "#" + FF9StateSystem.Settings.CurrentLanguage;
			if (FF9UIDataTool.iconLocalizeList.ContainsKey(key))
			{
				spriteName = FF9UIDataTool.iconLocalizeList[key];
			}
			return spriteName;
		}

		public static void ReleaseBitmapIconToPool(GameObject bitmap)
		{
			List<GameObject> theList;
			List<GameObject> list;
			FF9UIDataTool.GetCurrentPool(bitmap.tag, out theList, out list);
			bitmap.transform.parent = PersistenSingleton<UIManager>.Instance.transform;
			bitmap.SetActive(false);
			list.Remove(bitmap);
			theList.Push(bitmap);
		}

		private static void GetCurrentPool(String tag, out List<GameObject> currentPool, out List<GameObject> currentActivePool)
		{
			switch (tag)
			{
			case "BitmapKeyboard":
				currentPool = FF9UIDataTool.bitmapKeyboardPool;
				currentActivePool = FF9UIDataTool.activeBitmapKeyboardList;
				return;
			case "BitmapSprite":
				currentPool = FF9UIDataTool.bitmapSpritePool;
				currentActivePool = FF9UIDataTool.activeBitmapSpriteList;
				return;
			case "BitmapNewIcon":
				currentPool = FF9UIDataTool.bitmapNewIconPool;
				currentActivePool = FF9UIDataTool.activeBitmapNewIconList;
				return;
			}
			currentPool = null;
			currentActivePool = null;
		}

		private static void ReleaseAllBitmapIconsToPool(List<GameObject> currentPool, List<GameObject> currentActiveList)
		{
			foreach (GameObject gameObject in currentActiveList)
			{
				gameObject.transform.parent = PersistenSingleton<UIManager>.Instance.transform;
				gameObject.SetActive(false);
			}
			currentPool.AddRange(currentActiveList);
			currentActiveList.Clear();
		}

		public static void ReleaseAllTypeBitmapIconsToPool()
		{
			if (FF9UIDataTool.activeBitmapKeyboardList.Count != 0)
			{
				FF9UIDataTool.ReleaseAllBitmapIconsToPool(FF9UIDataTool.bitmapKeyboardPool, FF9UIDataTool.activeBitmapKeyboardList);
			}
			if (FF9UIDataTool.activeBitmapSpriteList.Count != 0)
			{
				FF9UIDataTool.ReleaseAllBitmapIconsToPool(FF9UIDataTool.bitmapSpritePool, FF9UIDataTool.activeBitmapSpriteList);
			}
			if (FF9UIDataTool.activeBitmapNewIconList.Count != 0)
			{
				FF9UIDataTool.ReleaseAllBitmapIconsToPool(FF9UIDataTool.bitmapNewIconPool, FF9UIDataTool.activeBitmapNewIconList);
			}
		}

		public static void ClearAllPool()
		{
			FF9UIDataTool.bitmapKeyboardPool.Clear();
			FF9UIDataTool.bitmapSpritePool.Clear();
			FF9UIDataTool.bitmapNewIconPool.Clear();
		}

		public static void DrawSprite(GameObject go, UIAtlas atlas, String spriteName)
		{
			UISprite component = go.GetComponent<UISprite>();
			component.atlas = atlas;
			component.spriteName = spriteName;
			component.MakePixelPerfect();
		}

		public static void DrawLabel(GameObject go, KeyCode keycode)
		{
			String text = String.Empty;
			if (FF9UIDataTool.KeyboardIconLabel.ContainsKey(keycode))
			{
				text = FF9UIDataTool.KeyboardIconLabel[keycode];
			}
			UILabel component = go.GetComponent<UILabel>();
			component.text = text;
			if (keycode >= KeyCode.Keypad0 && keycode <= KeyCode.KeypadPlus)
			{
				go.transform.localPosition = new Vector3(go.transform.localPosition.x, -37f, go.transform.localPosition.z);
				FF9UIDataTool.DrawSprite(go.GetParent(), FF9UIDataTool.IconAtlas, "keyboard_button_num");
			}
			else
			{
				go.transform.localPosition = new Vector3(go.transform.localPosition.x, -31f, go.transform.localPosition.z);
				FF9UIDataTool.DrawSprite(go.GetParent(), FF9UIDataTool.IconAtlas, "keyboard_button");
			}
		}

		public static String DialogButtonSpriteName(Control key, Boolean checkFromConfig, String tag)
		{
			String result = String.Empty;
			if (PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect || tag == NGUIText.JoyStickButtonIcon)
			{
				Dictionary<String, String> dictionary;
				if (Application.platform == RuntimePlatform.Android)
				{
					dictionary = FF9UIDataTool.buttonSpriteNameAndroidJoystick;
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					dictionary = FF9UIDataTool.buttonSpriteNameiOSJoystick;
				}
				else
				{
					dictionary = FF9UIDataTool.buttonSpriteNameJoystick;
				}
				switch (key)
				{
				case Control.Confirm:
				case Control.Cancel:
				case Control.Menu:
				case Control.Special:
				case Control.LeftBumper:
				case Control.RightBumper:
				case Control.LeftTrigger:
				case Control.RightTrigger:
				{
					String key2;
					if (checkFromConfig)
					{
						key2 = PersistenSingleton<HonoInputManager>.Instance.JoystickKeysPrimary[(Int32)key];
					}
					else
					{
						key2 = PersistenSingleton<HonoInputManager>.Instance.DefaultJoystickInputKeys[(Int32)key];
					}
					if (dictionary.ContainsKey(key2))
					{
						result = dictionary[key2];
					}
					break;
				}
				case Control.Pause:
					result = FF9UIDataTool.buttonSpriteNameJoystick["JoystickButton6"];
					break;
				case Control.Select:
					result = FF9UIDataTool.buttonSpriteNameJoystick["JoystickButton7"];
					break;
				case Control.Up:
					result = dictionary["Up"];
					break;
				case Control.Down:
					result = dictionary["Down"];
					break;
				case Control.Left:
					result = dictionary["Left"];
					break;
				case Control.Right:
					result = dictionary["Right"];
					break;
				case Control.DPad:
					result = dictionary["DPad"];
					break;
				}
			}
			return result;
		}

		public static String GetJoystickSpriteByName(String key)
		{
			String result = String.Empty;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (FF9UIDataTool.buttonSpriteNameiOSJoystick.ContainsKey(key))
				{
					result = FF9UIDataTool.buttonSpriteNameiOSJoystick[key];
				}
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				if (FF9UIDataTool.buttonSpriteNameAndroidJoystick.ContainsKey(key))
				{
					result = FF9UIDataTool.buttonSpriteNameAndroidJoystick[key];
				}
			}
			else if (FF9UIDataTool.buttonSpriteNameJoystick.ContainsKey(key))
			{
				result = FF9UIDataTool.buttonSpriteNameJoystick[key];
			}
			return result;
		}

		public static String AvatarSpriteName(Byte serialNo)
		{
			switch (serialNo)
			{
			case 0:
			case 1:
				return "face00";
			case 2:
				return "face01";
			case 3:
			case 4:
				return "face02";
			case 5:
			case 6:
				return "face03";
			case 7:
			case 8:
				return "face04";
			case 9:
				return "face05";
			case 10:
			case 11:
				return "face06";
			case 12:
				return "face07";
			case 13:
				return "face08";
			case 14:
				return "face09";
			case 15:
				return "face10";
			case 16:
			case 17:
				return "face11";
			case 18:
				return "face12";
			default:
				return String.Empty;
			}
		}

		public static Sprite LoadWorldTitle(SByte titleId, Boolean isShadow)
		{
			Sprite sprite = (Sprite)null;
			String text;
			if (FF9StateSystem.Settings.CurrentLanguage == "English(UK)")
			{
				text = "US";
			}
			else
			{
				text = Localization.GetSymbol();
			}
			String text2;
			if ((Int32)titleId == (Int32)FF9UIDataTool.WorldTitleMistContinent)
			{
				text2 = "title_world_mist";
			}
			else if ((Int32)titleId == (Int32)FF9UIDataTool.WorldTitleOuterContinent)
			{
				text2 = "title_world_outer";
			}
			else if ((Int32)titleId == (Int32)FF9UIDataTool.WorldTitleForgottenContinent)
			{
				text2 = "title_world_forgotten";
			}
			else
			{
				if ((Int32)titleId != (Int32)FF9UIDataTool.WorldTitleLostContinent)
				{
					global::Debug.LogError("World Continent Title: Could not found resource from titleId:" + titleId);
					return sprite;
				}
				text2 = "title_world_lost";
			}
			text2 += ((!isShadow) ? ("_" + text.ToLower()) : ("_shadow_" + text.ToLower()));
			if (FF9UIDataTool.worldTitleSpriteList.ContainsKey(text2))
			{
				sprite = FF9UIDataTool.worldTitleSpriteList[text2];
			}
			else
			{
				String path = "EmbeddedAsset/UI/Sprites/" + text + "/" + text2;
				sprite = Resources.Load<Sprite>(path);
				FF9UIDataTool.worldTitleSpriteList.Add(text2, sprite);
			}
			return sprite;
		}

		public static readonly Int32 NewIconId = 400;

		private static UIAtlas generalAtlas;

		private static UIAtlas iconAtlas;

		private static UIAtlas grayAtlas;

		private static UIAtlas blueAtlas;

		private static UIAtlas screenButtonAtlas;

		private static UIAtlas tutorialAtlas;

		private static GameObject controllerSpritePrefab = (GameObject)null;

		private static GameObject controllerKeyboardPrefab = (GameObject)null;

		private static GameObject newIconPrefab = (GameObject)null;

		private static List<GameObject> bitmapKeyboardPool = new List<GameObject>();

		private static List<GameObject> bitmapSpritePool = new List<GameObject>();

		private static List<GameObject> bitmapNewIconPool = new List<GameObject>();

		private static List<GameObject> activeBitmapKeyboardList = new List<GameObject>();

		private static List<GameObject> activeBitmapSpriteList = new List<GameObject>();

		private static List<GameObject> activeBitmapNewIconList = new List<GameObject>();

		private static Int32[] status_id = new Int32[]
		{
			154,
			153,
			152,
			151,
			150,
			149,
			148
		};

		private static Dictionary<String, String> buttonSpriteNameiOSJoystick = new Dictionary<String, String>
		{
			{
				"JoystickButton14",
				"joystick_button_a"
			},
			{
				"JoystickButton13",
				"joystick_button_b"
			},
			{
				"JoystickButton15",
				"joystick_button_x"
			},
			{
				"JoystickButton12",
				"joystick_button_y"
			},
			{
				"JoystickButton8",
				"joystick_l1"
			},
			{
				"JoystickButton9",
				"joystick_r1"
			},
			{
				"JoystickButton10",
				"joystick_l2"
			},
			{
				"JoystickButton11",
				"joystick_r2"
			},
			{
				"JoystickButton0",
				"joystick_start"
			},
			{
				"Empty",
				"joystick_analog_r"
			},
			{
				"Up",
				"ps_dpad_up"
			},
			{
				"Down",
				"ps_dpad_down"
			},
			{
				"Left",
				"ps_dpad_left"
			},
			{
				"Right",
				"ps_dpad_right"
			},
			{
				"DPad",
				"ps_dpad"
			}
		};

		private static Dictionary<String, String> buttonSpriteNameAndroidJoystick = new Dictionary<String, String>
		{
			{
				"JoystickButton0",
				"joystick_button_a"
			},
			{
				"JoystickButton1",
				"joystick_button_b"
			},
			{
				"JoystickButton2",
				"joystick_button_x"
			},
			{
				"JoystickButton3",
				"joystick_button_y"
			},
			{
				"JoystickButton4",
				"joystick_l1"
			},
			{
				"JoystickButton5",
				"joystick_r1"
			},
			{
				"LeftTrigger Android",
				"joystick_l2"
			},
			{
				"RightTrigger Android",
				"joystick_r2"
			},
			{
				"JoystickButton10",
				"joystick_start"
			},
			{
				"Empty",
				"joystick_analog_r"
			},
			{
				"Up",
				"ps_dpad_up"
			},
			{
				"Down",
				"ps_dpad_down"
			},
			{
				"Left",
				"ps_dpad_left"
			},
			{
				"Right",
				"ps_dpad_right"
			},
			{
				"DPad",
				"ps_dpad"
			}
		};

		private static Dictionary<String, String> buttonSpriteNameJoystick = new Dictionary<String, String>
		{
			{
				"JoystickButton0",
				"joystick_button_a"
			},
			{
				"JoystickButton1",
				"joystick_button_b"
			},
			{
				"JoystickButton2",
				"joystick_button_x"
			},
			{
				"JoystickButton3",
				"joystick_button_y"
			},
			{
				"JoystickButton4",
				"joystick_l1"
			},
			{
				"JoystickButton5",
				"joystick_r1"
			},
			{
				"LeftTrigger",
				"joystick_l2"
			},
			{
				"RightTrigger",
				"joystick_r2"
			},
			{
				"JoystickButton6",
				"joystick_start"
			},
			{
				"JoystickButton7",
				"joystick_select"
			},
			{
				"Up",
				"ps_dpad_up"
			},
			{
				"Down",
				"ps_dpad_down"
			},
			{
				"Left",
				"ps_dpad_left"
			},
			{
				"Right",
				"ps_dpad_right"
			},
			{
				"DPad",
				"ps_dpad"
			}
		};

		public static readonly Dictionary<Int32, String> IconSpriteName = new Dictionary<Int32, String>
		{
			{
				1,
				"cursor_hand_choice"
			},
			{
				19,
				"arrow_up"
			},
			{
				20,
				"arrow_down"
			},
			{
				27,
				"help_mog_dialog"
			},
			{
				28,
				"help_mog_hand_1"
			},
			{
				29,
				"help_mog_hand_2"
			},
			{
				92,
				"item00_00"
			},
			{
				93,
				"item01_00"
			},
			{
				94,
				"item02_00"
			},
			{
				95,
				"item03_00"
			},
			{
				96,
				"item04_00"
			},
			{
				97,
				"item05_00"
			},
			{
				98,
				"item06_00"
			},
			{
				99,
				"item07_00"
			},
			{
				100,
				"item08_00"
			},
			{
				101,
				"item09_00"
			},
			{
				102,
				"item10_00"
			},
			{
				103,
				"item11_00"
			},
			{
				104,
				"item12_00"
			},
			{
				105,
				"item13_01"
			},
			{
				106,
				"item14_02"
			},
			{
				107,
				"item15_01"
			},
			{
				108,
				"item16_01"
			},
			{
				109,
				"item17_01"
			},
			{
				110,
				"item18_00"
			},
			{
				111,
				"item19_02"
			},
			{
				112,
				"item20_01"
			},
			{
				113,
				"item21_00"
			},
			{
				114,
				"item22_02"
			},
			{
				115,
				"item23_00"
			},
			{
				116,
				"item24_00"
			},
			{
				117,
				"item25_03"
			},
			{
				118,
				"item26_09"
			},
			{
				119,
				"item27_01"
			},
			{
				120,
				"item28_03"
			},
			{
				121,
				"item29_01"
			},
			{
				122,
				"item30_00"
			},
			{
				123,
				"item31_08"
			},
			{
				124,
				"item32_02"
			},
			{
				125,
				"item33_01"
			},
			{
				126,
				"item34_02"
			},
			{
				127,
				"item35_03"
			},
			{
				131,
				"icon_status_22"
			},
			{
				132,
				"icon_status_23"
			},
			{
				133,
				"icon_status_11"
			},
			{
				134,
				"icon_status_00"
			},
			{
				135,
				"icon_status_01"
			},
			{
				136,
				"icon_status_02"
			},
			{
				137,
				"icon_status_03"
			},
			{
				138,
				"icon_status_04"
			},
			{
				139,
				"icon_status_05"
			},
			{
				140,
				"icon_status_06"
			},
			{
				141,
				"icon_status_07"
			},
			{
				142,
				"icon_status_08"
			},
			{
				143,
				"icon_status_09"
			},
			{
				144,
				"icon_status_10"
			},
			{
				145,
				"icon_status_12"
			},
			{
				146,
				"icon_status_13"
			},
			{
				147,
				"icon_status_14"
			},
			{
				148,
				"icon_status_15"
			},
			{
				149,
				"icon_status_16"
			},
			{
				150,
				"icon_status_17"
			},
			{
				151,
				"icon_status_18"
			},
			{
				152,
				"icon_status_19"
			},
			{
				153,
				"icon_status_20"
			},
			{
				154,
				"icon_status_21"
			},
			{
				180,
				"text_lv_us_uk_jp_gr_it"
			},
			{
				188,
				"ability_stone"
			},
			{
				189,
				"skill_stone_on"
			},
			{
				190,
				"ability_stone_null"
			},
			{
				191,
				"skill_stone_null"
			},
			{
				192,
				"ap_bar_complete_star"
			},
			{
				193,
				"skill_stone_gem"
			},
			{
				244,
				"skill_stone_off"
			},
			{
				254,
				"ap_bar_full"
			},
			{
				255,
				"ap_bar_half"
			},
			{
				257,
				"balloon_exclamation"
			},
			{
				258,
				"balloon_question"
			},
			{
				259,
				"balloon_card"
			},
			{
				260,
				"icon_action"
			},
			{
				261,
				"icon_back"
			},
			{
				262,
				"icon_analog"
			},
			{
				263,
				"virtual_up"
			},
			{
				264,
				"virtual_down"
			},
			{
				265,
				"virtual_left"
			},
			{
				266,
				"virtual_right"
			},
			{
				267,
				"icon_left"
			},
			{
				268,
				"icon_up"
			},
			{
				269,
				"icon_right"
			},
			{
				270,
				"icon_down"
			},
			{
				271,
				"icon_x"
			},
			{
				272,
				"icon_y"
			},
			{
				273,
				"icon_b"
			},
			{
				274,
				"icon_a"
			},
			{
				275,
				"icon_minus"
			},
			{
				276,
				"icon_plus"
			},
			{
				277,
				"icon_racing_01"
			},
			{
				278,
				"icon_racing_02"
			},
			{
				279,
				"icon_help"
			},
			{
				280,
				"virtual_aside"
			},
			{
				281,
				"icon_world_map"
			},
			{
				282,
				"icon_world_mog"
			},
			{
				283,
				"icon_world_map"
			},
			{
				284,
				"icon_cam_rotate"
			},
			{
				285,
				"icon_cam_perspective"
			},
			{
				286,
				"icon_world_dismount"
			},
			{
				287,
				"icon_cam_align"
			},
			{
				288,
				"icon_menu_jp"
			},
			{
				289,
				"icon_menu_us"
			},
			{
				290,
				"icon_menu_fr"
			},
			{
				291,
				"icon_menu_es"
			},
			{
				292,
				"icon_menu_gr"
			},
			{
				293,
				"icon_menu_it"
			},
			{
				294,
				"icon_menu_uk"
			},
			{
				295,
				"icon_deck_jp"
			},
			{
				296,
				"icon_deck_us"
			},
			{
				297,
				"icon_deck_fr"
			},
			{
				298,
				"icon_deck_es"
			},
			{
				299,
				"icon_deck_gr"
			},
			{
				300,
				"icon_deck_it"
			},
			{
				301,
				"icon_deck_uk"
			},
			{
				302,
				"icon_battle_run"
			},
			{
				303,
				"icon_battle_all"
			},
			{
				304,
				"icon_pause"
			},
			{
				305,
				"icon_bubble_question"
			},
			{
				306,
				"text_touch_us_uk_fr_gr_it_es"
			},
			{
				307,
				"text_touchconfirm_us_uk_fr_gr_it_es"
			},
			{
				308,
				"text_touchscreen_us_uk_fr_gr_it_es"
			},
			{
				309,
				"text_characterpanel_us_uk_fr_gr_it_es"
			},
			{
				310,
				"icon_bubble_card"
			},
			{
				311,
				"icon_chocobo_dig"
			},
			{
				312,
				"icon_bubble_question"
			},
			{
				313,
				"icon_ate_us_uk_jp"
			},
			{
				314,
				"icon_ate_es_fr"
			},
			{
				315,
				"icon_ate_gr_it"
			},
			{
				316,
				"text_touch_jp"
			},
			{
				317,
				"text_touch_us_uk"
			},
			{
				318,
				"text_touch_fr"
			},
			{
				319,
				"text_touch_es"
			},
			{
				320,
				"text_touch_gr"
			},
			{
				321,
				"text_touch_it"
			},
			{
				322,
				"text_touchconfirm_jp"
			},
			{
				323,
				"text_touchconfirm_us_uk"
			},
			{
				324,
				"text_touchconfirm_fr"
			},
			{
				325,
				"text_touchconfirm_es"
			},
			{
				326,
				"text_touchconfirm_gr"
			},
			{
				327,
				"text_touchconfirm_it"
			},
			{
				328,
				"text_touchscreen_jp"
			},
			{
				329,
				"text_touchscreen_us_uk"
			},
			{
				330,
				"text_touchscreen_fr"
			},
			{
				331,
				"text_touchscreen_es"
			},
			{
				332,
				"text_touchscreen_gr"
			},
			{
				333,
				"text_touchscreen_it"
			},
			{
				334,
				"text_characterpanel_jp"
			},
			{
				335,
				"text_characterpanel_us_uk"
			},
			{
				336,
				"text_characterpanel_fr"
			},
			{
				337,
				"text_characterpanel_es"
			},
			{
				338,
				"text_characterpanel_gr"
			},
			{
				339,
				"text_characterpanel_it"
			},
			{
				340,
				"icon_ate_us_uk_jp"
			},
			{
				341,
				"icon_ate_es_fr"
			},
			{
				342,
				"icon_ate_gr_it"
			},
			{
				343,
				"icon_beach"
			},
			{
				512,
				"item01_06"
			},
			{
				513,
				"item01_12"
			},
			{
				514,
				"item03_03"
			},
			{
				515,
				"item03_05"
			},
			{
				516,
				"item03_09"
			},
			{
				517,
				"item03_11"
			},
			{
				518,
				"item03_13"
			},
			{
				519,
				"item04_03"
			},
			{
				520,
				"item05_03"
			},
			{
				521,
				"item05_05"
			},
			{
				522,
				"item05_06"
			},
			{
				523,
				"item05_12"
			},
			{
				524,
				"item06_03"
			},
			{
				525,
				"item06_06"
			},
			{
				526,
				"item06_13"
			},
			{
				527,
				"item08_03"
			},
			{
				528,
				"item08_04"
			},
			{
				529,
				"item08_12"
			},
			{
				530,
				"item09_02"
			},
			{
				531,
				"item09_03"
			},
			{
				532,
				"item09_07"
			},
			{
				533,
				"item10_02"
			},
			{
				534,
				"item10_05"
			},
			{
				535,
				"item10_09"
			},
			{
				536,
				"item10_11"
			},
			{
				537,
				"item11_02"
			},
			{
				538,
				"item11_04"
			},
			{
				539,
				"item13_02"
			},
			{
				540,
				"item13_03"
			},
			{
				541,
				"item13_07"
			},
			{
				542,
				"item13_14"
			},
			{
				543,
				"item14_08"
			},
			{
				544,
				"item14_09"
			},
			{
				545,
				"item14_11"
			},
			{
				546,
				"item15_02"
			},
			{
				547,
				"item15_04"
			},
			{
				548,
				"item15_07"
			},
			{
				549,
				"item15_08"
			},
			{
				550,
				"item15_09"
			},
			{
				551,
				"item15_12"
			},
			{
				552,
				"item16_03"
			},
			{
				553,
				"item16_08"
			},
			{
				554,
				"item16_09"
			},
			{
				555,
				"item16_12"
			},
			{
				556,
				"item17_02"
			},
			{
				557,
				"item17_03"
			},
			{
				558,
				"item17_08"
			},
			{
				559,
				"item17_14"
			},
			{
				560,
				"item18_06"
			},
			{
				561,
				"item18_08"
			},
			{
				562,
				"item18_09"
			},
			{
				563,
				"item18_10"
			},
			{
				564,
				"item18_11"
			},
			{
				565,
				"item18_14"
			},
			{
				566,
				"item19_03"
			},
			{
				567,
				"item19_06"
			},
			{
				568,
				"item19_08"
			},
			{
				569,
				"item19_11"
			},
			{
				570,
				"item19_12"
			},
			{
				571,
				"item20_02"
			},
			{
				572,
				"item20_03"
			},
			{
				573,
				"item20_06"
			},
			{
				574,
				"item20_10"
			},
			{
				575,
				"item20_11"
			},
			{
				576,
				"item20_14"
			},
			{
				577,
				"item21_01"
			},
			{
				578,
				"item21_02"
			},
			{
				579,
				"item21_03"
			},
			{
				580,
				"item21_04"
			},
			{
				581,
				"item21_05"
			},
			{
				582,
				"item21_06"
			},
			{
				583,
				"item21_07"
			},
			{
				584,
				"item21_08"
			},
			{
				585,
				"item21_10"
			},
			{
				586,
				"item21_12"
			},
			{
				587,
				"item21_13"
			},
			{
				588,
				"item21_14"
			},
			{
				589,
				"item22_06"
			},
			{
				590,
				"item22_08"
			},
			{
				591,
				"item22_09"
			},
			{
				592,
				"item23_03"
			},
			{
				593,
				"item23_05"
			},
			{
				594,
				"item23_06"
			},
			{
				595,
				"item23_07"
			},
			{
				596,
				"item23_09"
			},
			{
				597,
				"item23_10"
			},
			{
				598,
				"item24_02"
			},
			{
				599,
				"item24_03"
			},
			{
				600,
				"item24_04"
			},
			{
				601,
				"item24_06"
			},
			{
				602,
				"item24_08"
			},
			{
				603,
				"item24_10"
			},
			{
				604,
				"item24_11"
			},
			{
				605,
				"item25_09"
			},
			{
				606,
				"item27_09"
			},
			{
				607,
				"item28_08"
			},
			{
				608,
				"item29_02"
			},
			{
				609,
				"item29_03"
			},
			{
				610,
				"item29_04"
			},
			{
				611,
				"item29_08"
			},
			{
				612,
				"item29_10"
			},
			{
				613,
				"item29_12"
			},
			{
				614,
				"item29_14"
			},
			{
				615,
				"item31_09"
			},
			{
				616,
				"item31_14"
			},
			{
				617,
				"item32_03"
			},
			{
				618,
				"item33_02"
			},
			{
				619,
				"item33_03"
			},
			{
				620,
				"item33_04"
			},
			{
				621,
				"item33_05"
			},
			{
				622,
				"item33_11"
			},
			{
				623,
				"item33_14"
			},
			{
				624,
				"item35_12"
			},
			{
				625,
				"icon_equip_0"
			},
			{
				626,
				"icon_equip_1"
			},
			{
				627,
				"icon_equip_2"
			},
			{
				628,
				"icon_equip_3"
			},
			{
				629,
				"icon_equip_4"
			},
			{
				630,
				"keyboard_button_enter"
			},
			{
				631,
				"keyboard_button_esc"
			},
			{
				632,
				"keyboard_button_arrow_up"
			},
			{
				633,
				"keyboard_button_arrow_left"
			},
			{
				634,
				"keyboard_button_arrow_right"
			},
			{
				635,
				"keyboard_button_arrow_down"
			},
			{
				636,
				"text_lv_es"
			},
			{
				637,
				"text_lv_fr"
			},
			{
				638,
				"joystick_start"
			},
			{
				639,
				"joystick_l2"
			},
			{
				640,
				"joystick_r2"
			},
			{
				641,
				"joystick_button_y"
			}
		};

		public static readonly Dictionary<Int32, String> TutorialIconSpriteName = new Dictionary<Int32, String>
		{
			{
				769,
				"tutorial_quadmist_2"
			},
			{
				770,
				"tutorial_quadmist_3"
			}
		};

		public static readonly Dictionary<KeyCode, String> KeyboardIconLabel = new Dictionary<KeyCode, String>
		{
			{
				KeyCode.Exclaim,
				"!"
			},
			{
				KeyCode.DoubleQuote,
				"\""
			},
			{
				KeyCode.Hash,
				"#"
			},
			{
				KeyCode.Dollar,
				"$"
			},
			{
				KeyCode.Ampersand,
				"&"
			},
			{
				KeyCode.Quote,
				"'"
			},
			{
				KeyCode.LeftParen,
				"("
			},
			{
				KeyCode.RightParen,
				")"
			},
			{
				KeyCode.Asterisk,
				"*"
			},
			{
				KeyCode.Plus,
				"+"
			},
			{
				KeyCode.Comma,
				","
			},
			{
				KeyCode.Minus,
				"-"
			},
			{
				KeyCode.Period,
				"."
			},
			{
				KeyCode.Slash,
				"/"
			},
			{
				KeyCode.Alpha0,
				"0"
			},
			{
				KeyCode.Alpha1,
				"1"
			},
			{
				KeyCode.Alpha2,
				"2"
			},
			{
				KeyCode.Alpha3,
				"3"
			},
			{
				KeyCode.Alpha4,
				"4"
			},
			{
				KeyCode.Alpha5,
				"5"
			},
			{
				KeyCode.Alpha6,
				"6"
			},
			{
				KeyCode.Alpha7,
				"7"
			},
			{
				KeyCode.Alpha8,
				"8"
			},
			{
				KeyCode.Alpha9,
				"9"
			},
			{
				KeyCode.Colon,
				":"
			},
			{
				KeyCode.Semicolon,
				";"
			},
			{
				KeyCode.Less,
				"<"
			},
			{
				KeyCode.Equals,
				"="
			},
			{
				KeyCode.Greater,
				">"
			},
			{
				KeyCode.Question,
				"?"
			},
			{
				KeyCode.At,
				"@"
			},
			{
				KeyCode.LeftBracket,
				"["
			},
			{
				KeyCode.Backslash,
				"\\"
			},
			{
				KeyCode.RightBracket,
				"]"
			},
			{
				KeyCode.Caret,
				"^"
			},
			{
				KeyCode.Underscore,
				"_"
			},
			{
				KeyCode.BackQuote,
				"`"
			},
			{
				KeyCode.A,
				"A"
			},
			{
				KeyCode.B,
				"B"
			},
			{
				KeyCode.C,
				"C"
			},
			{
				KeyCode.D,
				"D"
			},
			{
				KeyCode.E,
				"E"
			},
			{
				KeyCode.F,
				"F"
			},
			{
				KeyCode.G,
				"G"
			},
			{
				KeyCode.H,
				"H"
			},
			{
				KeyCode.I,
				"I"
			},
			{
				KeyCode.J,
				"J"
			},
			{
				KeyCode.K,
				"K"
			},
			{
				KeyCode.L,
				"L"
			},
			{
				KeyCode.M,
				"M"
			},
			{
				KeyCode.N,
				"N"
			},
			{
				KeyCode.O,
				"O"
			},
			{
				KeyCode.P,
				"P"
			},
			{
				KeyCode.Q,
				"Q"
			},
			{
				KeyCode.R,
				"R"
			},
			{
				KeyCode.S,
				"S"
			},
			{
				KeyCode.T,
				"T"
			},
			{
				KeyCode.U,
				"U"
			},
			{
				KeyCode.V,
				"V"
			},
			{
				KeyCode.W,
				"W"
			},
			{
				KeyCode.X,
				"X"
			},
			{
				KeyCode.Y,
				"Y"
			},
			{
				KeyCode.Z,
				"Z"
			},
			{
				KeyCode.Keypad0,
				"0"
			},
			{
				KeyCode.Keypad1,
				"1"
			},
			{
				KeyCode.Keypad2,
				"2"
			},
			{
				KeyCode.Keypad3,
				"3"
			},
			{
				KeyCode.Keypad4,
				"4"
			},
			{
				KeyCode.Keypad5,
				"5"
			},
			{
				KeyCode.Keypad6,
				"6"
			},
			{
				KeyCode.Keypad7,
				"7"
			},
			{
				KeyCode.Keypad8,
				"8"
			},
			{
				KeyCode.Keypad9,
				"9"
			},
			{
				KeyCode.KeypadDivide,
				"/"
			},
			{
				KeyCode.KeypadPeriod,
				"."
			},
			{
				KeyCode.KeypadMultiply,
				"*"
			},
			{
				KeyCode.KeypadPlus,
				"+"
			},
			{
				KeyCode.KeypadMinus,
				"-"
			},
			{
				KeyCode.F1,
				"F1"
			},
			{
				KeyCode.F2,
				"F2"
			},
			{
				KeyCode.F3,
				"F3"
			},
			{
				KeyCode.F4,
				"F4"
			},
			{
				KeyCode.F5,
				"F5"
			},
			{
				KeyCode.F6,
				"F6"
			},
			{
				KeyCode.F7,
				"F7"
			},
			{
				KeyCode.F8,
				"F8"
			},
			{
				KeyCode.F9,
				"F9"
			},
			{
				KeyCode.F10,
				"F10"
			},
			{
				KeyCode.F11,
				"F11"
			},
			{
				KeyCode.F12,
				"F12"
			}
		};

		private static readonly SByte WorldTitleMistContinent = 0;

		private static readonly SByte WorldTitleOuterContinent = 1;

		private static readonly SByte WorldTitleForgottenContinent = 2;

		private static readonly SByte WorldTitleLostContinent = 3;

		private static Dictionary<String, Sprite> worldTitleSpriteList = new Dictionary<String, Sprite>();

		private static readonly Dictionary<String, String> iconLocalizeList = new Dictionary<String, String>
		{
			{
				"keyboard_button_enter#French",
				"keyboard_button_enter_fr_gr"
			},
			{
				"keyboard_button_enter#German",
				"keyboard_button_enter_fr_gr"
			},
			{
				"keyboard_button_enter#Italian",
				"keyboard_button_enter_it"
			},
			{
				"keyboard_button_backspace#French",
				"keyboard_button_backspace_fr_gr_it"
			},
			{
				"keyboard_button_backspace#German",
				"keyboard_button_backspace_fr_gr_it"
			},
			{
				"keyboard_button_backspace#Italian",
				"keyboard_button_backspace_fr_gr_it"
			}
		};
	}
}
