using UnityEngine;

[DefaultExecutionOrder(-200)]
public class StackInventoryService : MonoBehaviour, IInventoryService
{
    [Min(0)] public int startingMaxItemCount = 4;
    public BaseItemType[] testStartingArray;

    private BaseItemData[] items;
    private int itemCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        items = new BaseItemData[Mathf.Max(0,startingMaxItemCount)];
        if (testStartingArray == null) return;

        foreach (BaseItemType type in testStartingArray)
        {
            if (itemCount == items.Length) break;
            if (type == null) continue;
            BaseItemData data = ScriptableObject.CreateInstance<BaseItemData>();
            data.type = type;
            TryAddItem(data);
        }
    }

    public void Start()
    {
        InventoryServiceLocator.Instance.Register(this);


    }



    public BaseItemData[] GetInventory() => items;

    public int Count() => itemCount;
    public int MaxCapacity() => items.Length;


    public bool TryAddItem(BaseItemData item)
    {
        if (item == null || itemCount >= items.Length) return false;
        items[itemCount++] = item;
        return true;
    }
    public bool TryRemoveItem(out BaseItemData item)
    {
        item = null;

        if (itemCount == 0 ) return false;
        int index = --itemCount;
        item = items[index];
        items[index] = null;
        return true;
    }
}
