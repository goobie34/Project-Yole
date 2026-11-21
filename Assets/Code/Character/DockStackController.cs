using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DockInventory))]
public class DockStackController : MonoBehaviour
{
    [Min(0f)] public float returnRange = 2.5f;
    [Range(0f, 180f)] public float returnAngle = 75f;

    private static readonly List<DockStackController> stacks = new();
    private DockInventory dock;
    private PickupObject placing;
    private bool reportedProblem;

    private void Awake() => dock = GetComponent<DockInventory>();
    private void OnEnable() => stacks.Add(this);
    private void OnDisable() => stacks.Remove(this);

    private bool HasBoat => dock.dockingManager != null && dock.dockingManager.HoldsBoat;

    private bool TryGetService(out IInventoryService service)
    {
        service = null;
        if (InventoryServiceLocator.Instance == null || !InventoryServiceLocator.Instance.TryGet(out service)) return false;

        if (service is InventoryService)
        {
            if (!reportedProblem)
                Debug.LogError("DockStackController: use StackInventoryService for this test scene. The original InventoryService has an index error in TryRemoveItem.", this);
            reportedProblem = true;
            return false;
        }
        return true;
    }

    private void LateUpdate()
    {
        if (dock.inventory == null) return;
        bool valid = HasBoat && TryGetService(out var service) && Matches(service);

        for (int i = 0; i < dock.inventory.Count; i++)
        {
            GameObject go = dock.inventory[i];
            if (go == null) continue;
            PickupObject pickup = go.AddComponent<PickupObject>();
            if (pickup != null) pickup.SetDockStack(this);
            Interactable interactable = go.GetComponent<Interactable>();
            if (interactable != null)
                interactable.isCurrentlyInteractable = valid && placing == null && i == dock.inventory.Count - 1 && pickup != null && !pickup.IsCarried;

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
            ItemInstance instance = go.GetComponent<ItemInstance>();
            if (instance == null || instance.data == null || instance.data != data[i]) return false;
        }
        return true;
    }

    public bool Contains(PickupObject item) =>
        item != null && dock.inventory != null && dock.inventory.Contains(item.gameObject);

    public bool CanTake(PickupObject item)
    {
        return isActiveAndEnabled && HasBoat && placing == null && Contains(item) && dock.inventory[dock.inventory.Count - 1] == item.gameObject && TryGetService(out var service) && Matches(service);
    }

    public bool TryTake(PickupObject item)
    {
        if (!CanTake(item) || !TryGetService(out var service)) return false;
        if (!service.TryRemoveItem(out _)) return false;
        dock.inventory.RemoveAt(dock.inventory.Count - 1);
        item.SetDockStack(null);
        item.transform.SetParent(null, true);
        return true;
    }

    public static DockStackController FindReturnTarget(Vector3 playerPosition, Vector3 forward)
    {
        DockStackController best = null;
        float bestDistance = float.PositiveInfinity;
        forward.y = 0f;

        foreach (DockStackController stack in stacks)
        {
            if (stack == null || !stack.isActiveAndEnabled || !stack.HasBoat) continue;
            Vector3 offset = stack.transform.position - playerPosition;

            //Avoid selecting a stack on a different floor
            if (Mathf.Abs(offset.y) > stack.returnRange) continue;
            offset.y = 0f;
            float distance = offset.sqrMagnitude;
            if (distance > stack.returnRange * stack.returnRange || distance >= bestDistance) continue;
            if (distance > 0.0001f && Vector3.Angle(forward,offset) > stack.returnAngle) continue;
            best = stack;
            bestDistance = distance;
        }
        return best;
    }

    public bool TryPlace(PickupObject item, out Vector3 position)
    {
        position = transform.position;
        if (!isActiveAndEnabled || !HasBoat || placing != null || item == null || !item.IsCarried || Contains(item) || !TryGetService(out var service) || !Matches(service)) return false;
        
        ItemInstance instance = item.GetComponent<ItemInstance>();

        if (instance == null || instance.data == null || instance.data.type == null)
        {
            Debug.LogWarning("This object needs ItemInstance with valid data before it can enter inventory.", item);
            return false;
        }

        if (dock.inventory.Count > 0)
            position = dock.inventory[dock.inventory.Count - 1].transform.position + Vector3.up * dock.heightDiff;

        if (!service.TryAddItem(instance.data))
        {
            Debug.LogWarning("Inventory is full. The object stays in your hands.", this);
            return false;
        }
        dock.inventory.Add(item.gameObject);
        item.SetDockStack(this);
        placing = item;
        return true;
    }

    public void CompletePlacement(PickupObject item, Vector3 position)
    {
        if (Contains(item))
        {
            item.transform.SetParent(transform, true);
            item.transform.SetPositionAndRotation(position, Quaternion.identity);
            item.SetCarried(false);

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            var obstacle = item.GetComponent<UnityEngine.AI.NavMeshObstacle>();

            if (obstacle != null) obstacle.enabled = true;
        }
        if (placing == item) placing = null;
    }
}
