using FMODUnity;
using UnityEngine;

/// <summary>
/// Music trigger using OnTriggerEnter and OnTriggerExit.
/// </summary>
[RequireComponent (typeof(BoxCollider))]
public class MusicTriggerScript : MonoBehaviour
{
    [SerializeField] EventReference eventReference;
    [SerializeField] string triggerTag;
    TagHandle tagHandle;

    //Using TagHandle instead of string for performance
    //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Component.CompareTag.html
    private void Start()
    {
        if (MusicManager.Instance == null)
            throw new System.Exception("MusicManager singleton instance cannot be found.");
    }
    private void OnEnable()
    {
        tagHandle = TagHandle.GetExistingTag(triggerTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(tagHandle)) return;

        MusicManager.Instance.ChangeMusic(eventReference);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(tagHandle)) return;

        MusicManager.Instance.StopMusic(eventReference);
    }
}
