using System;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public void NewGame()
    {
        ff9.w_frameSystemClean();
    }

    private void Awake()
    {
        this.LoadingType = WorldState.Loading.SplitAsync;
        this.PrintLogOnLoadingBlocksAsync = false;
        this.IsBeeScene = false;
        this.CreateHonoUpsDisplay = false;
        this.CreateMonoUpsDisplay = false;
        this.CreateFpsDisplay = true;
        this.DiscardBlockWhenStreaming = true;
        this.MaximumLoadAsynce = 3;
        this.HasLowFpsOnDevice = false;
        this.FixTypeCam = true;
        this.FF9World = new FF9StateWorldSystem();
        this.LoadingType = WorldState.Loading.SplitAsync;
        this.DiscardBlockWhenStreaming = false;
        this.MaximumLoadAsynce = 2;
        this.CreateHonoUpsDisplay = false;
        this.CreateMonoUpsDisplay = false;
        this.CreateFpsDisplay = false;
        this.PrintLogOnLoadingBlocksAsync = false;
        this.IsBeeScene = false;
    }

    public WorldState.Loading LoadingType;

    public Boolean PrintLogOnLoadingBlocksAsync;

    public Boolean IsBeeScene;

    public Boolean CreateHonoUpsDisplay;

    public Boolean CreateMonoUpsDisplay;

    public Boolean CreateFpsDisplay;

    public Boolean DiscardBlockWhenStreaming;

    public Int32 MaximumLoadAsynce = 3;

    public Boolean HasLowFpsOnDevice;

    public Boolean FixTypeCam;

    public FF9StateWorldSystem FF9World;

    public enum Loading
    {
        AllAtOnce,
        SplitAsync
    }
}
