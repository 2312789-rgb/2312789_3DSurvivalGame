using UnityEngine;
using UnityEngine.UI;
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance { get; set; }
    public bool onTarget;
    public GameObject interaction_Info_UI;
    public float maxDistance = 5f;
    public InteractableObject Selected { get; private set; }
    void Awake() { if (instance != null && instance != this) { enabled = false; return; } instance = this; }
    void OnDestroy() { if (instance == this) instance = null; }
    public bool IsValidTarget(InteractableObject target)
    {
        if (GameplayUI.IsOpen || Camera.main == null || target == null || !target.gameObject.activeInHierarchy) return false;
        RaycastHit hit;
        return Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f,.5f,0)), out hit, Mathf.Min(maxDistance,target.interactDistance), ~0, QueryTriggerInteraction.Ignore) && hit.collider.GetComponentInParent<InteractableObject>() == target;
    }
    public void RefreshSelection()
    {
        Selected = null;
        if (!GameplayUI.IsOpen && Camera.main != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f,.5f,0)), out hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                var target = hit.collider.GetComponentInParent<InteractableObject>();
                if (target != null && hit.distance <= target.interactDistance) { Selected = target; target.PlayerInRange = true; }
            }
        }
        onTarget = Selected != null;
        if (interaction_Info_UI != null)
        {
            interaction_Info_UI.SetActive(onTarget);
            if (onTarget) interaction_Info_UI.GetComponent<Text>().text = Selected.ItemName + (Selected.CanBePickedUp ? "  [LMB] Pick up" : "");
        }
    }
    void Update() { RefreshSelection(); if (Selected != null && Input.GetMouseButtonDown(0)) { Selected.TryPickup(); RefreshSelection(); } }
}
