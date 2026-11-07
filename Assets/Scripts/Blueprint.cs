using System;
using UnityEngine;
[Serializable]
public class CraftingRequirement { public string itemId; public int amount = 1; }
[CreateAssetMenu(menuName = "Survival/Blueprint")]
public class Blueprint : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public string resultItemId;
    public int resultCount = 1;
    public CraftingRequirement[] requirements;
}
