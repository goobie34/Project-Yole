using UnityEngine;
using FMODUnity;
using FMOD.Studio;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BuoyancyController))]
public class BoatSoundController : MonoBehaviour
{
    [SerializeField]
    EventReference eventReference;
    EventInstance eventInstance;

    PARAMETER_ID velocityParameterID;
    PARAMETER_ID submersionParameterID;

    [Header("FMOD Parameter Names")]
    [SerializeField] string velocityParameterString;
    [SerializeField] string submersionParameterString;

    [Header("FMOD Parameter Data")]
    [SerializeField] float maxVelocity;
    [SerializeField] float minSubmersion, maxSubmersion;

    [SerializeField]
    bool debugFMODParameterValues;

    //Boat Components to calculate FMOD parameter values from
    Rigidbody rb;
    BuoyancyController bc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BuoyancyController>();
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, this.gameObject);

        //Using parameter IDs performs better than using parameter names (string)
        //Especially for paramters that are updated every frame
        velocityParameterID = AudioManager.GetParameterID(eventInstance, velocityParameterString);
        submersionParameterID = AudioManager.GetParameterID(eventInstance, submersionParameterString);
    }

    private void Start()
    {
        eventInstance.start();
    }

    private void FixedUpdate()
    {
        eventInstance.setParameterByID(velocityParameterID, CalculateVelocity());
        eventInstance.setParameterByID(submersionParameterID, bc.submergedFrac);
    }

    private float CalculateVelocity()
    {
        float paramVal = rb.linearVelocity.magnitude / maxVelocity;
        paramVal = Mathf.Clamp01(paramVal);

        if (debugFMODParameterValues) Debug.Log(paramVal);

        return paramVal;
    }

    private void OnDestroy()
    {
        AudioManager.CleanUp(eventInstance);
    }
}
