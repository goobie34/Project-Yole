using UnityEngine;

public class ToggleBasedOnCameraDist : MonoBehaviour
{
    public Transform mainCam;
    public GameObject child;
    public float ToggleDist = 1000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var vector = mainCam.position - transform.position;
        bool toggle = vector.sqrMagnitude < (ToggleDist * ToggleDist);
        child?.SetActive(toggle);
    }




}
