using FMODUnity;
using UnityEngine;

/// <summary>
/// Temporary music trigger using OnTriggerEnter and Exit.
/// </summary>
[RequireComponent (typeof(BoxCollider))]
public class MusicTriggerScript : MonoBehaviour
{
    [SerializeField] EventReference eventReference;

    //Using TagHandle instead of string for performance
    //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Component.CompareTag.html
    TagHandle playerTag;

    private void OnEnable()
    {
        playerTag = TagHandle.GetExistingTag("player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(playerTag)) return;

        MusicManager.Instance.ChangeMusic(eventReference);
        MusicManager.Instance.StartMusic();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(playerTag)) return;

        MusicManager.Instance.StopMusic();
    }
}
