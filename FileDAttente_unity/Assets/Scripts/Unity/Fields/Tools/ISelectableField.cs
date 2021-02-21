using UnityEngine.Events;

public interface ISelectableField
{
    void AddSelectListener(UnityAction<ISelectableField, bool> selectAction);
    void RemoveSelectListener(UnityAction<ISelectableField, bool> selectAction);
    void SelectField();
}
