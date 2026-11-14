using System.Collections.Generic;
using UnityEngine;

public class DockInventory : MonoBehaviour
{
    public DockingManager dockingManager;

    public List<GameObject> inventory;

    public GameObject templateGameObject;

    public float heightDiff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(dockingManager == null) dockingManager = GetComponentInParent<DockingManager>();

        dockingManager.EVENT_DockBoat.AddListener(OnEventDockBoat);
        dockingManager.EVENT_UndockBoat.AddListener(OnEventUndockBoat);

    }

    private void OnDestroy()
    {
        dockingManager?.EVENT_DockBoat.AddListener(OnEventDockBoat);
        dockingManager?.EVENT_UndockBoat.AddListener(OnEventUndockBoat);
    }

    private void OnEventDockBoat()
    {
        Clear();
        LoadInventory();
    }
    private void OnEventUndockBoat()
    {
        Clear();
    }

    private void LoadInventory()
    {
        if (InventoryServiceLocator.Instance == null)
            return;

        if (!InventoryServiceLocator.Instance.TryGet(out var service))
            return;

        var inventory = service.GetInventory();

        for(int i = 0; i < service.Count(); i++)
        {
            LoadItem(inventory[i]);
        }
    }

    private void LoadItem(BaseItemData item)
    {
        if(ItemFactoryServiceLocator.Instance == null)
            return;

        if (!ItemFactoryServiceLocator.Instance.TryGet(out var service))
            return;

        var newItemGO = service.InstantiateGenericItem(templateGameObject, item, transform).gameObject;

        Transform targetTransform = transform;

        float heightDiff = 0;

        if (inventory.Count != 0)
        {
            targetTransform = inventory[inventory.Count - 1].transform;
            heightDiff = this.heightDiff;

        }

        newItemGO.transform.position = targetTransform.position + Vector3.up * heightDiff;

        inventory.Add(newItemGO);


    }

    

    private void Clear()
    {
        foreach (GameObject go in inventory)
        {
            Destroy(go);
        }

        inventory.Clear();
    }
}
