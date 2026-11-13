using UnityEngine;
using System.Collections.Generic;
public class CaravanManager : MonoBehaviour
{
    public List<BaseItemType> testLoadingArray;

    public List<BaseItemData> caravanInventory;

    private Transform caravanItemsHolder;

    [Header("Dependencies")]
    public CaravanPhysManager caravanPhysManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        caravanItemsHolder = new GameObject("caravanItems").transform;

        caravanInventory = new List<BaseItemData>();



        for(int i = 0; i < testLoadingArray.Count; i++)
        {
            var newItem = new BaseItemData();
            newItem.type = testLoadingArray[i];
            caravanInventory.Add(newItem);
        }


        SpawnCaravan();

    }

    private void OnDestroy()
    {
        if(caravanItemsHolder.gameObject != null)
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
