using UnityEngine;

[CreateAssetMenu(fileName = "ItemType", menuName = "Scriptable Objects/ItemType")]
public class BaseItemType : ScriptableObject
{
    public string ItemName;


    [SerializeField]    
    private GameObject Visuals;
    public GameObject GetVisualsTemplate => Visuals;

    [Header("Overrides")]
    public GameObject overrideIslandObject;
    public GameObject overrideCaravanObject;
    public GameObject overrideFreeFloatObject;

}
