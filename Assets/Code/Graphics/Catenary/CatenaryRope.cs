using UnityEngine;
using UnityEngine.UIElements;

public class CatenaryRope : MonoBehaviour
{
    public Transform otherTest;

    public float a = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Vector2 p1 = new Vector2();

        for(int i = 0; i < 10; i++)
        {
            
        }
    }
}

public static class CatenaryHelper
{
    public static float GetCatenaryHeight(float a, Vector2 p1, Vector2 p2, float lerp)
    {
        float d = p2.x - p1.x;

        float x = p1.x + d * lerp;
        float y = a * Unity.Mathematics.math.cosh((x) / a);

        return 0;
    }
}
