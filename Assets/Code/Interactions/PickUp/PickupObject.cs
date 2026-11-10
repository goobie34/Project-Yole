using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerCarryController carryController;

    public bool IsCarried { get; private set; }

    private void Awake()
    {
        if (carryController == null) carryController = FindFirstObjectByType<PlayerCarryController>();
    }

    public void Pickup()
    {
        if (IsCarried || carryController == null) return;

        carryController.TryPickup(this);
    }

    public void SetCarried(bool carried)
    {
        IsCarried = carried;
    }
}
