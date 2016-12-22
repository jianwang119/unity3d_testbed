using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyImage : Image
{
    protected override void UpdateMaterial()
    {
        if (!IsActive())
            return;

        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(materialForRendering, 0);
        //canvasRenderer.SetTexture(mainTexture);
    }
}
