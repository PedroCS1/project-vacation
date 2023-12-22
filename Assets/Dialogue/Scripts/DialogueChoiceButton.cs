using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private Image Background;

    private Image[] ChoiceIcons;

    private Button Button = null;

    public void Initialize()
    {
        Button = GetComponent<Button>();
        ChoiceIcons = GetComponentsInChildren<Image>().Where(img => img.transform != transform).ToArray();
    }

    public void AddListener(Action<int> action, int index)
    {
        Button.onClick.AddListener(() => action(index));
    }

    public void AddListener(Action action)
    {
        Button.onClick.AddListener(() => action());
    }

    public void RemoveListeners()
    {
        Button.onClick.RemoveAllListeners();
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    public void SetInteractable(bool interactable)
    {
        Button.interactable = interactable;
    }

    public void SetFontMaterial(Material material)
    {
        if (material != null)
            Text.fontMaterial = material;
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite != null)
            Background.sprite = sprite;
    }

    public void SetChoiceColor(Color color)
    {
        var colors = Button.colors;
        colors.disabledColor = color;
        Button.colors = colors;
    }
}
