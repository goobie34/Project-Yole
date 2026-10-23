using System;
using System.Collections;
using UnityEngine;

public class SquashStrech : MonoBehaviour
{
    [Header("Notes")]
    [SerializeField, Multiline(lines: 2)] private string notes;

    [Header("Squash and Strech Core")]
    [SerializeField] 
    private Transform transformToAffect;
    [SerializeField]
    private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField]
    private float animationDuration = 0.25f;
    [SerializeField] 
    private bool canBeOverwritten;
    [SerializeField]
    private bool playOnStart;
    [SerializeField] 
    private bool playsEveryTime = true;
    [SerializeField] 
    private float chanceToPlay = 100f;


    [Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    [Header("Animation Settings")]
    [SerializeField]
    private float initialScale = 1f;
    [SerializeField]
    private float maximumScale = 1.3f;
    [SerializeField]
    private bool resetToInitialScaleAfterAnimation = true;
    [SerializeField] private bool resverseAnimationCurveAfterPlaying;
    private bool isReversed;
    [SerializeField]
    private AnimationCurve squashAndStretchCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(1f, 0f));

    [Header("Looping Settings")]
    [SerializeField]
    private bool looping;
    [SerializeField]
    private float loopingDelay = 0.5f;

    private Coroutine squashAndStretchCoroutine;
    private WaitForSeconds loopingDelayWaitForSeconds;
    private Vector3 initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private static event Action squashAndStretchAllObjectsLikeThis;
   
    private void Awake()
    {
        if (transformToAffect == null)
            transformToAffect = transform;

        initialScaleVector = transformToAffect.localScale;
        loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    public static void SquashAndStretchAllObjectsLikeThis()
    {
        squashAndStretchAllObjectsLikeThis?.Invoke();
    }

    private void OnEnable()
    {
        squashAndStretchAllObjectsLikeThis += PlaySquashAndStretch;
    }

    private void OnDisable()
    {
        if (squashAndStretchCoroutine != null)
            StopCoroutine(squashAndStretchCoroutine);

        squashAndStretchAllObjectsLikeThis -= PlaySquashAndStretch;
    }

    private void Start()
    {
        if (playOnStart)
            CheckForAndStartCoroutine();
    }

    [ContextMenu("Play Squash and Stretch")]

    public void PlaySquashAndStretch()
    {
        if (looping && !canBeOverwritten)
            return;

        CheckForAndStartCoroutine();
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            if (!playsEveryTime)
            {
                float random = UnityEngine.Random.Range(0, 100f);
                if (random > chanceToPlay)
                {
                    yield return null;
                    continue;
                }
            }

            if (resverseAnimationCurveAfterPlaying)
                isReversed = !isReversed;

            float elapsedTime = 0;
            Vector3 originalScale = initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition;

                if (isReversed)
                    curvePosition = 1 - (elapsedTime / animationDuration);
                else
                    curvePosition = elapsedTime / animationDuration;

                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

                float minimumThresHold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThresHold)
                    remappedValue = minimumThresHold;

                if (affectX)
                    modifiedScale.x = originalScale.x * remappedValue;
                else
                    modifiedScale.x = originalScale.x / remappedValue;

                if (affectY)
                    modifiedScale.y = originalScale.y * remappedValue;
                else
                    modifiedScale.y = originalScale.y / remappedValue;

                if (affectZ)
                    modifiedScale.z = originalScale.z * remappedValue;
                else
                    modifiedScale.z = originalScale.z / remappedValue;

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = originalScale;

            if (looping)
            {
                yield return loopingDelayWaitForSeconds;
            }
        } while (looping);
    }

    private void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }

    private void CheckForAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis to affect is set to None.", gameObject);
            return;
        }

        if (squashAndStretchCoroutine != null)
        {
            StopCoroutine(squashAndStretchCoroutine);
            if (playsEveryTime && resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = initialScaleVector;
        }

        squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }
}
