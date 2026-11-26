using UnityEngine;

[CreateAssetMenu(fileName = "BaseItemData", menuName = "Scriptable Objects/BaseItemData")]
public class BaseItemData : ScriptableObject
{
    public BaseItemType type;

    public GameObject GetVisualsPrefabTemplate()
    {
        return type.GetVisualsTemplate;
    }

}
