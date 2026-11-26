using UnityEngine;
using System.Collections.Generic;
public class CaravanManager : MonoBehaviour
{
    

    public List<BaseItemData> caravanInventory;

    private Transform caravanItemsHolder;

    [Header("Dependencies")]
    public CaravanPhysManager caravanPhysManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        caravanItemsHolder = new GameObject("caravanItems").transform;

        caravanInventory = new List<BaseItemData>();




        LoadInventory();

        SpawnCaravan();

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
            caravanInventory.Add(inventory[i]);
        }
        
    }

    private void OnDestroy()
    {
       

        if(caravanItemsHolder != null)
            Destroy(caravanItemsHolder.gameObject);
    }

    private void SpawnCaravan()
    {
        if (ItemFactoryServiceLocator.Instance == null) return;

        if (!ItemFactoryServiceLocator.Instance.TryGet(out var service)) return;

        for(int i = 0;i < caravanInventory.Count; i++)
        {
            var obj = service.InstantiateCaravanItem(caravanInventory[i], caravanItemsHolder);
            caravanPhysManager?.AddItemToSnake(obj.GetComponent<Rigidbody>());
        }

    }

    
}
