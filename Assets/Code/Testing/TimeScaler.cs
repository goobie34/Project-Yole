using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public float timescale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timescale;
    }
}
