using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public abstract class InteractableObjectBase : MonoBehaviour
{
    [SerializeField] protected UnityEvent _PrimaryEvent;
    [SerializeField] protected UnityEvent _SecondaryEvent;

    protected bool _CanInteract = false;
    [SerializeField] protected string _InteractionDescription = "";

    private Outline _Outline;
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _Outline = GetComponent<Outline>();
        Highlight(false);
    }

    public string InteractionDescription { get => _InteractionDescription; set => _InteractionDescription = value; }

    public abstract void Interact(GameObject playerRef);
    public void ShowInteractionText(TextMeshProUGUI playerText)
    {
        playerText.text = _InteractionDescription;
    }

    public void Highlight(bool enabled)
    {
        _Outline.enabled = enabled;
    }
}
