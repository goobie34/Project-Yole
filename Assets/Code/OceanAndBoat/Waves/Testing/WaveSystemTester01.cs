using UnityEngine;

public class WaveSystemTester01 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(WaveServiceLocator.Instance.ContainsService());       
    }

    
}
