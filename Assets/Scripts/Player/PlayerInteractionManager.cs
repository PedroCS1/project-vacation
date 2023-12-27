using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _TipText;
    [SerializeField] float _PlayerRange = 2f;
    [SerializeField] LayerMask _InteractObjLayer;
    [SerializeField] LayerMask _Ignore;


    private InteractableObjectBase _currentlyHighlighted;

    void Update()
    {
        HandleRaycasting();
    }
    /*
    private void HandleRaycasting()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _PlayerRange, _Ignore))
            return;

        if (Physics.Raycast(transform.position, transform.forward, out hit, _PlayerRange, _InteractObjLayer))
        {
            InteractableObjectBase interactable = hit.collider.gameObject.GetComponent<InteractableObjectBase>();
            if (!interactable) return;

            interactable.ShowInteractionText(_TipText);
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact(gameObject);
            }
        }
        else
        {
            _TipText.text = "";
        }
    }
    */

    private void HandleRaycasting()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, _PlayerRange, _InteractObjLayer))
        {
            InteractableObjectBase interactable = hit.collider.gameObject.GetComponent<InteractableObjectBase>();
            if (!interactable) return;

            OnInteractableFound(interactable);
        }
        else
        {
            OnInteractableNotFound();
        }
    }

    private void OnInteractableFound(InteractableObjectBase interactable)
    {
        HighlightObject(interactable);
        interactable.ShowInteractionText(_TipText);
        Interact(interactable);
    }

    private void Interact(InteractableObjectBase interactable)
    {
        if (interactable == null) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactable.Interact(gameObject);
        }
    }

    private void OnInteractableNotFound()
    {
        if (_currentlyHighlighted != null)
        {
            _currentlyHighlighted.Highlight(false);
            _currentlyHighlighted = null;
        }
        if (!string.IsNullOrEmpty(_TipText.text))
        {
            _TipText.text = "";
        }
    }

    private void HighlightObject(InteractableObjectBase interactable)
    {
        if (_currentlyHighlighted != interactable)
        {
            if (_currentlyHighlighted != null)
            {
                _currentlyHighlighted.Highlight(false);
            }

            _currentlyHighlighted = interactable;
            _currentlyHighlighted.Highlight(true);
        }
    }
}
