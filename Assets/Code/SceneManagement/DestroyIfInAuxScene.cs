using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyIfInAuxScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (gameObject.scene != SceneManager.GetActiveScene())
        {
            Destroy(gameObject);
            return;
        }

        Destroy(this);


    }

    
}
