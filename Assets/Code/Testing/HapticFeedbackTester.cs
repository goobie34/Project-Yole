using UnityEngine;
using UnityEngine.InputSystem;

public class HapticFeedbackTester : MonoBehaviour
{
    public float high;
    public float low;

    public bool update;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void Update()
    {
        high = Mathf.Clamp01(high);
        low = Mathf.Clamp01(low);
        
        if(update)
            Gamepad.current?.SetMotorSpeeds(high, low);

    }


    private void OnDisable()
    {
        Gamepad.current?.SetMotorSpeeds(0, 0);
    }
}
