using UnityEngine;
public static class GameplayUI
{
    public static bool IsOpen { get { return (InventorySystem.Instance != null && InventorySystem.Instance.isOpen) || (CraftingSystem.Instance != null && CraftingSystem.Instance.isOpen); } }
    public static void UpdateCursor()
    {
        Cursor.lockState = IsOpen ? CursorLockMode.None : CursorLockMode.Locked; Cursor.visible = IsOpen;
        if (CraftingSystem.Instance != null && CraftingSystem.Instance.aimDot != null) CraftingSystem.Instance.aimDot.SetActive(!IsOpen);
        if (CraftingSystem.Instance != null && CraftingSystem.Instance.legacyAimDot != null) CraftingSystem.Instance.legacyAimDot.SetActive(!IsOpen);
    }
}
