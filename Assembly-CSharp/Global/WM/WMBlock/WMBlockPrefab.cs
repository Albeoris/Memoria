using System;
using UnityEngine;

public class WMBlockPrefab : MonoBehaviour
{
    public Transform TerrainForm1;

    public Transform ObjectForm1;

    public Transform TerrainForm2;

    public Transform ObjectForm2;

    public Transform Beach1;

    public Transform Beach2;

    public Transform Stream;

    public Transform River;

    public Transform RiverJoint;

    public Transform Falls;

    public Transform Sea1;

    public Transform Sea2;

    public Transform Sea3;

    public Transform Sea4;

    public Transform Sea5;

    public Transform Sea6;

    public Transform VolcanoCrater1;

    public Transform VolcanoLava1;

    public Transform VolcanoCrater2;

    public Transform VolcanoLava2;

    public Transform Sea3_2;

    public Transform Sea4_2;

    public Transform Sea5_2;

    public Boolean Is3_9;

    public Boolean IsSea;

    public Boolean HasSpecialObject;

    public Boolean IsSwitchable;

    public Boolean HasRiver;

    public Boolean HasRiverJoint;

    public Boolean HasStream;

    public Boolean HasFalls;

    public Boolean HasBeach1;

    public Boolean HasBeach2;

    public Boolean HasSea;

    public Boolean HasVolcanoCrater;

    public Boolean HasVolcanoLava;

    public Int32 InitialX;

    public Int32 InitialY;

    public Int32 Number;

    public Int32 CurrentX;

    public Int32 CurrentY;

    public Boolean InsideFrustum;

    public Color DebugColor = Color.yellow;

    public Bounds Bounds;

    public Boolean StartedLoadAsync;

    public Int32 Form;

    public Boolean IsReady;
}
