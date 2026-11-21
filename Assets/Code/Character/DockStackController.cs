using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DockInventory), typeof(Interactable))]
public class DockStackController : MonoBehaviour
{
    [Min(0f)] public float returnRange = 2.5f;
    [Range(0f, 180f)] public float returnAngle = 75f;
    [SerializeField] private PlayerCarryController carryController;

    private static readonly List<DockStackController> stacks = new();
    private DockInventory dock;
    private Interactable interaction;
    private ItemInstance placing;
    private bool warned;

    private void Awake()
    {
        dock = GetComponent<DockInventory>();
        interaction = GetComponent<Interactable>();
        interaction.OnInteract.AddListener(InteractWithStack);
    }
    private void OnEnable() => stacks.Add(this);
    private void OnDisable()
    {
        stacks.Remove(this);
        if (interaction != null) interaction.isCurrentlyInteractable = false;
    }

    private void OnDestroy()
    {
        if (interaction != null) interaction.OnInteract.RemoveListener(InteractWithStack);
    }

    private bool HasBoat => dock.dockingManager != null && dock.dockingManager.HoldsBoat;

    private bool TryGetService(out IInventoryService service)
    {
        service = null;
        if (InventoryServiceLocator.Instance == null || !InventoryServiceLocator.Instance.TryGet(out service)) return false;

        if (service is InventoryService)
        {
            if (!warned)
                Debug.LogError("DockStackController: use StackInventoryService for this test scene. The original InventoryService has an index error in TryRemoveItem.", this);
            warned = true;
            return false;
        }
        return true;
    }

    private void LateUpdate()
    {
        interaction.isCurrentlyInteractable = Ready;
        if (dock.inventory == null) return;

        foreach (GameObject go in dock.inventory)
        {
            if (go == null) continue;
            WorldItemInteraction world = go.GetComponent<WorldItemInteraction>();

            if (world != null) world.SetStored(true);

            foreach (Interactable child in go.GetComponentsInChildren<Interactable>(true))
                child.isCurrentlyInteractable = false;

            Rigidbody rb = go.GetComponent<Rigidbody>();

            if (rb != null) rb.isKinematic = true;
        }
    }

    private bool Matches(IInventoryService service)
    {
        if (dock.inventory == null || service.Count() != dock.inventory.Count) return false;
        BaseItemData[] data = service.GetInventory();
        for (int i = 0; i < dock.inventory.Count; i++)
        {
            GameObject go = dock.inventory[i];
            if (go == null) return false;
            ItemInstance item = go.GetComponent<ItemInstance>();
            if (item == null || item.data == null || item.data != data[i]) return false;
        }
        return true;
    }

    private bool Ready => isActiveAndEnabled && HasBoat && placing == null && TryGetService(out var serivce) && Matches(serivce);

    public void InteractWithStack()
    {
        if (!Ready) return;
        if (carryController == null) carryController = FindFirstObjectByType<PlayerCarryController>();
        if (carryController == null || !CanReach(carryController.transform.position)) return;

        if (carryController.IsCarrying) carryController.TryReturnToStack(this);
        else carryController.TryPickupTop(this);
    }

    public bool Contains(Component item) =>
        item != null && dock.inventory != null && dock.inventory.Contains(item.gameObject);

    public static bool IsStored(Transform item)
    {
        foreach (DockStackController stack in stacks)
            if (stack != null && stack.Contains(item)) return true;
        return false;
    }

    public bool TryTakeTop(out ItemInstance item)
    {
        item = null;
        if (!Ready || dock.inventory.Count == 0 || !TryGetService(out var service)) return false;
        ItemInstance top = dock.inventory[dock.inventory.Count - 1].GetComponent<ItemInstance>();

        if (!service.TryRemoveItem(out _)) return false;
        dock.inventory.RemoveAt(dock.inventory.Count - 1);
        top.transform.SetParent(null, true);
        WorldItemInteraction world = top.GetComponent<WorldItemInteraction>();

        if (world != null) world.SetStored(false);
        item = top;
        return true;
    }

    public bool TryTake(Component item)
    {
        if (!Contains(item) || dock.inventory[dock.inventory.Count -1] != item.gameObject) return false;
        return TryTakeTop(out _);
    }

    public bool CanReach(Vector3 playerPosition)
    {
        Vector3 offset = transform.position - playerPosition;

        if (Mathf.Abs(offset.y) > returnRange) return false;
        offset.y = 0f;
        return offset.sqrMagnitude <= returnRange * returnRange;
    }

    public static DockStackController FindReturnTarget(Vector3 playerPosition, Vector3 forward)
    {
        DockStackController best = null;
        float bestDistance = float.PositiveInfinity;
        forward.y = 0f;

        foreach (DockStackController stack in stacks)
        {
            if (stack == null || !stack.isActiveAndEnabled || !stack.HasBoat || !stack.CanReach(playerPosition)) continue;
            Vector3 offset = stack.transform.position - playerPosition;

            offset.y = 0f;
            float distance = offset.sqrMagnitude;

            if (distance >= bestDistance) continue;
            if (distance > 0.0001f && Vector3.Angle(forward, offset) > stack.returnAngle) continue;

            best = stack;
            bestDistance = distance;
        }
        return best;
    }

    public bool TryPlace(ItemInstance item, out Vector3 position)
    {
        position = transform.position;
        if (!Ready || item == null || Contains(item) || !TryGetService(out var service)) return false;
        
        WorldItemInteraction world = item.GetComponent<WorldItemInteraction>();

        if (world == null || !world.IsCarried || world.IsStored || item.data == null || item.data.type == null) return false;

        if (dock.inventory.Count > 0)
            position = dock.inventory[dock.inventory.Count - 1].transform.position + Vector3.up * dock.heightDiff;
        if (!service.TryAddItem(item.data))
        {
            Debug.LogWarning("Inventory is full; the item stays in your hands.", this);
            return false;
        }

        dock.inventory.Add(item.gameObject);
        world.SetStored(true);
        placing = item;
        return true;
    }

    public void CompletePlacement(ItemInstance item, Vector3 position)
    {
        if (Contains(item))
        {
            item.transform.SetParent(transform, true);
            item.transform.SetPositionAndRotation(position, Quaternion.identity);
            WorldItemInteraction world = item.GetComponent<WorldItemInteraction>();
            if (world != null) world.SetCarried(false);

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            var obstacle = item.GetComponent<UnityEngine.AI.NavMeshObstacle>();

            if (obstacle != null) obstacle.enabled = true;
        }
        if (placing == item) placing = null;
    }
}
