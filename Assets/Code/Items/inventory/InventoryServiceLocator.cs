using UnityEngine;
using UnityEngine.Events;

public class InventoryServiceLocator : ServiceLocator<IInventoryService>
{
   
}

public interface IInventoryService
{
    public BaseItemData[] GetInventory();

    public int Count();
    public int MaxCapacity();


    public bool TryAddItem(BaseItemData item);
    public bool TryRemoveItem(out BaseItemData item);


}
