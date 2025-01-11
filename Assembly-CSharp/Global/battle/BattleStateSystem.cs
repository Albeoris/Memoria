using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Memoria;
using Memoria.Prime;
using Memoria.Scripts;

public class BattleStateSystem : MonoBehaviour
{
    private void Awake()
    {
        try
        {
            this.Init();
            this.InitMapName();
            this.IsPlayFieldBGMInCurrentBattle = false;
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    private void InitMapName()
    {
        this.mapName = new Dictionary<Int32, String>();
        String battleListStr = AssetManager.LoadString("EmbeddedAsset/Manifest/BattleMap/BattleMapList.txt");
        StringReader battleList = new StringReader(battleListStr);
        String battleParamLine;
        while ((battleParamLine = battleList.ReadLine()) != null)
        {
            String[] battleParams = battleParamLine.Split(',');
            Int32.TryParse(battleParams[0], out Int32 battleId);
            if (battleId != -1)
                this.mapName.Add(battleId, battleParams[1]);
        }
    }

    public void Init()
    {
        this.battleMapIndex = 0;
        this.isFrontRow = false;
        this.debugStartType = battle_start_type_tags.BTL_START_NORMAL_ATTACK;
        this.isLevitate = false;
        this.isTrance = new Boolean[4];
        for (Int32 i = 0; i < this.isTrance.Length; i++)
            this.isTrance[i] = false;
        this.isFade = false;
        this.selectCharPosID = 0;
        this.selectPlayerCount = 4;
        this.isDebug = false;
        this.isRandomEncounter = false;
        this.isEncount = false;
        this.isTutorial = false;
        battle.isAlreadyShowTutorial = false;
        this.FF9Battle = new FF9StateBattleSystem();
        this.FF9Battle.status_data = FF9BattleDB.StatusData;
        this.FF9Battle.aa_data = FF9BattleDB.CharacterActions;
        this.FF9Battle.add_status = FF9BattleDB.StatusSets;
        this.fadeShader = ShadersLoader.Find("PSX/BattleMap_Abr_1");
        this.battleShader = ShadersLoader.Find(Configuration.Shaders.BattleCharacterShader);
        this.shadowShader = ShadersLoader.Find("PSX/BattleMap_Abr_2");
        this.detailTexture = AssetManager.Load<Texture2D>("EmbeddedAsset/BattleMap/detailTexture", false);
    }

    public Boolean isNoBoosterMap()
    {
        return this.battleMapIndex == 302 || this.battleMapIndex == 155 || this.battleMapIndex == 160 || this.battleMapIndex == 163 || this.battleMapIndex == 4 || this.battleMapIndex == 73 || this.battleMapIndex == 299;
    }

    public Dictionary<Int32, String> mapName;

    public Int32 battleMapIndex;
    public Byte patternIndex;

    public Boolean isFrontRow;
    public battle_start_type_tags debugStartType;
    public Boolean isLevitate;
    public Boolean[] isTrance;
    public Boolean isFade;
    public Boolean isDebug;
    public Boolean mappingBattleIDWithMapList;
    public Boolean isTutorial;
    public Boolean isRandomEncounter;

    public Int32 selectCharPosID;
    public Int32 selectPlayerCount;

    public Texture2D detailTexture;
    public Shader fadeShader;
    public Shader battleShader;
    public Shader shadowShader;

    public FF9StateBattleSystem FF9Battle;

    public Boolean IsPlayFieldBGMInCurrentBattle;
    public Boolean isEncount;
}
