using UnityEngine;

public class SequenceParticleEmitterTest : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    [SerializeField] private SequencePlayer sequencePlayer;

    private void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
            //Debug.LogError("Particle system is not assigned!");
            //return;
        }

        if (sequencePlayer == null)
        {
            sequencePlayer = GetComponent<SequencePlayer>();
            //Debug.LogError("SequencePlayer is not assigned!");
            //return;
        }

        if (sequencePlayer != null)
        {
            sequencePlayer.OnCorrectInput.AddListener(EmitGreenParticles);
            sequencePlayer.OnSequenceSelect.AddListener(EmitYellowParticles);
            sequencePlayer.OnWrongInput.AddListener(EmitRedParticles);
        }
        else Debug.Log("SequencePlayer was not found!");
    }

    private void EmitRedParticles()
    {
        var main = particleSystem.main;
        main.startColor = Color.red;
        particleSystem.Play();
    }

    private void EmitYellowParticles()
    {
        var main = particleSystem.main;
        main.startColor = Color.yellow;
        particleSystem.Play();
    }

    private void EmitGreenParticles()
    {
        var main = particleSystem.main;
        main.startColor = Color.green;
        particleSystem.Play();
    }
}
