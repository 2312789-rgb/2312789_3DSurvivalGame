using UnityEngine;
public class InteractableObject : MonoBehaviour
{
    public bool PlayerInRange;
    public string ItemName;
    public bool CanBePickedUp = true;
    public float interactDistance = 5f;
    bool picked;
    public string GetItemName() { return ItemName; }
    public bool TryPickup()
    {
        if (picked || !CanBePickedUp || GameplayUI.IsOpen || SelectionManager.instance == null || SelectionManager.instance.Selected != this || !SelectionManager.instance.IsValidTarget(this)) return false;
        if (InventorySystem.Instance == null || !InventorySystem.Instance.TryAdd(ItemName)) return false;
        picked = true; gameObject.SetActive(false); Destroy(gameObject); return true;
    }
}
