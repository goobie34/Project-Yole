using UnityEngine;

public class SimpleIslandItemSpawner : MonoBehaviour
{
    public BaseItemType type;

    private BaseItemData createdData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createdData = new BaseItemData();
        createdData.type = type;

        SpawnItem();
    }

    public void SpawnItem()
    {
        if(ItemFactoryServiceLocator.Instance == null) return;

        if (!ItemFactoryServiceLocator.Instance.TryGet(out var service)) return;

        var creation = service.InstantiateLandItem(createdData, transform);

        creation.transform.position = transform.position;
        creation.transform.rotation = transform.rotation;
        
    }
}
