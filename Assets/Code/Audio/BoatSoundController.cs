using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BuoyancyController))]
[RequireComponent(typeof(SailingForceControler))]
public class BoatSoundController : MonoBehaviour
{
    [SerializeField]
    EventReference eventReference;
    EventInstance eventInstance;

    [SerializeField]
    EventReference splashEvent;

    PARAMETER_ID velocityParameterID;
    PARAMETER_ID submersionParameterID;
    PARAMETER_ID sailTensionParameterID;
    PARAMETER_ID submersionSpeedParameterID;

    [Header("FMOD Parameter Names")]
    [SerializeField] string velocityParameterString;
    [SerializeField] string submersionParameterString;
    [SerializeField] string submersionSpeedParameterString;
    [SerializeField] string sailTensionParameterString;

    [Header("FMOD Parameter Settings")]
    [SerializeField] float maxVelocity;
    [SerializeField] float minSubmersion, maxSubmersion;
    [SerializeField] float maxSubmersionSpeed;
    bool wasSubmerged = true;

    [Header("FMOD Parameter Values (read only)")]
    [SerializeField] float fmodVelocity;
    [SerializeField] float fmodSubmersion;
    [SerializeField] float fmodSubmersionSpeed;
    [SerializeField] float fmodSailTension;

    //Boat Components to calculate FMOD parameter values from
    Rigidbody rb;
    BuoyancyController bc;
    SailingForceControler sfc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BuoyancyController>();
        sfc = GetComponent<SailingForceControler>();
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, this.gameObject);

        //Using parameter IDs performs better than using parameter names (string)
        //Especially for paramters that are updated every frame
        velocityParameterID = AudioManager.GetParameterID(eventInstance, velocityParameterString);
        submersionParameterID = AudioManager.GetParameterID(eventInstance, submersionParameterString);
        sailTensionParameterID = AudioManager.GetParameterID(eventInstance, sailTensionParameterString);
        submersionSpeedParameterID = AudioManager.GetParameterID(eventInstance, submersionSpeedParameterString);
    }

    private void Start()
    {
        eventInstance.start();
    }

    private void FixedUpdate()
    {
        eventInstance.setParameterByID(velocityParameterID, CalculateVelocity());
        eventInstance.setParameterByID(submersionParameterID, CalculateSubmersion());
        eventInstance.setParameterByID(sailTensionParameterID, CalculateSailTension());

        if (bc.submergedFrac == 0)
            wasSubmerged = false;
        else if (bc.submergedFrac > 0 && !wasSubmerged)
        {
            //eventInstance.setParameterByID(submersionSpeedParameterID, CalculateSubmersionSpeed());
            //float paramVal = CalculateSubmersionSpeed();
            wasSubmerged = true;
            AudioManager.PlayOneShotWithParameters(splashEvent, this.gameObject.transform.position, (submersionParameterID, CalculateSubmersionSpeed()));
        }
    }

    private float CalculateVelocity()
    {
        Vector3 vel = rb.linearVelocity;
        float paramVal =  new Vector2(vel.x, vel.z).magnitude / maxVelocity;
        paramVal = Mathf.Clamp01(paramVal);

        fmodVelocity = paramVal;
        return paramVal;
    }
    private float CalculateSubmersion()
    {
        float paramVal = bc.submergedFrac;
        paramVal = Mathf.Clamp01(paramVal);

        fmodSubmersion = paramVal;
        return paramVal;
    }
    private float CalculateSubmersionSpeed()
    {
        Vector3 vel = rb.linearVelocity;
        if (vel.y >= 0) return 0;

        float paramVal = Mathf.Abs(vel.y) / maxSubmersionSpeed;
        paramVal = Mathf.Clamp01(paramVal);

        fmodSubmersionSpeed = paramVal;
        return paramVal;
    }

    public float CalculateSailTension()
    {
        float paramVal =  Mathf.Abs(sfc.sailWindDot);
        
        paramVal = Mathf.Clamp01(paramVal);

        fmodSailTension = paramVal;
        return paramVal;
    }
    private void OnDestroy()
    {
        AudioManager.CleanUp(eventInstance);
    }
}
