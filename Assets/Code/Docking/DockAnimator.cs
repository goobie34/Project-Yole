using UnityEngine;

public class DockAnimator : MonoBehaviour
{

    public Animator anchorSign;
    public DockingManager manager;

    private bool boatCollider;
    private bool dockHasBoat;
    private bool showSign => boatCollider && !dockHasBoat;

    private void Start()
    {
        
       

        if(manager == null)
            manager = GetComponent<DockingManager>();
    }

    public void Update()
    {
        dockHasBoat = manager.HoldsBoat;

        anchorSign?.SetBool("Enabled",showSign);
    }

    public void OnTriggerEnter(Collider other)
    {
        var boat = other.GetComponentInParent<PlayerBoatInstanceHandeler>();

        if (boat == null)
            return;
        boatCollider = true;
    }

    public void OnTriggerExit(Collider other)
    {
        var boat = other.GetComponentInParent<PlayerBoatInstanceHandeler>();

        if (boat == null)
            return;

        boatCollider = false;

    }

}
