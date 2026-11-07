using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public GameObject inventoryScreenUI;
    public List<GameObject> slotlist = new List<GameObject>();
    public List<string> itemlist = new List<string>();
    public bool isOpen;
    public event Action Changed;
    readonly List<InventoryItem> items = new List<InventoryItem>();
    bool transaction;
    void Awake() { if (Instance != null && Instance != this) { enabled = false; return; } Instance = this; }
    void Start()
    {
        slotlist = inventoryScreenUI.GetComponentsInChildren<ItemSlot>(true).Select(s => s.gameObject).ToList();
        foreach (var slot in slotlist)
            foreach (var item in slot.GetComponentsInChildren<InventoryItem>(true)) items.Add(item);
        SetOpen(false); Publish();
    }
    void OnDestroy() { if (Instance == this) Instance = null; }
    void Update() { if (Input.GetKeyDown(KeyCode.I)) SetOpen(!isOpen); }
    public void SetOpen(bool value)
    {
        if (DragDrop.itemBeingDragged != null) return;
        // I controls Inventory alone; C explicitly opens both panels.
        if (CraftingSystem.Instance != null && CraftingSystem.Instance.isOpen) CraftingSystem.Instance.SetOpen(false);
        SetVisibleWithCrafting(value);
    }
    public void SetVisibleWithCrafting(bool value)
    {
        isOpen = value;
        if (inventoryScreenUI != null) inventoryScreenUI.SetActive(value);
        GameplayUI.UpdateCursor();
    }
    public int Count(string id) { return items.Count(i => i != null && string.Equals(i.itemId, id, StringComparison.OrdinalIgnoreCase)); }
    public bool CheckIfFull() { return !slotlist.Any(s => s != null && s.transform.childCount == 0); }
    public void AddToInventory(string id) { TryAdd(id); }
    public bool TryAdd(string id)
    {
        if (transaction || string.IsNullOrWhiteSpace(id)) return false;
        var slot = slotlist.FirstOrDefault(s => s != null && s.transform.childCount == 0);
        var prefab = Resources.Load<GameObject>(id);
        if (slot == null || prefab == null) return false;
        var go = Instantiate(prefab, slot.transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        var item = go.GetComponent<InventoryItem>() ?? go.AddComponent<InventoryItem>();
        item.itemId = id; items.Add(item); Publish(); return true;
    }
    public bool CanCraft(Blueprint recipe, out string reason)
    {
        reason = "";
        if (recipe == null || string.IsNullOrWhiteSpace(recipe.resultItemId) || recipe.resultCount < 1 || recipe.requirements == null || recipe.requirements.Length == 0)
        { reason = "Invalid blueprint"; return false; }
        if (DragDrop.itemBeingDragged != null || transaction) { reason = "Finish the current action first"; return false; }
        if (recipe.requirements.Any(r => r == null || string.IsNullOrWhiteSpace(r.itemId) || r.amount < 1))
        { reason = "Invalid requirements"; return false; }
        foreach (var group in recipe.requirements.GroupBy(r => r.itemId, StringComparer.OrdinalIgnoreCase))
            if (Count(group.Key) < group.Sum(r => r.amount)) { reason = "Gather more materials"; return false; }
        int consumed = recipe.requirements.Sum(r => r.amount);
        if (slotlist.Count(s => s != null && s.transform.childCount == 0) + consumed < recipe.resultCount) { reason = "Inventory has no room for the result"; return false; }
        if (Resources.Load<GameObject>(recipe.resultItemId) == null) { reason = "Result prefab is missing"; return false; }
        return true;
    }
    public bool TryCraft(Blueprint recipe, out string reason)
    {
        if (!CanCraft(recipe, out reason)) return false;
        transaction = true;
        var consumed = new List<InventoryItem>();
        var parents = new List<Transform>();
        var created = new List<InventoryItem>();
        try
        {
            foreach (var group in recipe.requirements.GroupBy(r => r.itemId, StringComparer.OrdinalIgnoreCase))
                consumed.AddRange(items.Where(i => string.Equals(i.itemId, group.Key, StringComparison.OrdinalIgnoreCase)).Take(group.Sum(r => r.amount)));
            foreach (var item in consumed) { parents.Add(item.transform.parent); item.transform.SetParent(transform, false); item.gameObject.SetActive(false); }
            var prefab = Resources.Load<GameObject>(recipe.resultItemId);
            for (int n = 0; n < recipe.resultCount; n++)
            {
                var slot = slotlist.First(s => s.transform.childCount == 0);
                var go = Instantiate(prefab, slot.transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                var item = go.GetComponent<InventoryItem>() ?? go.AddComponent<InventoryItem>();
                item.itemId = recipe.resultItemId; created.Add(item);
            }
            items.RemoveAll(i => consumed.Contains(i)); items.AddRange(created);
            foreach (var item in consumed) Destroy(item.gameObject);
            reason = "Crafted " + recipe.displayName;
        }
        catch (Exception ex)
        {
            foreach (var item in created) { item.transform.SetParent(transform); item.gameObject.SetActive(false); Destroy(item.gameObject); }
            for (int n = 0; n < consumed.Count; n++) { consumed[n].transform.SetParent(parents[n], false); consumed[n].gameObject.SetActive(true); }
            reason = "Craft failed; materials restored"; Debug.LogException(ex); return false;
        }
        finally { transaction = false; }
        Publish(); return true;
    }
    void Publish() { itemlist = items.Where(i => i != null).Select(i => i.itemId).ToList(); Changed?.Invoke(); }
}
