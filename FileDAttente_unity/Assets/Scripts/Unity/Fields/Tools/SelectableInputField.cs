using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableInputField : InputField, ISelectableField
{
    private class SelectEvent : UnityEvent<ISelectableField, bool> { }
    private SelectEvent onSelect;

    public void AddSelectListener(UnityAction<ISelectableField, bool> selectAction)
    {
        if (onSelect == null) onSelect = new SelectEvent();
        onSelect.AddListener(selectAction);
    }

    public void RemoveSelectListener(UnityAction<ISelectableField, bool> selectAction)
    {
        if (onSelect == null) return;
        onSelect.RemoveListener(selectAction);
    }

    public void SelectField()
    {
        Select();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelect?.Invoke(this, true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        onSelect?.Invoke(this, false);
    }
}
