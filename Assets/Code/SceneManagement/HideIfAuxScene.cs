using UnityEngine;
using UnityEngine.SceneManagement;

public class HideIfAuxScene : MonoBehaviour
{
    [SerializeField] private int layerIfAux = 6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(gameObject.scene != SceneManager.GetActiveScene())
        {
            gameObject.layer = layerIfAux;
        }        
    }

}
