using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Lab4UIRevision
{
    static Sprite Sprite(string path) { return AssetDatabase.LoadAssetAtPath<Sprite>(path); }
    static void Top(GameObject go, float x, float y, float width, float height)
    {
        var r=(RectTransform)go.transform; r.anchorMin=r.anchorMax=new Vector2(0,1); r.pivot=new Vector2(0,1);
        r.anchoredPosition=new Vector2(x,-y); r.sizeDelta=new Vector2(width,height); r.localScale=Vector3.one;
    }
    static void Stretch(GameObject go)
    {
        var r=(RectTransform)go.transform; r.anchorMin=Vector2.zero; r.anchorMax=Vector2.one; r.offsetMin=r.offsetMax=Vector2.zero; r.localScale=Vector3.one;
    }
    static GameObject Child(string name,Transform parent)
    {
        var existing=parent.Find(name); if(existing!=null) return existing.gameObject;
        var go=new GameObject(name,typeof(RectTransform)); go.layer=5; go.transform.SetParent(parent,false); return go;
    }
    static Text Text(GameObject go,string value,int size,Color color,TextAnchor alignment)
    {
        var text=go.GetComponent<Text>() ?? go.AddComponent<Text>(); text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize=size; text.text=value; text.color=color; text.alignment=alignment; text.raycastTarget=false; text.fontStyle=FontStyle.Normal;
        text.horizontalOverflow=HorizontalWrapMode.Wrap; text.verticalOverflow=VerticalWrapMode.Truncate; return text;
    }
    public static string Apply()
    {
        if(!Application.dataPath.Replace('\\','/').EndsWith("/2312789_3DSurvivalGame/Assets") || EditorApplication.isPlaying) throw new InvalidOperationException("Wrong project or Play Mode");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene(); if(scene.name!="SampleScene") throw new InvalidOperationException("Wrong scene");
        var c=UnityEngine.Object.FindFirstObjectByType<CraftingSystem>(); var i=UnityEngine.Object.FindFirstObjectByType<InventorySystem>(); var canvas=i.inventoryScreenUI.GetComponentInParent<Canvas>();
        var scaler=canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080); scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
        var inv=(RectTransform)i.inventoryScreenUI.transform; inv.anchorMin=inv.anchorMax=new Vector2(.5f,.5f); inv.pivot=new Vector2(0,1); inv.anchoredPosition=new Vector2(-825,410); inv.sizeDelta=new Vector2(790,495);
        var slots=i.inventoryScreenUI.GetComponentsInChildren<ItemSlot>(true);
        for(int n=0;n<slots.Length;n++)
        {
            Top(slots[n].gameObject,35+(n%7)*103,50+(n/7)*103,98,98);
            var slotRect=(RectTransform)slots[n].transform; slotRect.pivot=new Vector2(.5f,.5f); slotRect.anchoredPosition+=new Vector2(49,-49);
        }
        var oldReticle=canvas.transform.Find("Image"); c.legacyAimDot=oldReticle!=null ? oldReticle.gameObject : null;
        var screen=(RectTransform)c.craftingScreen.transform; screen.anchorMin=screen.anchorMax=new Vector2(.5f,.5f); screen.pivot=new Vector2(0,1); screen.anchoredPosition=new Vector2(72,410); screen.sizeDelta=new Vector2(660,822);
        var background=c.craftingScreen.GetComponent<Image>(); background.sprite=Sprite("Assets/GUI_Parts/Gui_parts/big_background.png"); background.type=Image.Type.Simple; background.color=Color.white;
        var frame=Child("MedievalFrame",screen); Stretch(frame); var frameImage=frame.GetComponent<Image>() ?? frame.AddComponent<Image>(); frameImage.sprite=Sprite("Assets/GUI_Parts/Gui_parts/Frame_big.png"); frameImage.raycastTarget=false; frameImage.color=Color.white;
        foreach(var name in new[]{"Accent","Subtitle","CloseButton"}) {var old=screen.Find(name); if(old!=null) old.gameObject.SetActive(false);}
        c.inventorySummary.gameObject.SetActive(false);
        c.panelTitle=Text(screen.Find("Title").gameObject,"Crafting",58,Color.white,TextAnchor.MiddleCenter); c.panelTitle.fontStyle=FontStyle.Bold; Top(c.panelTitle.gameObject,35,26,590,90);
        Stretch(c.categoryScreen); Stretch(c.toolsScreen);
        foreach(Transform child in c.categoryScreen.transform) if(child.name!="ToolsButton") child.gameObject.SetActive(false);
        var toolsButton=c.categoryScreen.transform.Find("ToolsButton").GetComponent<Button>(); Top(toolsButton.gameObject,62,170,154,154);
        toolsButton.GetComponent<Image>().sprite=Sprite("Assets/GUI_Parts/Gui_parts/button2_ready_off.png"); toolsButton.GetComponent<Image>().color=Color.white;
        var toolsColors=toolsButton.colors; toolsColors.normalColor=Color.white; toolsColors.highlightedColor=new Color(1,.9f,.75f); toolsColors.pressedColor=new Color(.7f,.65f,.6f); toolsButton.colors=toolsColors;
        var toolFrame=Child("Frame",toolsButton.transform); Stretch(toolFrame); var fi=toolFrame.GetComponent<Image>() ?? toolFrame.AddComponent<Image>(); fi.sprite=Sprite("Assets/GUI_Parts/Gui_parts/Mini_frame1.png"); fi.raycastTarget=false;
        var symbol=Child("CrossedTools",toolsButton.transform); Top(symbol,27,27,100,100); var graphic=symbol.GetComponent<ToolsSymbolGraphic>() ?? symbol.AddComponent<ToolsSymbolGraphic>(); graphic.color=Color.white; graphic.raycastTarget=false;
        var toolsLabel=toolsButton.transform.Find("Label").gameObject; Top(toolsLabel,-10,153,174,56); Text(toolsLabel,"Tools",31,Color.white,TextAnchor.MiddleCenter).fontStyle=FontStyle.Bold;
        var view=c.recipeViews[0]; Top(view.gameObject,70,150,170,260); var oldCard=view.GetComponent<Image>(); oldCard.enabled=false;
        var paperObject=Child("Parchment",view.transform); Stretch(paperObject); paperObject.transform.SetAsFirstSibling();
        var paper=paperObject.GetComponent<ParchmentGraphic>() ?? paperObject.AddComponent<ParchmentGraphic>(); paper.color=Color.white; paper.raycastTarget=false;
        Top(view.icon.gameObject,55,23,60,60); view.icon.sprite=Sprite("Assets/GUI_Parts/Icons/weapon_icon.png"); view.icon.preserveAspect=true;
        var axeFrame=Child("AxeIconFrame",view.transform); Top(axeFrame,53,21,64,64); var ai=axeFrame.GetComponent<Image>() ?? axeFrame.AddComponent<Image>(); ai.sprite=Sprite("Assets/GUI_Parts/Gui_parts/Mini_frame1.png"); ai.raycastTarget=false;
        Top(view.title.gameObject,12,83,146,38); Text(view.title.gameObject,"Axe",25,new Color(.17f,.11f,.06f),TextAnchor.MiddleCenter);
        Top(view.requirementsText.gameObject,21,125,139,65); Text(view.requirementsText.gameObject,"3 Stone [0]\n3 Stick [0]",19,new Color(.24f,.17f,.10f),TextAnchor.UpperLeft);
        Top(view.craftButton.gameObject,18,212,134,29); var craftImage=view.craftButton.GetComponent<Image>(); craftImage.sprite=Sprite("Assets/GUI_Parts/Gui_parts/button.png"); craftImage.color=Color.white;
        var cc=view.craftButton.colors; cc.normalColor=Color.white; cc.highlightedColor=new Color(1,.9f,.7f); cc.disabledColor=new Color(.55f,.5f,.43f); view.craftButton.colors=cc;
        var craftLabel=view.craftButton.GetComponentInChildren<Text>(); Stretch(craftLabel.gameObject); Text(craftLabel.gameObject,"Craft",15,Color.white,TextAnchor.MiddleCenter);
        var back=c.toolsScreen.transform.Find("BackButton").gameObject; Top(back,70,740,120,35); var bi=back.GetComponent<Image>(); bi.sprite=craftImage.sprite; bi.color=Color.white;
        var bc=back.GetComponent<Button>().colors; bc.normalColor=Color.white; bc.highlightedColor=new Color(1,.9f,.7f); back.GetComponent<Button>().colors=bc; var bl=back.GetComponentInChildren<Text>(); Stretch(bl.gameObject); Text(bl.gameObject,"Back",20,Color.white,TextAnchor.MiddleCenter);
        Top(c.statusText.gameObject,45,685,570,45); Text(c.statusText.gameObject,"",20,new Color(.8f,.72f,.56f),TextAnchor.MiddleCenter);
        var blueprint=c.blueprints[0]; blueprint.displayName="Axe"; blueprint.icon=view.icon.sprite; EditorUtility.SetDirty(blueprint);
        foreach(var id in new[]{"stone","Stick","StoneAxe"})
        {
            string path="Assets/Resources/"+id+".prefab"; var prefab=PrefabUtility.LoadPrefabContents(path); ((RectTransform)prefab.transform).sizeDelta=new Vector2(86,86);
            if(id=="StoneAxe") prefab.GetComponent<Image>().sprite=blueprint.icon;
            if(id=="stone") prefab.GetComponent<Image>().sprite=Sprite("Assets/Resources/stone.png");
            prefab.GetComponent<Image>().preserveAspect=true; PrefabUtility.SaveAsPrefabAsset(prefab,path); PrefabUtility.UnloadPrefabContents(prefab);
        }
        frame.transform.SetAsLastSibling(); c.craftingScreen.SetActive(false); c.categoryScreen.SetActive(true); c.toolsScreen.SetActive(false); i.inventoryScreenUI.SetActive(false);
        EditorUtility.SetDirty(c); EditorUtility.SetDirty(i); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        return "Saved medieval Inventory/Crafting/Tools revision in SampleScene";
    }
    // Test-only Game View resolution selector; restore with the saved index afterwards.
    public static int Resolution(int width,int height)
    {
        var assembly=typeof(Editor).Assembly; var sizesType=assembly.GetType("UnityEditor.GameViewSizes"); var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType); var sizes=singleton.GetProperty("instance").GetValue(null,null);
        var group=sizesType.GetMethod("GetGroup").Invoke(sizes,new object[]{Enum.Parse(assembly.GetType("UnityEditor.GameViewSizeGroupType"),"Standalone")});
        var sizeType=assembly.GetType("UnityEditor.GameViewSize"); var fixedType=Enum.Parse(assembly.GetType("UnityEditor.GameViewSizeType"),"FixedResolution");
        var constructor=sizeType.GetConstructors(System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic).First(x=>x.GetParameters().Length==4);
        var size=constructor.Invoke(new object[]{fixedType,width,height,"Lab4 UI QA "+width+"x"+height}); group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});
        int built=(int)group.GetType().GetMethod("GetBuiltinCount").Invoke(group,null),custom=(int)group.GetType().GetMethod("GetCustomCount").Invoke(group,null);
        var gameType=assembly.GetType("UnityEditor.GameView"); var window=EditorWindow.GetWindow(gameType); var property=gameType.GetProperty("selectedSizeIndex",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic);
        int old=(int)property.GetValue(window,null); property.SetValue(window,built+custom-1,null); window.Repaint(); return old;
    }
}
