using UnityEngine;

public class SelectionOutline : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private Animator anim;

    private void Start()
    {
        if (interactable == null)
            interactable = GetComponent<Interactable>();
        if (anim == null)
            anim = GetComponent<Animator>();

        interactable?.OnSelect.AddListener(EnableOutline);
        interactable?.OnDeselect.AddListener(DisableOutline);
    }

    private void EnableOutline()
    {
        //Debug.Log("Enabling outline for " + gameObject?.name);
        anim.SetBool("IsSelected", true);
    }

    private void DisableOutline()
    {
        //Debug.Log("Disabling outline for " + gameObject?.name);
        anim.SetBool("IsSelected", false);
    }
}
