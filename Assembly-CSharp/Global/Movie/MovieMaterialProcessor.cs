using Assets.Sources.Graphics.Movie;
using System;
using UnityEngine;

public class MovieMaterialProcessor : MonoBehaviour
{
    public MovieMaterial MovieMaterial { get; set; }

    private void Update()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 2933 && this.MovieMaterial != null)
        {
            // last/cw mbg 1
            this.MovieMaterial.Update();
            MobileMovieManager.NativeUpdate();
        }
    }

    private void LateUpdate()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo != 2933 && this.MovieMaterial != null)
        {
            // last/cw mbg 1
            this.MovieMaterial.Update();
            MobileMovieManager.NativeUpdate();
        }
    }
}
