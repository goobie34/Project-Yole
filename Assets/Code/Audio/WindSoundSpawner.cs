using UnityEngine;
using FMODUnity;

public class WindSoundSpawner : MonoBehaviour
{
    [SerializeField]
    float cooldown;
    float timer;

    [SerializeField]
    float variance;

    [SerializeField]
    float maxDistance;
    [SerializeField]
    float minDistance;

    [SerializeField]
    EventReference eventReference;

    [SerializeField]
    GameObject listenerObject;
    private void Start()
    {
        timer = cooldown;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            PlaySound(listenerObject.transform.position + GetRandomWindOffset(maxDistance, minDistance));
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timer = cooldown + variance * (Random.value * 2 - 1);
    }

    private void PlaySound(Vector3 pos)
    {
        Debug.Log("SOUND PLAYED");
        RuntimeManager.PlayOneShot(eventReference, pos);
    }

    private Vector3 GetRandomWindOffset(float maxDist, float minDist = 0)
    {
        Vector2 direction = Random.insideUnitCircle;
        float diff = maxDist - minDist;
        float distance = minDist + diff * Random.value;
        Vector2 offset2D = direction * distance;
        Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);
        return offset;
    }
}
