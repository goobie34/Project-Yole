using UnityEngine;

public class ItemFactoryService : MonoBehaviour, IItemFactoryService
{
    public GameObject IslandItemTemplatePrefab;
    public GameObject CaravanItemTemplatePrefab;
    public GameObject FloatingItemTemplatePrefab;

    public void Start()
    {
        ItemFactoryServiceLocator.Instance.Register(this);
    }

    public ItemInstance InstantiateLandItem(BaseItemData data,Transform destination, Rigidbody refRB = null)
    {
        if(data.type.overrideIslandObject != null)
        {
            return CreateGenericItem(data.type.overrideIslandObject, data, destination, refRB);
        }

        return CreateGenericItem(IslandItemTemplatePrefab, data, destination, refRB);
    }

    public ItemInstance InstantiateCaravanItem(BaseItemData data, Transform destination, Rigidbody refRB = null)
    {
        if (data.type.overrideCaravanObject != null)
        {
            return CreateGenericItem(data.type.overrideCaravanObject, data, destination, refRB);
        }

        return CreateGenericItem(CaravanItemTemplatePrefab, data, destination, refRB);
    }

    public ItemInstance InstantiateLooseFloatingItem(BaseItemData data, Transform destination, Rigidbody refRB = null)
    {
        if (data.type.overrideFreeFloatObject != null)
        {
            return CreateGenericItem(data.type.overrideFreeFloatObject, data, destination, refRB);
        }

        return CreateGenericItem(FloatingItemTemplatePrefab, data, destination, refRB);
    }

    public ItemInstance InstantiateGenericItem(GameObject template, BaseItemData data, Transform destination, Rigidbody refRB = null)
    {
        return CreateGenericItem(template, data, destination, refRB);
    }


    public ItemInstance CreateGenericItem(GameObject template, BaseItemData data, Transform destination, Rigidbody refRB = null)
    {
        if(template == null)
            return null;

        if (data == null)
            return null;

        var obj = Instantiate(template, destination);

        var itemInstance = obj.GetComponent<ItemInstance>();
        
        if(itemInstance == null)
        {
            itemInstance = obj.AddComponent<ItemInstance>();
        }


        var visuals = Instantiate(data.GetVisualsPrefabTemplate(), itemInstance.GetVisualsHolder);


        itemInstance.data = data;

        return itemInstance;
    }

}
