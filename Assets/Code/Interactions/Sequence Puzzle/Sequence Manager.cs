using System.Collections;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private Interactable sequenceStarter; // TEMPORÄR, SKA TAS BORT!
    [SerializeField] private Interactable[] interactableObjects;
    [SerializeField] private SequencePlayer[] sequence;
    
    private int currentIndex = 0;
    private int currentStep = 1;
    private float sequenceLengthPerObject = 1;

    private SequenceState currentState;

    private enum SequenceState
    {
        NotBegun,
        Playing,
        Waiting
    }

    private void Start()
    {
        currentState = SequenceState.NotBegun;

        sequenceStarter.OnInteract.AddListener(StartSequence);

        for (int i = 0; i < interactableObjects.Length; i++)
        {
            int index = i;
            interactableObjects[i].OnInteract.AddListener(() => OnInteraction(index));
        }
    }

    public void StartSequence()
    {
        if (currentState != SequenceState.NotBegun)
            return;

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        Debug.Log("Sequence started!");

        currentState = SequenceState.Playing;

        HideSequence();

        yield return new WaitForSeconds(1f);

        currentIndex = 0;

        for (int i = 0; i < currentStep; i++)
        {
            HideSequence();

            sequence[i].SequenceSelect();

            yield return new WaitForSeconds(sequenceLengthPerObject);
        }

        HideSequence();

        Debug.Log("Step finished!");

        currentState = SequenceState.Waiting;
    }
    
    private void HideSequence()
    {
        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i].SequenceDeselect();
        }
    }

    public void OnInteraction(int index)
    {
        if (currentState != SequenceState.Waiting)
        {
            return;
        }

        if (sequence[currentIndex].CheckInput(index))
        {
            sequence[currentIndex].CorrectInput();

            currentIndex++;

            if (currentIndex >= currentStep)
            {
                if (currentStep >= sequence.Length)
                {
                    Debug.Log("Sequence completed!");

                    currentIndex = 0;
                    currentStep = 1;
                    
                    currentState = SequenceState.NotBegun;

                    return;
                }
                currentStep++;

                Debug.Log("Next step loading!");

                currentState = SequenceState.NotBegun;

                StartCoroutine(RestartSequence());
            }

        }
        else
        {
            interactableObjects[index].GetComponent<SequencePlayer>().WrongInput();

            Debug.Log("Sequence failed!");
            currentIndex = 0;

            currentStep = 1;

            currentState = SequenceState.NotBegun;
            StartCoroutine(RestartSequence());
        }
    }

    private IEnumerator RestartSequence()
    {
        yield return new WaitForSeconds(1f);
        StartSequence();
    }
}

