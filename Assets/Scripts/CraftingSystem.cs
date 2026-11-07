using UnityEngine;
using UnityEngine.UI;
public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }
    public GameObject craftingScreen, categoryScreen, toolsScreen, aimDot, legacyAimDot;
    public Blueprint[] blueprints;
    public CraftingRecipeView[] recipeViews;
    public Text statusText, inventorySummary;
    public Text panelTitle;
    public bool isOpen;
    float nextCraftTime;
    void Awake() { if (Instance != null && Instance != this) { enabled = false; return; } Instance = this; }
    void Start()
    {
        if (InventorySystem.Instance != null) InventorySystem.Instance.Changed += Refresh;
        SetOpen(false); Refresh();
    }
    void OnDestroy()
    {
        if (InventorySystem.Instance != null) InventorySystem.Instance.Changed -= Refresh;
        if (Instance == this) Instance = null;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) SetOpen(!isOpen);
        if (Input.GetKeyDown(KeyCode.Escape)) { SetOpen(false); if (InventorySystem.Instance != null) InventorySystem.Instance.SetOpen(false); }
    }
    public void SetOpen(bool value)
    {
        if (DragDrop.itemBeingDragged != null) return;
        isOpen = value; craftingScreen.SetActive(value);
        if (InventorySystem.Instance != null) InventorySystem.Instance.SetVisibleWithCrafting(value);
        if (value) { ShowCategories(); statusText.text = ""; Refresh(); }
        GameplayUI.UpdateCursor();
    }
    public void Close() { SetOpen(false); }
    public void ShowTools() { categoryScreen.SetActive(false); toolsScreen.SetActive(true); if (panelTitle != null) panelTitle.text = "Tools"; Refresh(); }
    public void ShowCategories() { categoryScreen.SetActive(true); toolsScreen.SetActive(false); if (panelTitle != null) panelTitle.text = "Crafting"; }
    public bool Craft(Blueprint blueprint)
    {
        if (Time.unscaledTime < nextCraftTime) return false;
        nextCraftTime = Time.unscaledTime + .2f;
        string reason = "Inventory unavailable";
        bool ok = InventorySystem.Instance != null && InventorySystem.Instance.TryCraft(blueprint, out reason);
        statusText.text = reason; Refresh(); return ok;
    }
    public void Refresh()
    {
        foreach (var view in recipeViews) if (view != null) view.Refresh();
        if (inventorySummary != null && InventorySystem.Instance != null)
            inventorySummary.text = "Stone " + InventorySystem.Instance.Count("stone") + "    Stick " + InventorySystem.Instance.Count("Stick") + "    Stone Axe " + InventorySystem.Instance.Count("StoneAxe") + "    |    I  Inventory    C  Crafting    Esc  Close";
    }
}
