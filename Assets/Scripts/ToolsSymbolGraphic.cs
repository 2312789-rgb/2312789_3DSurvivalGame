using UnityEngine;
using UnityEngine.UI;

// Crossed axe/pickaxe silhouette matching the category symbol in the reference.
[RequireComponent(typeof(CanvasRenderer))]
public class ToolsSymbolGraphic : MaskableGraphic
{
    void Quad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        int i=vh.currentVertCount; var r=rectTransform.rect;
        foreach(var p in new[]{a,b,c,d}) vh.AddVert(new Vector3(r.xMin+p.x*r.width,r.yMin+p.y*r.height),color,Vector2.zero);
        vh.AddTriangle(i,i+1,i+2); vh.AddTriangle(i,i+2,i+3);
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Quad(vh,new Vector2(.16f,.12f),new Vector2(.23f,.08f),new Vector2(.78f,.78f),new Vector2(.71f,.83f));
        Quad(vh,new Vector2(.78f,.08f),new Vector2(.86f,.15f),new Vector2(.30f,.82f),new Vector2(.23f,.76f));
        Quad(vh,new Vector2(.10f,.67f),new Vector2(.23f,.57f),new Vector2(.45f,.79f),new Vector2(.26f,.96f));
        Quad(vh,new Vector2(.38f,.79f),new Vector2(.42f,.87f),new Vector2(.79f,.90f),new Vector2(.82f,.79f));
        Quad(vh,new Vector2(.79f,.90f),new Vector2(.82f,.79f),new Vector2(.95f,.60f),new Vector2(.90f,.85f));
    }
}
