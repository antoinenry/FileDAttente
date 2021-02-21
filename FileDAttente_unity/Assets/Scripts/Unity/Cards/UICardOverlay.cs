using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UICardOverlay : MonoBehaviour
{
    public UICardBuilder cardBuilder;
    public Image background;
    public RectTransform panel;

    public bool IsActive { get; private set; }

    private void OnEnable()
    {
        AddListenners();
        UpdateDisplay();
    }

    private void OnDisable()
    {
        RemoveListenners();
    }

    private void Update()
    {
        if (cardBuilder == null)
        {
            if (Application.isPlaying == true) Debug.LogWarning("Overlay card builder is null");
            cardBuilder = GetComponentInChildren<UICardBuilder>();
            if (cardBuilder != null) AddListenners();
        }
    }

    private void AddListenners()
    {
        if (cardBuilder != null) cardBuilder.onCardChange.AddListener(OnBuilderCardChange);
    }

    private void RemoveListenners()
    {
        if (cardBuilder != null) cardBuilder.onCardChange.RemoveListener(OnBuilderCardChange);
    }

    public void SetOverlayActive(bool active)
    {
        if (active == IsActive) return;
        IsActive = active;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (background != null) background.enabled = IsActive;
        if (panel != null) panel.localScale = IsActive ? Vector2.one : Vector2.zero;
    }

    private void OnBuilderCardChange()
    {
        SetOverlayActive(cardBuilder != null && cardBuilder.CurrentCard != null);
    }
}
