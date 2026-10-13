using UnityEngine;

public class LoadingScreenService : MonoBehaviour, ILoadingScreenService
{
    public int queueCounter = 0;

    public Animator animator;

    public void Awake()
    {
        LoadingScreenServiceLocator.Instance.Register(this);

        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public void AddShowLoadingScreen()
    {
        queueCounter++;
    }

    public void AddHideLoadingScreen()
    {
        queueCounter--;
    }

    public void FixedUpdate()
    {
        CheckLoadingScreenToggle();
    }

    private void CheckLoadingScreenToggle()
    {
        animator.SetBool("Shown", queueCounter > 0);
    }

}
