using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using FMOD;
/// <summary>
/// Manages FMOD banks, FMOD mix buses, and contains helper functions for FMOD playback.
/// </summary>
public class AudioManager : SingletonPersistent<AudioManager>
{
    [SerializeField] private MusicManager musicManager;
    protected override void Awake()
    {
        base.Awake();
    }

    public static void CleanUp(EventInstance instance) { instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); instance.release(); }
    public static bool CompareGUID(EventInstance instance, EventReference reference)
    {
        return GetGUID(instance) == reference.Guid;
    }

    public static EventDescription GetEventDescription(EventInstance instance)
    {
        EventDescription description;
        instance.getDescription(out description);
        return description;
    }
    public static GUID GetGUID(EventInstance instance)
    {
        GUID guid;
        GetEventDescription(instance).getID(out guid);
        return guid;
    }

    public static PARAMETER_DESCRIPTION GetParameterDescription(EventInstance instance, string parameterName)
    {
        PARAMETER_DESCRIPTION description;
        GetEventDescription(instance).getParameterDescriptionByName(parameterName, out description);
        return description;
    }
    public static PARAMETER_ID GetParameterID(EventInstance instance, string parameterName)
    {
        return GetParameterDescription(instance, parameterName).id;
    }

    



    //    [SerializeField] bool debug = true;    

    //    Dictionary<EventReference, EventInstance> eventInstances;

    //public bool CreateInstance(EventReference reference)
    //{
    //    EventInstance instance = RuntimeManager.CreateInstance(reference);
    //    if (!eventInstances.TryAdd(reference, instance))
    //        return false;

    //    Log("FMOD EventInstance created for EventReference: " + reference.ToString());

    //    return true;
    //}

    //public bool StartInstance(EventReference reference)
    //{
    //    EventInstance instance;
    //    if (!eventInstances.TryGetValue(reference, out instance))
    //        return false;

    //    instance.start();

    //    return true;
    //}

    //public bool StopInstance(EventReference reference, bool allowFade = true)
    //{
    //    EventInstance instance;
    //    if (!eventInstances.TryGetValue(reference, out instance))
    //        return false;

    //    instance.stop(allowFade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);

    //    return true;
    //}
    //public bool RemoveInstance(EventReference reference)
    //{
    //    EventInstance instance;
    //    if (!eventInstances.TryGetValue(reference, out instance))
    //        return false;

    //    instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //    instance.release();
    //    eventInstances.Remove(reference);

    //    Log("FMOD EventInstance removed for EventReference: " + reference.ToString());

    //    return true;
    //}

    //public bool SetInstanceParameter(EventReference reference, string param, float val)
    //{
    //    EventInstance instance;
    //    if (!eventInstances.TryGetValue(reference, out instance))
    //        return false;

    //    instance.setParameterByName(param, val);

    //    return true;
    //}
    //public bool SetInstanceParameter(EventReference reference, string param, string label)
    //{
    //    EventInstance instance;
    //    if (!eventInstances.TryGetValue(reference, out instance))
    //        return false;

    //    instance.setParameterByNameWithLabel(param, label);

    //    return true;
    //}

    //public bool PauseInstance(EventReference reference, bool pauseState)
    //{
    //    EventInstance instance;
    //    if (!eventInstances.TryGetValue(reference, out instance))
    //        return false;

    //    instance.setPaused(pauseState);

    //    return true;
    //}

    //private void OnDestroy()
    //{
    //    foreach(EventInstance instance in eventInstances.Values)
    //    {
    //        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //        instance.release();
    //    }
    //}

    //public void Log(string message)
    //{
    //    if (debug) Debug.Log(message);
    //}
}
