using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        interactable.Interact();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        interactable.Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactable.Deselect();
    }
}
