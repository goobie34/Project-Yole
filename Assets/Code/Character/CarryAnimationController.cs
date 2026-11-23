using System;
using System.Collections;
using UnityEngine;

public class CarryAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform animatedCarryPoint;
    [SerializeField]
    private Transform carryPoint;

    [Header("Pickup Animation")]
    [SerializeField]
    private string pickupState = "Pickup";
    [SerializeField]
    private float pickupDuration = 0.5f;

    [Header("Drop Animation")]
    [SerializeField]
    private string dropState = "Drop";
    [SerializeField]
    private float dropDuration = 0.5f;

    private Coroutine currentAnimation;
    private Action completion;
    public bool IsAnimating => currentAnimation != null;
    public bool CanPlayPickup => isActiveAndEnabled && animator != null && animatedCarryPoint != null && carryPoint != null;
    public bool CanPlayDrop => isActiveAndEnabled && animator != null && animatedCarryPoint != null;

    public void PlayPickup(Transform target)
    {
        if (target == null || !CanPlayPickup || IsAnimating) return;
        completion = () =>
        {
            if (target == null || carryPoint == null) return;
            target.SetParent(carryPoint);
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
        };
        currentAnimation = StartCoroutine(AnimatePickup(target));
    }

    public void PlayDrop(Transform target, Vector3 destination, Action onComplete)
    {
        if (target == null || !CanPlayDrop || IsAnimating) return;
        completion = () =>
        {
            if (target != null) target.SetPositionAndRotation(destination, Quaternion.identity);
            onComplete?.Invoke();
        };
        currentAnimation = StartCoroutine(AnimateDrop(target, destination, onComplete));
    }

    private void Finish()
    {
        currentAnimation = null;
        Action callback = completion;
        completion = null;
        callback?.Invoke();
    }

    private void OnDisable()
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        Finish();
    }

    private IEnumerator AnimatePickup(Transform target)
    {
        Vector3 startPosition = target.position;
        Quaternion startRotation = target.rotation;

        animator.Play(pickupState, 0, 0f);
        animator.Update(0f);

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, pickupDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 destination = carryPoint.position;
            float horizontalT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((t - 0.2f) / 0.8f));
            Vector3 position = Vector3.Lerp(startPosition, destination, horizontalT);
            position.y = Mathf.Lerp(startPosition.y, destination.y, smoothT);

            //Use the animated point's vertical offset as the arc 
            position.y += animatedCarryPoint.localPosition.y;

            target.position = position;
            target.rotation = Quaternion.Slerp(startRotation, carryPoint.rotation, smoothT);

            yield return null;
        }
        Finish();
    }

    private IEnumerator AnimateDrop(Transform target, Vector3 destination, Action OnComplete)
    {
        target.SetParent(null, true);

        Vector3 startPosition = target.position;
        Quaternion startRotation = target.rotation;

        animator.Play(dropState, 0, 0f);
        animator.Update(0f);

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, dropDuration);

        while (elapsed < duration && target != null)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 position = Vector3.Lerp(startPosition, destination, smoothT);
            position.y += animatedCarryPoint.localPosition.y;

            target.position = position;
            target.rotation = Quaternion.Slerp(startRotation, Quaternion.identity, smoothT);

            yield return null;
        }
        Finish();
    }
}
