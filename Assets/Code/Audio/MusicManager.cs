using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
/// <summary>
/// Manages music event instances. (Currently only 1).
/// 
/// WIP. IS NOT IN FUNCTIONAL STATE.
/// </summary>
public class MusicManager : SingletonPersistent<MusicManager>
{
    //temp
    [SerializeField] EventReference defaultMusic;
    public EventReference queuedMusic;

    EventInstance musicInstance;
    EVENT_CALLBACK musicStoppedCallback;
    private void Start()
    {
        musicStoppedCallback = new EVENT_CALLBACK(MusicStoppedCallback);
    }
    public void ChangeMusic(EventReference reference)
    {
        musicInstance.setCallback(musicStoppedCallback, EVENT_CALLBACK_TYPE.STOPPED);

        queuedMusic = reference;
        StopMusic(true);

        musicInstance.setCallback(musicStoppedCallback, EVENT_CALLBACK_TYPE.STOPPED);

    }

    public void StartMusic() => musicInstance.start();
    public void StopMusic(bool fadeOut = true) => musicInstance.stop(fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
    public void PauseMusic() => musicInstance.setPaused(true);
    public void ResumeMusic() => musicInstance.setPaused(false);

    static FMOD.RESULT MusicStoppedCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        Instance.musicInstance.release();
        Instance.musicInstance = RuntimeManager.CreateInstance(Instance.queuedMusic);

        Debug.Log("MUSIC STOPPED NOW");
        return FMOD.RESULT.OK;
    }
    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
}
