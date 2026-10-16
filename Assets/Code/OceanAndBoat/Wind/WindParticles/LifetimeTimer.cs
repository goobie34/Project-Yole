using UnityEngine;

public class LifetimeTimer : MonoBehaviour
{

    public float lifetime = 1;

    public bool dropChildren;

    public void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if(lifetime < 0 )
            Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (!transform.parent.gameObject.activeInHierarchy)
            return;

        for ( int i = 0; i < transform.childCount; i++)
        {
            

            transform.GetChild(i).SetParent(transform.parent);
        }
        
    }


}
