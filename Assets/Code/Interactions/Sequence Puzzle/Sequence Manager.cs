using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private Interactable sequenceStarter; // TEMPORÄR, SKA TAS BORT! eller?
    [SerializeField] private Interactable[] interactableObjects;
    [SerializeField] private SequencePlayer[] sequence;

    [SerializeField] private SequenceType type;

    [SerializeField] private GameObject reward;

    private bool isSequenceCompleted = false;
    
    private enum SequenceType
    {
        SimonSays,
        Addition,
    }

    //[SerializeField] private bool allowRepeats = false; Yet to be implemented, might not be needed, but could be a fun feature to add later on.

    private int currentIndex = 0;
    private int currentStep = 1;
    
    [SerializeField] private float sequenceLengthPerObject = 1;

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

        sequenceStarter?.OnInteract.AddListener(() => { if (currentState == SequenceState.NotBegun) StartCoroutine(InitializeSequence()); });

        for (int i = 0; i < interactableObjects.Length; i++)
        {
            int index = i;

            interactableObjects[i].OnInteract.AddListener(() => OnInteraction(index));
        }
    }

    public IEnumerator InitializeSequence()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowAllBorders(false));

        StartCoroutine(StartSequence());
    }

    public IEnumerator StartSequence()
    {
        yield return null;
        //yield return StartCoroutine(ShowAllBorders());
        if (type == SequenceType.SimonSays)
            StartCoroutine(PlaySequence());
        else if (type == SequenceType.Addition)
            StartCoroutine(PlayAdditionSequence());
        else Debug.LogError("Sequence type not implemented!");

        DisableInteraction();
    }

    private IEnumerator ShowAllBorders(bool isFinished)
    {
        for (int i = 0; i < interactableObjects.Length; ++i)
        {
            if (!isFinished)
                interactableObjects[i].GetComponent<SequencePlayer>().SequenceSelect();
            else
                interactableObjects[i].GetComponent<SequencePlayer>().CorrectInput();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator PlaySequence()
    {
        Debug.Log("Sequence started!");

        currentState = SequenceState.Playing;

        yield return new WaitForSeconds(1f);

        currentIndex = 0;

        for (int i = 0; i < currentStep; i++)
        {
            sequence[i].SequenceSelect();

            yield return new WaitForSeconds(sequenceLengthPerObject);
        }

        Debug.Log("Step finished!");

        currentState = SequenceState.Waiting;
        EnableInteraction();
    }
    
    private IEnumerator PlayAdditionSequence()
    {
        Debug.Log("Addition sequence started!");
        currentState = SequenceState.Playing;
        yield return new WaitForSeconds(1f);
        currentIndex = 0;

        sequence[currentStep - 1].SequenceSelect();
        yield return new WaitForSeconds(sequenceLengthPerObject);

        //for (int i = 0; i < currentStep; i++)
        //{
        //    HideSequence();
        //    sequence[i].SequenceSelect();
        //    yield return new WaitForSeconds(sequenceLengthPerObject);
        //}
        Debug.Log("Addition step finished!");
        currentState = SequenceState.Waiting;

        EnableInteraction();
    }

    private void EnableInteraction()
    {
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            interactableObjects[i].isCurrentlyInteractable = true;
        }
    }

    private void DisableInteraction()
    {
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            interactableObjects[i].isCurrentlyInteractable = false;
        }
    }


    public void OnInteraction(int index)
    {
        if (currentState != SequenceState.Waiting) // Cannot interact right now, sequence is either playing or not begun yet
        {
            return;
        }

        if (sequence[currentIndex].CheckInput(index)) // Correct input
        {
            sequence[currentIndex].CorrectInput();

            currentIndex++;

            if (currentIndex >= currentStep) // All inputs for this step have been completed
            {
                if (currentStep >= sequence.Length) // All steps have been completed and the sequence is therefore completed
                {
                    StartCoroutine(SequenceComplete());
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

            ShuffleSequence();
            StartCoroutine(InitializeSequence());
        }
    }

    private IEnumerator SequenceComplete()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("Sequence completed!");
        
        currentIndex = 0;
        currentStep = 1;
        currentState = SequenceState.NotBegun;

        yield return StartCoroutine(ShowAllBorders(true));

        yield return new WaitForSeconds(1f);

        if (!isSequenceCompleted)
        {
            reward?.SetActive(true);
            isSequenceCompleted = true;
        }
    }

    private IEnumerator RestartSequence()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartSequence());
    }

    private void ShuffleSequence() // should probably revisit this one
    {
        int n = sequence.Length;
        SequencePlayer[] tempSequence = sequence;

        List<SequencePlayer> sequenceList = new(tempSequence);
        List<SequencePlayer> shuffledSequence = new();

        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(0, sequenceList.Count);

            shuffledSequence.Add(sequenceList[randomIndex]);
            sequenceList.RemoveAt(randomIndex);
        }

        for (int i = 0; i < shuffledSequence.Count; i++)
        {
            sequence[i] = shuffledSequence[i];
        }
    }
}

