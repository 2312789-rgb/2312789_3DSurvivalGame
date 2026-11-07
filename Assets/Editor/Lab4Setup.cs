using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Explicit, one-shot editor installer. Never runs on load or during a build.
public static class Lab4Setup
{
    static Font font;
    static Color ink = new Color(.88f,.91f,.85f), panel = new Color(.055f,.09f,.085f,.98f), gold = new Color(.76f,.62f,.33f);
    static GameObject Rect(string name, Transform parent, Vector2 min, Vector2 max, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform)); go.layer = 5; go.transform.SetParent(parent, false);
        var r = (RectTransform)go.transform; r.anchorMin = min; r.anchorMax = max; r.anchoredPosition = pos; r.sizeDelta = size; return go;
    }
    static GameObject Box(string name, Transform parent, Vector2 min, Vector2 max, Vector2 pos, Vector2 size, Color color)
    { var go = Rect(name,parent,min,max,pos,size); go.AddComponent<Image>().color = color; return go; }
    static Text Label(string name, Transform parent, string text, Vector2 min, Vector2 max, Vector2 pos, Vector2 size, int fontSize = 24)
    {
        var go = Rect(name,parent,min,max,pos,size); var t = go.AddComponent<Text>(); t.font = font; t.fontSize = fontSize;
        t.color = ink; t.text = text; t.alignment = TextAnchor.MiddleLeft; t.raycastTarget = false; return t;
    }
    static Button Button(string name, Transform parent, string text, Vector2 pos, Vector2 size)
    {
        var go = Box(name,parent,new Vector2(.5f,.5f),new Vector2(.5f,.5f),pos,size,new Color(.19f,.29f,.24f));
        var b = go.AddComponent<Button>(); var c = b.colors; c.highlightedColor = new Color(.8f,.85f,.65f); c.disabledColor = new Color(.3f,.3f,.3f,.65f); b.colors=c;
        var t = Label("Label",go.transform,text,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero,23); t.alignment=TextAnchor.MiddleCenter; return b;
    }
    static Sprite Icon(string id, bool axe)
    {
        string path = "Assets/Lab4/" + id + ".png";
        var tex = new Texture2D(96,96,TextureFormat.RGBA32,false);
        for (int y=0;y<96;y++) for(int x=0;x<96;x++)
        {
            Color c=Color.clear;
            float axis = x - (y*.48f+22);
            if (y>9 && y<84 && Mathf.Abs(axis)<5) c = new Color(.49f,.30f,.13f);
            if (y>12 && y<81 && axis> -2 && axis<1) c = new Color(.72f,.50f,.26f);
            if (axe && y>57 && y<84 && x>28 && x<78 && x > 43-(y-57)*.4f) c=new Color(.56f,.65f,.65f);
            if (axe && y>59 && y<82 && x>72 && x<79) c=new Color(.85f,.90f,.86f);
            tex.SetPixel(x,y,c);
        }
        tex.Apply(); File.WriteAllBytes(path,tex.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path); var importer=(TextureImporter)AssetImporter.GetAtPath(path); importer.textureType=TextureImporterType.Sprite; importer.spriteImportMode=SpriteImportMode.Single; importer.alphaIsTransparency=true; importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    static void ItemPrefab(string id, Sprite icon)
    {
        string path="Assets/Resources/"+id+".prefab";
        if (File.Exists(path)) throw new InvalidOperationException("Refusing to overwrite " + path);
        var go=new GameObject(id,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image),typeof(CanvasGroup),typeof(DragDrop),typeof(InventoryItem));
        go.layer=5; ((RectTransform)go.transform).sizeDelta=new Vector2(58,58); go.GetComponent<Image>().sprite=icon; go.GetComponent<Image>().preserveAspect=true; go.GetComponent<InventoryItem>().itemId=id;
        PrefabUtility.SaveAsPrefabAsset(go,path); UnityEngine.Object.DestroyImmediate(go);
    }
    public static string Install()
    {
        if (!Application.dataPath.Replace('\\','/').EndsWith("/2312789_3DSurvivalGame/Assets")) throw new InvalidOperationException("Wrong project");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.name!="SampleScene" || EditorApplication.isPlaying) throw new InvalidOperationException("Wrong scene/mode");
        if (UnityEngine.Object.FindFirstObjectByType<CraftingSystem>()!=null) throw new InvalidOperationException("Crafting already installed");
        Directory.CreateDirectory("Assets/Lab4"); AssetDatabase.Refresh(); font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var stick=Icon("Stick",false); var axe=Icon("StoneAxe",true); ItemPrefab("Stick",stick); ItemPrefab("StoneAxe",axe);
        var blueprint=ScriptableObject.CreateInstance<Blueprint>(); blueprint.displayName="Stone Axe"; blueprint.resultItemId="StoneAxe"; blueprint.resultCount=1; blueprint.icon=axe;
        blueprint.requirements=new[]{new CraftingRequirement{itemId="stone",amount=3},new CraftingRequirement{itemId="Stick",amount=3}};
        AssetDatabase.CreateAsset(blueprint,"Assets/Lab4/StoneAxeBlueprint.asset");
        var canvas=UnityEngine.Object.FindFirstObjectByType<Canvas>();
        var scaler=canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1600,900); scaler.matchWidthOrHeight=.5f;
        var manager=new GameObject("CraftingSystem").AddComponent<CraftingSystem>();
        var screen=Box("CraftingScreen",canvas.transform,new Vector2(.12f,.12f),new Vector2(.88f,.88f),Vector2.zero,Vector2.zero,panel);
        manager.craftingScreen=screen;
        Box("Accent",screen.transform,new Vector2(0,1),new Vector2(1,1),new Vector2(0,-3),new Vector2(0,6),gold);
        Label("Title",screen.transform,"CRAFTING",new Vector2(0,1),new Vector2(1,1),new Vector2(32,-52),new Vector2(-64,60),38);
        Label("Subtitle",screen.transform,"SURVIVAL WORKSHOP   /   2312789",new Vector2(0,1),new Vector2(1,1),new Vector2(32,-100),new Vector2(-64,32),18).color=gold;
        var categories=Rect("Categories",screen.transform,Vector2.zero,Vector2.one,new Vector2(0,-20),new Vector2(-64,-260)); manager.categoryScreen=categories;
        Label("Intro",categories.transform,"Turn gathered materials into useful tools.",new Vector2(0,1),new Vector2(1,1),new Vector2(0,-30),new Vector2(0,50),25);
        var tools=Button("ToolsButton",categories.transform,"TOOLS   >",new Vector2(0,-15),new Vector2(380,100)); UnityEventTools.AddPersistentListener(tools.onClick,manager.ShowTools);
        var toolsScreen=Rect("ToolsCategoryScreen",screen.transform,Vector2.zero,Vector2.one,new Vector2(0,-15),new Vector2(-64,-250)); manager.toolsScreen=toolsScreen;
        var card=Box("StoneAxeBlueprint",toolsScreen.transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero,new Color(.10f,.16f,.14f));
        var view=card.AddComponent<CraftingRecipeView>(); view.blueprint=blueprint;
        var image=Rect("Icon",card.transform,new Vector2(.18f,.58f),new Vector2(.18f,.58f),Vector2.zero,new Vector2(150,150)).AddComponent<Image>(); image.sprite=axe; image.preserveAspect=true; image.raycastTarget=false; view.icon=image;
        view.title=Label("ItemName",card.transform,"Stone Axe",new Vector2(.35f,.78f),new Vector2(.95f,.78f),Vector2.zero,new Vector2(0,50),32);
        view.requirementsText=Label("Requirements",card.transform,"",new Vector2(.35f,.27f),new Vector2(.95f,.67f),Vector2.zero,Vector2.zero,23);
        view.craftButton=Button("CraftButton",card.transform,"CRAFT",new Vector2(0,-130),new Vector2(220,52)); UnityEventTools.AddPersistentListener(view.craftButton.onClick,view.Craft);
        var back=Button("BackButton",toolsScreen.transform,"<  BACK",new Vector2(-370,-130),new Vector2(160,52)); UnityEventTools.AddPersistentListener(back.onClick,manager.ShowCategories);
        var close=Button("CloseButton",screen.transform,"CLOSE  [C]",Vector2.zero,new Vector2(170,46)); var closeRect=(RectTransform)close.transform; closeRect.anchorMin=closeRect.anchorMax=new Vector2(1,1); closeRect.anchoredPosition=new Vector2(-120,-52); UnityEventTools.AddPersistentListener(close.onClick,manager.Close);
        manager.statusText=Label("Status",screen.transform,"Choose Tools to view blueprints",new Vector2(0,0),new Vector2(1,0),new Vector2(32,43),new Vector2(-64,42),21);
        manager.statusText.color=gold; manager.blueprints=new[]{blueprint}; manager.recipeViews=new[]{view};
        manager.inventorySummary=Label("Lab4HUD",canvas.transform,"I  Inventory    C  Crafting    LMB  Pick up",new Vector2(0,0),new Vector2(1,0),new Vector2(24,25),new Vector2(-48,40),19);
        manager.inventorySummary.color=new Color(1,1,.85f);
        var crosshair=Label("AimDot",canvas.transform,"+",new Vector2(.5f,.5f),new Vector2(.5f,.5f),Vector2.zero,new Vector2(28,28),22); crosshair.alignment=TextAnchor.MiddleCenter;
        screen.SetActive(false); manager.toolsScreen.SetActive(false);
        var inventory=UnityEngine.Object.FindFirstObjectByType<InventorySystem>(); inventory.inventoryScreenUI.SetActive(false);
        var stoneIcon=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/stone.png");
        var ground=new GameObject("Lab4_Materials");
        var player=UnityEngine.Object.FindFirstObjectByType<PlayerMovement>().transform;
        for(int i=0;i<6;i++)
        {
            bool isStick=i>=3; var go=GameObject.CreatePrimitive(isStick ? PrimitiveType.Capsule : PrimitiveType.Sphere); go.name=isStick ? "Stick Pickup "+(i-2) : "Stone Pickup "+(i+1); go.transform.SetParent(ground.transform);
            var p=player.position+new Vector3((i%3-1)*1.4f,0,3+(isStick ? 1.5f : 0)); RaycastHit hit;
            if(Physics.Raycast(p+Vector3.up*30,Vector3.down,out hit,100,1<<3)) p.y=hit.point.y+.3f;
            go.transform.position=p; go.transform.localScale=isStick ? new Vector3(.16f,.55f,.16f) : new Vector3(.55f,.4f,.5f);
            if(isStick) go.transform.rotation=Quaternion.Euler(0,20,75);
            var material=new Material(Shader.Find("Universal Render Pipeline/Lit")); material.color=isStick ? new Color(.36f,.20f,.09f) : new Color(.4f,.45f,.44f); string matPath="Assets/Lab4/"+(isStick ? "Stick" : "Stone")+"Material.mat";
            var existing=AssetDatabase.LoadAssetAtPath<Material>(matPath); if(existing==null) { AssetDatabase.CreateAsset(material,matPath); existing=material; } else UnityEngine.Object.DestroyImmediate(material);
            go.GetComponent<Renderer>().sharedMaterial=existing; var pickup=go.AddComponent<InteractableObject>(); pickup.ItemName=isStick ? "Stick" : "stone";
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        return "Installed saved Crafting UI, Blueprint, item prefabs and six material pickups in " + scene.path;
    }
}
