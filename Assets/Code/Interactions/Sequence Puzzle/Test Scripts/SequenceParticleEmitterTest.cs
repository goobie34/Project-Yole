using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SequenceParticleEmitterTest : MonoBehaviour
{
    //[SerializeField] private ParticleSystem particleSystem;

    [SerializeField] private SequencePlayer sequencePlayer;

    public bool rune = false;

    private Animator anim;

    [SerializeField] private float flashTime;

    private readonly Color trans = new Color(0, 0, 0, 0);

    [SerializeField] private EffectMaterialInterface emi;

    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        //if (particleSystem == null)
        //{
        //    particleSystem = GetComponent<ParticleSystem>();
        //    //Debug.LogError("Particle system is not assigned!");
        //    //return;
        //}

        if (sequencePlayer == null)
        {
            sequencePlayer = GetComponent<SequencePlayer>();
            //Debug.LogError("SequencePlayer is not assigned!");
            //return;
        }

        if (sequencePlayer != null)
        {
            sequencePlayer.OnCorrectInput.AddListener(() => anim?.SetTrigger("CorrectInput"));

            //sequencePlayer.OnSequenceSelect.AddListener(() => anim?.SetTrigger("StoneSelection"));

            sequencePlayer.OnSequenceSelect.AddListener(() => anim?.SetTrigger("RuneSelection"));

            sequencePlayer.OnWrongInput.AddListener(() => anim?.SetTrigger("WrongInput"));

            //sequencePlayer.OnSequenceSelect.AddListener(() => StartCoroutine(ShowYellowBorder()));
            //sequencePlayer.OnSequenceDeselect.AddListener(DisableBorderColor);
            //sequencePlayer.OnWrongInput.AddListener(() => StartCoroutine(ShowRedBorder()));
        }
        else Debug.Log("SequencePlayer was not found!");

        if (emi == null)
            emi = GetComponent<EffectMaterialInterface>();
    }

    private void DisableBorderColor()
    {
        emi.FlashColor.a = 0;
        emi.OutlineColor = trans;
    }

    private IEnumerator ShowRedBorder()
    {
        yield return StartCoroutine(Flash());
        
        emi.OutlineColor.a = 1;
        emi.OutlineColor.r = 1;
    }
    private IEnumerator ShowGreenBorder()
    {
        yield return StartCoroutine(Flash());

        emi.OutlineColor.a = 1;
        emi.OutlineColor.g = 1;
    }

    //private IEnumerator ShowYellowBorder()
    //{
    //    yield return StartCoroutine(Flash());

    //    emi.OutlineColor.a = 1;
    //    emi.OutlineColor.r = 0.565f;
    //    emi.OutlineColor.g = 0.439f;
    //    emi.OutlineColor.b = 0.004f;
    //}

    private IEnumerator Flash()
    {
        emi.FlashColor.a = 1;
        yield return new WaitForSeconds(flashTime);
        emi.FlashColor.a = 0;
    }

    //private void EmitRedParticles()
    //{
    //    var main = particleSystem.main;
    //    main.startColor = Color.red;
    //    particleSystem.Play();

    //}

    //private void EmitYellowParticles()
    //{
    //    var main = particleSystem.main;
    //    main.startColor = Color.yellow;
    //    particleSystem.Play();
    //}

    //private void EmitGreenParticles()
    //{
    //    var main = particleSystem.main;
    //    main.startColor = Color.green;
    //    particleSystem.Play();
    //}
}
