using Unity.Cinemachine;
using UnityEngine;

public static class LerpHelper
{

    // based on CinemachineInputAxisDriver
    public static void InputSpeedAccelerationDriver(ref float currentSpeed, float accelTime, float decelTime, float input, float deltaTime)
    {
        float speed = input;

        float dampTime = Mathf.Abs(speed) < Mathf.Abs(currentSpeed) ? decelTime : accelTime;
        speed = currentSpeed + Damper.Damp(speed - currentSpeed, dampTime, deltaTime);
        currentSpeed = speed;

       
                
    }


}
