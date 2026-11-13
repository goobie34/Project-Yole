using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    public BaseItemData data;

    public Transform visualsHolder;
    public Transform GetVisualsHolder => visualsHolder == null? transform : visualsHolder;
}
