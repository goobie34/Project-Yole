using UnityEngine;

public class WorldItemInteraction : MonoBehaviour
{
    private Interactable interaction;
    public bool IsCarried {  get; private set; }
    public bool IsStored { get; private set; }

    private void Awake() => EnsureInteraction();

    private void EnsureInteraction()
    {
        if (interaction != null) return;
        interaction = GetComponent<Interactable>();
        if (interaction == null) interaction = gameObject.AddComponent<Interactable>();
        interaction.OnInteract.AddListener(Pickup);
        Refresh();
    }

    public static WorldItemInteraction GetOrCreate(Transform item)
    {
        WorldItemInteraction state = item.GetComponent<WorldItemInteraction>();
        if (state == null) state = item.gameObject.AddComponent<WorldItemInteraction>();
        state.EnsureInteraction();
        return state;
    }

    public void SetCarried(bool value) { IsCarried = value; Refresh(); }
    public void SetStored(bool value) { IsStored = value; Refresh(); }

    private void Refresh()
    {
        if (interaction != null) interaction.isCurrentlyInteractable = !IsCarried && !IsStored;
    }

    private void Pickup()
    {
        if (IsCarried || IsStored) return;
        PlayerCarryController player = FindFirstObjectByType<PlayerCarryController>();
        if (player != null) player.TryPickup(this);
    }

    private void OnDestroy()
    {
        if (interaction != null) interaction.OnInteract.RemoveListener(Pickup);
    }
}
