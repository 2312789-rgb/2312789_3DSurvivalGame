using UnityEngine;
using UnityEngine.UI;
public class CraftingRecipeView : MonoBehaviour
{
    public Blueprint blueprint;
    public Text title, requirementsText;
    public Image icon;
    public Button craftButton;
    public void Refresh()
    {
        if (blueprint == null) return;
        title.text = blueprint.displayName; icon.sprite = blueprint.icon;
        var inventory = InventorySystem.Instance;
        requirementsText.text = "";
        foreach (var r in blueprint.requirements)
        {
            int count = inventory != null ? inventory.Count(r.itemId) : 0;
            string itemName = r.itemId.Length > 0 ? char.ToUpperInvariant(r.itemId[0]) + r.itemId.Substring(1) : "";
            requirementsText.text += r.amount + " " + itemName + " [" + count + "]\n";
        }
        string reason;
        craftButton.interactable = inventory != null && inventory.CanCraft(blueprint, out reason);
    }
    public void Craft() { if (CraftingSystem.Instance != null) CraftingSystem.Instance.Craft(blueprint); }
}
