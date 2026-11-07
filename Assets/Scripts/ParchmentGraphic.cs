using UnityEngine;
using UnityEngine.UI;

// Saved UI-native paper mesh. No runtime UI construction or downloaded textures.
[RequireComponent(typeof(CanvasRenderer))]
public class ParchmentGraphic : MaskableGraphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); var r = rectTransform.rect;
        const int columns = 24, rows = 36;
        for (int y=0; y<=rows; y++) for(int x=0; x<=columns; x++)
        {
            float fx=x/(float)columns, fy=y/(float)rows;
            float px=Mathf.Lerp(r.xMin+3,r.xMax-3,fx), py=Mathf.Lerp(r.yMin+3,r.yMax-3,fy);
            if(x==0 || x==columns) px+=Mathf.Sin(y*5.7f)*2f;
            if(y==0 || y==rows) py+=Mathf.Sin(x*4.3f)*2.5f;
            float edge=Mathf.Min(Mathf.Min(fx,1-fx),Mathf.Min(fy,1-fy));
            float grain=Mathf.PerlinNoise(x*2.31f,y*1.93f)*.09f;
            var c=Color.Lerp(new Color(.48f,.31f,.13f),new Color(.94f,.82f,.59f),Mathf.Clamp01(edge*18f));
            c*=1f-grain; c.a=color.a;
            vh.AddVert(new Vector3(px,py),c,Vector2.zero);
        }
        for(int y=0;y<rows;y++) for(int x=0;x<columns;x++)
        {int n=y*(columns+1)+x;vh.AddTriangle(n,n+columns+1,n+1);vh.AddTriangle(n+1,n+columns+1,n+columns+2);}
    }
}
