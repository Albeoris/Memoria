using Assets.Sources.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayBoosterUI : MonoBehaviour
{
    private void Awake()
    {
        this.boosterImageList = new List<Image>
        {
            this.booster1Image,
            this.booster2Image,
            this.booster3Image,
            this.booster4Image,
            this.booster5Image
        };
        this.boosterSpriteList = new List<Sprite>
        {
            this.highSpeedModeSprite,
            this.battleAssistanceModeSprite,
            this.attack9999ModeSprite,
            this.noRandomEncounterModeSprite,
            this.MasterSkillModeSprite
        };
        this.boosterEnableList = new List<Int32>
        {
            -1,
            -1,
            -1,
            -1,
            -1
        };
        this.origBoosterContainerScale = this.boosterContainer.transform.localScale;
        this.boosterContainerRectTrans = this.boosterContainer.GetComponent<RectTransform>();
        this.origBoosterContainerPos = this.boosterContainerRectTrans.anchoredPosition;
        this.UpdateBoosterSize();
    }

    private void OnGUI()
    {
        if (!base.gameObject.activeSelf || (!SettingsState.IsRapidEncounter && SettingsState.IsFriendlyBattleOnly == 0))
            return;

        GUI.skin.font = DebugGuiSkin.font;
        GUILayout.BeginArea(new Rect(5, 5, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        if (SettingsState.IsRapidEncounter)
        {
            if (GUILayout.Button("Rapid Encounter", GUILayout.ExpandWidth(false)))
                SettingsState.IsRapidEncounter = false;
        }
        if (SettingsState.IsFriendlyBattleOnly > 0)
        {
            if (GUILayout.Button(SettingsState.IsFriendlyBattleOnly == 1 ? "Friendly Only" : "Ragtime Only", GUILayout.ExpandWidth(false)))
                SettingsState.IsFriendlyBattleOnly = (SettingsState.IsFriendlyBattleOnly + 1) % 3;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void Restart()
    {
        this.UpdateBoosterSize();
    }

    private void LateUpdate()
    {
    }

    public void UpdateBoosterSize()
    {
        if (!base.gameObject.activeSelf)
        {
            return;
        }
        Single num = (Single)Screen.width / OverlayCanvas.ReferenceScreenSize.x;
        Single num2 = this.boosterContainerRectTrans.sizeDelta.x * (this.origBoosterContainerScale.x * num);
        Single num3 = this.boosterContainerRectTrans.sizeDelta.y * (this.origBoosterContainerScale.y * num);
        Single x = -1f * this.origBoosterContainerPos.x * (this.origBoosterContainerScale.x - this.origBoosterContainerScale.x * num) + this.origBoosterContainerPos.x;
        Single y = -1f * this.origBoosterContainerPos.y * (this.origBoosterContainerScale.y - this.origBoosterContainerScale.y * num) + this.origBoosterContainerPos.y;

        this.boosterContainerRectTrans.anchoredPosition = new Vector2(x, y);
        if (UIManager.UIPillarBoxSize.y <= 0f)
        {
            Single num4 = (Single)((PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.Battle) ? 0 : 6);
            this.boosterContainerRectTrans.anchoredPosition = new Vector2(-(UIManager.UIPillarBoxSize.x + num4), this.boosterContainerRectTrans.anchoredPosition.y);
        }
        this.boosterContainer.transform.localScale = new Vector3(this.origBoosterContainerScale.x * num, this.origBoosterContainerScale.y * num, this.origBoosterContainerScale.z);
    }

    public void SetBoosterIcon(BoosterType type, Boolean isActive)
    {
        if (!base.gameObject.activeSelf)
        {
            return;
        }
        BoosterSlider.BoosterIcon boosterIcon = BoosterSlider.BoosterIcon.None;
        if (type == BoosterType.HighSpeedMode)
        {
            boosterIcon = BoosterSlider.BoosterIcon.HighSpeedMode;
        }
        else if (type == BoosterType.BattleAssistance)
        {
            boosterIcon = BoosterSlider.BoosterIcon.BattleAssistance;
        }
        else if (type == BoosterType.Attack9999)
        {
            boosterIcon = BoosterSlider.BoosterIcon.Attack9999;
        }
        else if (type == BoosterType.NoRandomEncounter)
        {
            boosterIcon = BoosterSlider.BoosterIcon.NoRandomEncounter;
        }
        else if (type == BoosterType.MasterSkill)
        {
            boosterIcon = BoosterSlider.BoosterIcon.MasterSkill;
        }
        if (boosterIcon != BoosterSlider.BoosterIcon.None)
        {
            Int32 num = this.boosterEnableList.IndexOf((Int32)boosterIcon);
            if (num == -1)
            {
                if (isActive)
                {
                    Int32 num2 = 0;
                    foreach (Int32 num3 in this.boosterEnableList)
                    {
                        if (num3 > -1 && num3 > (Int32)boosterIcon)
                        {
                            num2++;
                        }
                    }
                    this.boosterEnableList.RemoveAt(this.boosterEnableList.Count - 1);
                    this.boosterEnableList.Insert(num2, (Int32)boosterIcon);
                }
            }
            else if (isActive)
            {
                this.boosterEnableList[num] = (Int32)boosterIcon;
            }
            else
            {
                this.boosterEnableList[num] = -1;
            }
            for (Int32 i = 0; i < this.boosterEnableList.Count; i++)
            {
                if (this.boosterEnableList[i] == -1)
                {
                    this.boosterEnableList.RemoveAt(i);
                    this.boosterEnableList.Add(-1);
                }
            }
            this.UpdateBoosterIconImage();
        }
    }

    private void UpdateBoosterIconImage()
    {
        for (Int32 i = 0; i < this.boosterEnableList.Count; i++)
        {
            Int32 num = this.boosterEnableList[i];
            Image image = this.boosterImageList[i];
            if (num == -1)
            {
                image.gameObject.SetActive(false);
            }
            else
            {
                image.gameObject.SetActive(true);
                image.sprite = this.boosterSpriteList[num];
            }
        }
    }

    public Image booster1Image;

    public Image booster2Image;

    public Image booster3Image;

    public Image booster4Image;

    public Image booster5Image;

    public GameObject boosterContainer;

    public Sprite highSpeedModeSprite;

    public Sprite battleAssistanceModeSprite;

    public Sprite attack9999ModeSprite;

    public Sprite noRandomEncounterModeSprite;

    public Sprite MasterSkillModeSprite;

    private List<Image> boosterImageList;

    private List<Sprite> boosterSpriteList;

    private List<Int32> boosterEnableList;

    private Vector3 origBoosterContainerScale;

    private Vector3 origBoosterContainerPos;

    private RectTransform boosterContainerRectTrans;
}
