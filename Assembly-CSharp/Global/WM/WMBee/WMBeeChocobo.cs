using System;
using UnityEngine;

public class WMBeeChocobo : MonoBehaviour
{
    public WMActor Actor { get; private set; }

    private void Intialize()
    {
        this.Actor = base.GetComponent<WMActor>();
        this.renderers = base.GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        if (!this.didInitialize)
        {
            this.Intialize();
            this.didInitialize = true;
        }
    }

    public void SetType(Int32 chocoboType)
    {
        if (!this.didInitialize)
        {
            this.Intialize();
            this.didInitialize = true;
        }
        if (chocoboType < Obj.OBJINDEX_CHOCOBO_YELLOW || chocoboType > Obj.OBJINDEX_CHOCOBO_GOLD)
        {
            global::Debug.Log("Uh oh!");
            return;
        }
        this.Actor.originalActor.index = (Byte)chocoboType;
        switch (chocoboType)
        {
            case Obj.OBJINDEX_CHOCOBO_YELLOW:
                this.renderers[0].material = this.NormalChocoboMaterials[0];
                this.renderers[1].material = this.NormalChocoboMaterials[1];
                this.renderers[2].material = this.NormalChocoboMaterials[2];
                break;
            case Obj.OBJINDEX_CHOCOBO_TEAL:
                this.renderers[0].material = this.AsaseChocoboMaterials[0];
                this.renderers[1].material = this.AsaseChocoboMaterials[1];
                this.renderers[2].material = this.AsaseChocoboMaterials[2];
                break;
            case Obj.OBJINDEX_CHOCOBO_RED:
                this.renderers[0].material = this.YamaChocoboMaterials[0];
                this.renderers[1].material = this.YamaChocoboMaterials[1];
                this.renderers[2].material = this.YamaChocoboMaterials[2];
                break;
            case Obj.OBJINDEX_CHOCOBO_BLUE:
                this.renderers[0].material = this.UmiChocoboMaterials[0];
                this.renderers[1].material = this.UmiChocoboMaterials[1];
                this.renderers[2].material = this.UmiChocoboMaterials[2];
                break;
            case Obj.OBJINDEX_CHOCOBO_GOLD:
                this.renderers[0].material = this.SoraChocoboMaterials[0];
                this.renderers[1].material = this.SoraChocoboMaterials[1];
                this.renderers[2].material = this.SoraChocoboMaterials[2];
                break;
        }
    }

    public Material[] NormalChocoboMaterials;
    public Material[] AsaseChocoboMaterials;
    public Material[] YamaChocoboMaterials;
    public Material[] UmiChocoboMaterials;
    public Material[] SoraChocoboMaterials;
    public Renderer[] renderers;

    private Boolean didInitialize;
}
