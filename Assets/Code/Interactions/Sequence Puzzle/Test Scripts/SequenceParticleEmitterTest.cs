using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SequenceParticleEmitterTest : MonoBehaviour
{
    [SerializeField] private SequencePlayer sequencePlayer;

    private Animator anim;

    [SerializeField] private EffectMaterialInterface emi;

    private void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (sequencePlayer == null)
        {
            sequencePlayer = GetComponent<SequencePlayer>();
        }

        if (sequencePlayer != null)
        {
            sequencePlayer.OnCorrectInput.AddListener(() => anim?.SetTrigger("CorrectInput"));

            sequencePlayer.OnSequenceSelect.AddListener(() => anim?.SetTrigger("RuneSelection"));

            sequencePlayer.OnWrongInput.AddListener(() => anim?.SetTrigger("WrongInput"));
        }
        else Debug.Log("SequencePlayer was not found!");
    }
}
