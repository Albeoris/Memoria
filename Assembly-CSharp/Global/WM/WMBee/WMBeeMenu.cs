using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using System;
using UnityEngine;

public class WMBeeMenu : MonoBehaviour
{
    public Boolean ShowOnlyBackButton { get; set; }

    protected void Awake()
    {
        this.ShowMenu = true;
        if (FF9StateSystem.World.IsBeeScene)
        {
            this.showRightMenu = true;
            this.showChangeCharMenu = true;
            this.showChocoboMenu = false;
            this.showBackMenu = true;
            this.showBeeMessage = false;
        }
        else
        {
            this.showRightMenu = false;
            this.showChangeCharMenu = false;
            this.showChocoboMenu = false;
            this.showBackMenu = false;
            this.showBeeMessage = false;
        }
        if (ff9.w_frameDisc == 1)
        {
            this.showSetPositionMenu = true;
            this.showWorldMapStateMenu = false;
            this.showSetPositionMenu = false;
        }
        else
        {
            this.showSetPositionMenu = false;
        }
    }

    private void OnAutoLayout()
    {
        Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
        DebugGuiSkin.ApplySkin();
        GUILayout.BeginArea(fullscreenRect);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.BeginVertical(new GUILayoutOption[0]);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.BeginVertical(new GUILayoutOption[0]);
        this.DrawLeftMenu();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(new GUILayoutOption[0]);
        if (this.showWorldStateMenu)
        {
            Single width = fullscreenRect.width * 2.5f / 5f;
            Single height = fullscreenRect.height;
            if (this.showWorldMapStateMenu)
            {
                this.showWorldMapStateMenuScrollPosition = GUILayout.BeginScrollView(this.showWorldMapStateMenuScrollPosition, new GUILayoutOption[]
                {
                    GUILayout.Width(width),
                    GUILayout.Height(height)
                });
                this.DrawWorldMapStateMenu();
                GUILayout.EndScrollView();
            }
            if (this.showSetPositionMenu)
            {
                this.showWorldMapStateMenuScrollPosition = GUILayout.BeginScrollView(this.showWorldMapStateMenuScrollPosition, new GUILayoutOption[]
                {
                    GUILayout.Width(width),
                    GUILayout.Height(height)
                });
                this.DrawSetPositionMenu();
                GUILayout.EndScrollView();
            }
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical(new GUILayoutOption[0]);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        if (this.showRightMenu && GUILayout.Button("World State", new GUILayoutOption[0]))
        {
            this.showWorldStateMenu = !this.showWorldStateMenu;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.BeginVertical(new GUILayoutOption[0]);
        if (this.showWorldStateMenu)
        {
            this.DrawRightMenu();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawLeftMenu()
    {
        if (this.showBackMenu && GUILayout.Button("Back", new GUILayoutOption[0]))
        {
            SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
        }
        if (this.showChangeCharMenu && GUILayout.Button("Change Char.", new GUILayoutOption[0]))
        {
            WMScriptDirector.Instance.SetToNextChracter();
            if (this.walkMeshOn)
            {
            }
        }
        if (this.showChocoboMenu && !FF9StateSystem.World.IsBeeScene && GUILayout.Button("Choc", new GUILayoutOption[0]))
        {
            FF9StateSystem.EventState.gEventGlobal[181] = 1;
        }
        if (this.showBeeMessage)
        {
            GUILayout.Button("Loaded Blocks " + Singleton<WMWorld>.Instance.ActiveBlockCount + "/480", new GUILayoutOption[0]);
        }
    }

    private void DrawRightMenu()
    {
        if (GUILayout.Button("Disc " + ff9.w_frameDisc, new GUILayoutOption[0]))
        {
            if (ff9.w_frameDisc == 1)
            {
                ff9.w_frameSetParameter(501, 11090);
                return;
            }
            if (ff9.w_frameDisc == 4)
            {
                ff9.w_frameSetParameter(502, 0);
                return;
            }
        }
        if (ff9.w_frameDisc == 1 && GUILayout.Button("Object Form" + this.objectForm, new GUILayoutOption[0]))
        {
            if (this.objectForm == 1)
            {
                this.objectForm = 2;
                Singleton<WMWorld>.Instance.SetBlockForms(this.objectForm);
            }
            else if (this.objectForm == 2)
            {
                this.objectForm = 1;
                Singleton<WMWorld>.Instance.SetBlockForms(this.objectForm);
            }
        }
        if (ff9.w_frameDisc == 1 && GUILayout.Button("Jump To", new GUILayoutOption[0]))
        {
            this.showSetPositionMenu = !this.showSetPositionMenu;
            this.showWorldMapStateMenu = false;
        }
    }

    private void DrawWorldMapStateMenu()
    {
        if (GUILayout.Button("SGATE_DESTROYED", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 2990);
        }
        if (GUILayout.Button("CLAY_DESTROYED", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 4990);
        }
        if (GUILayout.Button("LIND_DESTROssYED", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 5598);
        }
        if (GUILayout.Button("KUROMA_APPEAR", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 6200);
        }
        if (GUILayout.Button("SGATE_RECOVER", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 6990);
        }
        if (GUILayout.Button("ALEX_DESTROYED", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 8800);
        }
        if (GUILayout.Button("HOKORA_START", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 10600);
        }
        if (GUILayout.Button("HOKORA_END", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 10700);
        }
        if (GUILayout.Button("LAST_WORLD", new GUILayoutOption[0]))
        {
            ff9.w_frameSetParameter(501, 11090);
        }
    }

    private void DrawSetPositionMenu()
    {
        if (GUILayout.Button("Go to Cleyra", new GUILayoutOption[0]))
        {
            ff9.w_moveActorPtr.SetPosition(233768.5f, 1101.8f, -201550.9f);
            ff9.w_movementChrInitSlice();
            this.showSetPositionMenu = false;
        }
        if (GUILayout.Button("Go to Dhali", new GUILayoutOption[0]))
        {
            ff9.w_moveActorPtr.SetPosition(283801.3f, 7295.1f, -207228.9f);
            ff9.w_movementChrInitSlice();
            this.showSetPositionMenu = false;
        }
        if (GUILayout.Button("Go to Lindblum", new GUILayoutOption[0]))
        {
            ff9.w_moveActorPtr.SetPosition(232612.8f, 6736.6f, -277148f);
            ff9.w_movementChrInitSlice();
            this.showSetPositionMenu = false;
        }
        if (GUILayout.Button("Go to Alexandria", new GUILayoutOption[0]))
        {
            WMScriptDirector.Instance.SetChocoboAsMainCharacter();
            WMActor controlledDebugDebugActor = WMActor.ControlledDebugDebugActor;
            Singleton<WMWorld>.Instance.SetAbsolutePositionOf(controlledDebugDebugActor.transform, new Vector3(1270.8f, 26.8f, -691.6f), 0f);
            ff9.w_movementChrInitSlice();
            this.showSetPositionMenu = false;
        }
        if (GUILayout.Button("Go to Water Shrine", new GUILayoutOption[0]))
        {
            WMScriptDirector.Instance.SetChocoboAsMainCharacter();
            ff9.w_moveActorPtr.SetPosition(53354.1f, -259.4f, -157476.9f);
            ff9.w_movementChrInitSlice();
            this.showSetPositionMenu = false;
        }
        if (GUILayout.Button("Go to Fire Shrine", new GUILayoutOption[0]))
        {
            WMScriptDirector.Instance.SetChocoboAsMainCharacter();
            ff9.w_moveActorPtr.SetPosition(131381f, 2583.2f, -27989.6f);
            ff9.w_movementChrInitSlice();
            this.showSetPositionMenu = false;
        }
    }

    private void OnGUI()
    {
        if (this.drawMenu)
        {
            this.OnAutoLayout();
        }
    }

    public Boolean ShowMenu;

    private Boolean walkMeshOn;

    private Boolean showRightMenu = true;

    private Vector2 showWorldMapStateMenuScrollPosition = new Vector2(0f, 0f);

    private Boolean showWorldMapStateMenu;

    private Boolean showSetPositionMenu;

    private Boolean showChangeCharMenu;

    private Boolean showWorldStateMenu;

    private Boolean showBackMenu;

    private Boolean showBeeMessage;

    private Int32 objectForm = 1;

    private Boolean drawMenu = true;

    private Single debugUILastTouchTime;

    private Single showHideDebugUICoolDown = 1f;

    private Boolean showChocoboMenu;
}
