using UnityEngine;

public class ItemFactoryServiceLocator : ServiceLocatorPersistent<IItemFactoryService>
{
    
}

public interface IItemFactoryService
{
    public ItemInstance InstantiateLandItem(BaseItemData data, Transform destination, Rigidbody refRB = null);

    public ItemInstance InstantiateCaravanItem(BaseItemData data, Transform destination, Rigidbody refRB = null);

    public ItemInstance InstantiateLooseFloatingItem(BaseItemData data, Transform destination, Rigidbody refRB = null);

    public ItemInstance InstantiateGenericItem(GameObject template, BaseItemData data, Transform destination, Rigidbody refRB = null);

}
