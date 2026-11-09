using UnityEngine;

public class PlaceholderDockPlayerTrigger : MonoBehaviour
{
    public IslandPlayerTrigger islandPlayerTrigger;
    public DockingManager dockingManager;

    public GameObject visuals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Start()
    {
        if(dockingManager == null)
            dockingManager = GetComponentInParent<DockingManager>();
    }

    private void OnEnable()
    {
        if (islandPlayerTrigger == null)
            islandPlayerTrigger = GetComponent<IslandPlayerTrigger>();

        islandPlayerTrigger.EVENT_OnPlayerEnter.AddListener(OnEventPlayerEnter);
        islandPlayerTrigger.EVENT_OnPlayerExit.AddListener(OnEventPlayerExit);

    }


    private void OnDisable()
    {
        if (islandPlayerTrigger == null)
            islandPlayerTrigger = GetComponent<IslandPlayerTrigger>();

        islandPlayerTrigger.EVENT_OnPlayerEnter.RemoveListener(OnEventPlayerEnter);
        islandPlayerTrigger.EVENT_OnPlayerExit.RemoveListener(OnEventPlayerExit);

    }

    private void FixedUpdate()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (visuals == null)
            return;

        if (dockingManager == null) 
        {
            visuals.SetActive(false);
            return;
        }

        visuals.SetActive(dockingManager.HoldsBoat);
    }

    void OnEventPlayerEnter(GameObject go)
    {
        TryEnterBoat();
    }
    void OnEventPlayerExit(GameObject go)
    {

    }


    void TryEnterBoat()
    {
        if (dockingManager == null) return;

        dockingManager.TryUndockBoat();
    }
}
