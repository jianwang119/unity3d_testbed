using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyText : Text
{
    private int m_fontTexId = -1;

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);

        UIVertex vertex = new UIVertex(); ;
        int vcount = toFill.currentVertCount;
        for (int i = 0; i < vcount; i++)
        {
            toFill.PopulateUIVertex(ref vertex, i);
            vertex.uv1 = new Vector2(1, 0);
            toFill.SetUIVertex(vertex, i);
        }
    }

    protected override void UpdateMaterial()
    {
        if (!IsActive())
            return;

        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial(materialForRendering, 0);

        if (m_fontTexId == -1)
        {
            m_fontTexId = Shader.PropertyToID("_FontTex");
        }
        materialForRendering.SetTexture(m_fontTexId, mainTexture);
        //canvasRenderer.SetTexture(mainTexture);
    }
}
