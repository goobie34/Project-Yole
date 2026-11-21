using UnityEngine;

public class TrejectoryTest : MonoBehaviour
{
    public float offsetDiffMult = 0.5f, offsetAdditionalHeight = 1f;

    public Vector3 uppVector = Vector3.up;

    public Transform start;
    public Transform end;

    public float duration = 5f;
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float time = (Time.time % duration) / duration;

       var pos = TrejectoryLerpHelper.LerpParabolaTrejectory(start.position, end.position, time, uppVector, offsetDiffMult, offsetAdditionalHeight);

        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        if(start == null || end == null) return;

        int count = 30;
        Gizmos.color = Color.red;

        for (int i = 0; i < count; i++)
        {
            float time = i / (float)count;

            var pos = TrejectoryLerpHelper.LerpParabolaTrejectory(start.position, end.position, time, uppVector, offsetDiffMult, offsetAdditionalHeight);

            

            Gizmos.DrawSphere(pos, 0.1f);
        }
        Gizmos.color = Color.white;
    }

}
