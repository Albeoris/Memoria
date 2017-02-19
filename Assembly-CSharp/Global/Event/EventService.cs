using System;

public class EventService
{
	private static void FF9Menu_Command(UInt32 menuId, UInt32 subId)
	{
		switch (menuId)
		{
		case 0u:
			EventService.OpenMainMenu();
			break;
		case 1u:
			EventService.OpenNameMenu(Convert.ToInt32(subId));
			break;
		case 2u:
			EventService.OpenShopMenu(Convert.ToInt32(subId));
			break;
		case 4u:
			if (subId == 0u)
			{
				EventService.OpenSaveMenu();
			}
			break;
		case 5u:
			EventService.OpenChocoGraph(Convert.ToInt32(subId));
			break;
		}
	}

	private static void OpenSaveMenu()
	{
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			PersistenSingleton<UIManager>.Instance.SaveLoadScene.Type = SaveLoadUI.SerializeType.Save;
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Serialize);
		});
	}

	private static void OpenShopMenu(Int32 shopId)
	{
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			PersistenSingleton<UIManager>.Instance.ShopScene.Id = shopId;
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Shop);
		});
	}

	private static void OpenNameMenu(Int32 playerId)
	{
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			PersistenSingleton<UIManager>.Instance.NameSettingScene.SubNo = playerId;
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.NameSetting);
		});
	}

	private static void OpenChocoGraph(Int32 param)
	{
		Singleton<DialogManager>.Instance.CloseAll();
		EIcon.IsProcessingFIcon = false;
		Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			EIcon.InitFIcon();
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Chocograph);
		});
	}

	public static void OpenPartyMenu(FF9PARTY_INFO sPartyInfo)
	{
		Singleton<DialogManager>.Instance.CloseAll();
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			PersistenSingleton<UIManager>.Instance.PartySettingScene.Info = sPartyInfo;
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.PartySetting);
		});
	}

	public static void OpenGameOver()
	{
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = true;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.GameOver);
		});
	}

	public static void StartMenu(UInt32 menuId, UInt32 subId)
	{
		EventService.FF9Menu_Command(menuId, subId);
		PersistenSingleton<UIManager>.Instance.HideAllHUD();
		PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
	}

	public static void SetMiniGame(UInt16 Arg)
	{
		FF9StateSystem.Common.FF9.miniGameArg = Arg;
	}

	private static void OpenMainMenu()
	{
		EIcon.IsProcessingFIcon = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).NextSceneIsModal = false;
		PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Hide(delegate
		{
			EIcon.IsProcessingFIcon = true;
			PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
		});
	}

	public static void OpenBasicControlTutorial()
	{
		PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
		TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
		tutorialScene.DisplayMode = TutorialUI.Mode.BasicControl;
		tutorialScene.BasicControlTutorialID = 0;
		tutorialScene.AfterFinished = new UIScene.SceneVoidDelegate(EventService.OnFinishTutorialBasicControl);
		PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
	}

	private static void OnFinishTutorialBasicControl()
	{
		PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
	}

	public enum MenuId
	{
		FF9MENU_ID_FIELD,
		FF9MENU_ID_NAME,
		FF9MENU_ID_SHOP,
		FF9MENU_ID_PARTY,
		FF9MENU_ID_MCARD,
		FF9MENU_ID_CHOCOBO,
		FF9MENU_ID_DEBUG,
		FF9MENU_ID_BEND,
		FF9MENU_ID_CDCHG,
		FF9MENU_ID_MAX
	}
}
