using UnityEngine;
using UnityEngine.InputSystem;

public class AccelInputDriverTest : MonoBehaviour
{
    public float accelTime = 1;
    public float decelTime = 1;

    public float current = 0;
    public float targetInputValue = 0;


    // Update is called once per frame
    void Update()
    {
        LerpHelper.InputSpeedAccelerationDriver(ref current, accelTime, decelTime, targetInputValue, Time.deltaTime);
    }

    private void OnExtendSail(InputValue input)
    {
        targetInputValue = input.Get<float>();

    }
}
