using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages music event instances and music playback. By default it lets music event instances fade out according to FMOD's ADHRS envelope.
/// </summary>
public class MusicManager : SingletonPersistent<MusicManager>
{
    [SerializeField] EventReference startupMusicEvent;

    EventInstance currentMusicInstance;
    List<EventInstance> fadingMusicInstances;

    public EventInstance CurrentMusic { get{ return currentMusicInstance; }}
    private void Start()
    {
        fadingMusicInstances = new List<EventInstance>();

        if (!startupMusicEvent.IsNull)
        {
            ChangeMusic(startupMusicEvent);
        }
    }
    public void ChangeMusic(EventReference reference)
    {
        if (currentMusicInstance.isValid())
        {
            fadingMusicInstances.Add(currentMusicInstance);
            StartCoroutine(FadeOutAndRelease(currentMusicInstance));
        }

        currentMusicInstance = RuntimeManager.CreateInstance(reference);
        currentMusicInstance.start();
    }

    public void StartMusic() => currentMusicInstance.start();
    public void PauseMusic() => currentMusicInstance.setPaused(true);
    public void ResumeMusic() => currentMusicInstance.setPaused(false);
    public void StopMusic(EventReference reference, bool fadeOut = true)
    {
        if (AudioManager.CompareGUID(currentMusicInstance, reference))
        {
            StopMusicInstance(currentMusicInstance, fadeOut);
        }
    }
    public void StopMusic(bool fadeOut = true)
    {
        StopMusicInstance(currentMusicInstance, fadeOut);
    }
    private void StopMusicInstance(EventInstance instance, bool fadeOut = true)
    {
        if (!instance.isValid()) return;

        if (fadeOut)
        {
            fadingMusicInstances.Add(instance);
            StartCoroutine(FadeOutAndRelease(instance));
        }
        else
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    private IEnumerator FadeOutAndRelease(EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        while (true)
        {
            if (!instance.isValid())
                break;

            PLAYBACK_STATE state;
            instance.getPlaybackState(out state);
            if (state == PLAYBACK_STATE.STOPPED)
                break;

            yield return new WaitForSeconds(0.1f);
        }
        
        if (instance.isValid())
        {
            instance.release();
            fadingMusicInstances.Remove(instance);
        }
    }

    public bool SetParameter(EventReference reference, PARAMETER_ID id, float val)
    {
        if (AudioManager.GetGUID(currentMusicInstance) != reference.Guid)
            return false;
        
        currentMusicInstance.setParameterByID(id, val);
        return true;
    }
    public bool SetParameter(EventReference reference, PARAMETER_ID id, string label)
    {
        if (AudioManager.GetGUID(currentMusicInstance) != reference.Guid)
            return false;
        
        currentMusicInstance.setParameterByIDWithLabel(id, label);
        return true;
    }

    void OnDestroy()
    {
        currentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentMusicInstance.release();

        foreach(var instance in fadingMusicInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }
}
