using UnityEngine;

public class InventoryService : MonoBehaviour, IInventoryService 
{
    public int startingMaxItemCount = 4;
    public BaseItemType[] testStartingArray;

    private BaseItemData[] items;
    private int itemCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        items = new BaseItemData[startingMaxItemCount];

        for(int i = 0; i < Mathf.Min(testStartingArray.Length, items.Length); i++)
        {
            var newItem = ScriptableObject.CreateInstance<BaseItemData>();
            newItem.type = testStartingArray[i];

            TryAddItem(newItem);
        }
    }

    public void Start()
    {
        InventoryServiceLocator.Instance.Register(this);


    }



    public BaseItemData[] GetInventory()
    {
        return items;
    }

    public int Count()
    {
        return itemCount;
    }
    public int MaxCapacity()
    {
        return items.Length;
    }


    public bool TryAddItem(BaseItemData item)
    {
        if (Count() >= MaxCapacity())
            return false;

        items[itemCount++] = item;
        return true;
    }
    public bool TryRemoveItem(out BaseItemData item)
    {
        item = null;

        if (Count() <= 0)
            return false;

        item = items[itemCount--];
        items[itemCount] = null;

        return true;
    }

}
