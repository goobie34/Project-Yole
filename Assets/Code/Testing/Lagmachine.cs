using System.Threading;
using UnityEngine;

public class Lagmachine : MonoBehaviour
{
    public int time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Thread.Sleep(time);
    }
}
