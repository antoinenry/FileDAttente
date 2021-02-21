using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class UIArrayFieldBuilder : UIFieldBuilder, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform arrayRoot;
    public Button addItemButton;
    public Button removeItemButton;
    public Button upButton;
    public Button downButton;
    public Scrollbar scrollBar;

    protected List<UIFieldBuilder> itemFieldBuilders;

    private UIAccurateButton accurateRemoveItemButton;
    private UIAccurateButton accurateUpButton;
    private UIAccurateButton accurateDownButton;
    private float fieldHeight;
    private float minScrollY;
    private RectTransform arrayContainer;
    private Coroutine clearSelectionCoroutine;
    private Coroutine mouseScrollCoroutine;

    public int ItemBuilderCount
    {
        get
        {
            if (itemFieldBuilders != null && itemFieldBuilders.Count > 0)
            {
                if (itemFieldBuilders.Count == 1)
                    return (itemFieldBuilders[0].gameObject.activeSelf == true) ? 1 : 0;
                else
                    return itemFieldBuilders.Count;
            }
            return 0;
        }
    }


    private int MaxVisibleFields
    {
        get
        {
            float containerHeight = arrayContainer != null ? arrayContainer.rect.height : 0f;
            return Mathf.FloorToInt(containerHeight / fieldHeight) - 1;
        }
    }

    public float ScrollPosition
    {
        get
        {
            return arrayRoot != null && fieldHeight > 0f ? (arrayRoot.anchoredPosition.y - minScrollY) / fieldHeight : 0;
        }
        private set
        {
            if (arrayRoot != null)
            {
                float clampValue = Mathf.Clamp(value, 0, Mathf.Max(0f, ItemBuilderCount - MaxVisibleFields));
                arrayRoot.anchoredPosition = new Vector2(arrayRoot.anchoredPosition.x, clampValue * fieldHeight + minScrollY);
            }
        }
    }

    public int SelectionIndex
    {
        get
        {
            if (CurrentField != null && CurrentField is UIArrayField)
                return (CurrentField as UIArrayField).selectionIndex;
            else
                return -1;
        }

        set
        {
            if (CurrentField != null && CurrentField is UIArrayField)
                (CurrentField as UIArrayField).selectionIndex = value;
        }
    }

    private void Awake()
    {
        if (arrayRoot != null)
        {
            arrayContainer = arrayRoot.parent.GetComponent<RectTransform>();
            UIFieldBuilder[] initInputs = arrayRoot.GetComponentsInChildren<UIFieldBuilder>(true);
            if (initInputs.Length > 0)
            {
                for (int i = 1, iend = initInputs.Length; i < iend; i++)
                    Destroy(initInputs[i].gameObject);
                UIFieldBuilder inputModel = initInputs[0];
                itemFieldBuilders = new List<UIFieldBuilder> { inputModel };
                inputModel.gameObject.SetActive(false);
                fieldHeight = inputModel.GetComponent<RectTransform>().rect.height;
            }
            minScrollY = arrayRoot.anchoredPosition.y;
        }

        if (removeItemButton != null)
        {
            accurateRemoveItemButton = removeItemButton.GetComponent<UIAccurateButton>();
            removeItemButton.interactable = false;
        }

        if (upButton != null)
        {
            accurateUpButton = upButton.GetComponent<UIAccurateButton>();
            upButton.interactable = false;
        }

        if (downButton != null)
        {
            accurateDownButton = downButton.GetComponent<UIAccurateButton>();
            downButton.interactable = false;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (addItemButton != null) addItemButton.onClick.AddListener(OnPressAddItemButton);
        if (removeItemButton != null) accurateRemoveItemButton.Down.AddListener(OnPressRemoveItemButton);
        if (upButton != null) accurateUpButton.Down.AddListener(OnPressUpButton);
        if (downButton != null) accurateDownButton.Down.AddListener(OnPressDownButton);
        if (scrollBar != null) scrollBar.onValueChanged.AddListener(OnScrollBarMove);
        SelectionIndex = -1;
        ScrollPosition = 0;
        UpdateScrollBar();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (addItemButton != null) addItemButton.onClick.RemoveListener(OnPressAddItemButton);
        if (removeItemButton != null) accurateRemoveItemButton.Down.RemoveListener(OnPressRemoveItemButton);
        if (upButton != null) accurateUpButton.Down.RemoveListener(OnPressUpButton);
        if (downButton != null) accurateDownButton.Down.RemoveListener(OnPressDownButton);
        if (scrollBar != null) scrollBar.onValueChanged.RemoveListener(OnScrollBarMove);
    }

    protected override void Build(UIField f)
    {
        base.Build(f);
        RemoveAllItemBuilders();
        if (CurrentField != null)
        {
            UIArrayField arrayField = CurrentField as UIArrayField;
            if (arrayField.ItemFields != null)
            {
                foreach (UIField itemField in arrayField.ItemFields)
                    AddItemBuilder(itemField, false);
            }
        }
        UpdateScrollBar();
    }

    public override void UpdateInputComponent()
    {
        if (itemFieldBuilders != null && CurrentField is UIArrayField)
        {
            //int builderCount = itemFieldBuilders.Count;
            //int fieldCount = (CurrentField as UIArrayField).ItemCount;
            //List<UIField> itemFields = (CurrentField as UIArrayField).ItemFields;
            //for (int i = 0; i < fieldCount; i++)
            //{
            //    if (i < builderCount)
            //    {
            //        Debug.Log("Set currentField at " +  i);
            //        itemFieldBuilders[i].CurrentField = itemFields[i];
            //    }
            //    else
            //    {
            //        Debug.Log("Add fieldBuilder at " + i);
            //        AddItemBuilder(itemFields[i], false);
            //    }
            //}
            //for (int i = builderCount - 1; i > fieldCount; i--)
            //    RemoveItemBuilderAt(i, false);

            int builderCount = ItemBuilderCount;
            int fieldCount = (CurrentField as UIArrayField).ItemCount;
            List<UIField> itemFields = (CurrentField as UIArrayField).ItemFields;
            for (int i = builderCount; i < fieldCount; i++)
                AddItemBuilder(itemFields[i], false);
            for (int i = builderCount - 1; i >= fieldCount; i--)
                RemoveItemBuilderAt(i, false);

            foreach (UIFieldBuilder b in itemFieldBuilders)
                b.UpdateInputComponent();

            bool showButtons = !CurrentField.ReadOnly;
            if (addItemButton != null) addItemButton.gameObject.SetActive(showButtons);
            if (removeItemButton != null) removeItemButton.gameObject.SetActive(showButtons);
            if (upButton != null) upButton.gameObject.SetActive(showButtons);
            if (downButton != null) downButton.gameObject.SetActive(showButtons);
        }



        //if (itemFieldBuilders != null)
        //{
        //    foreach (UIFieldBuilder b in itemFieldBuilders)
        //    {
        //        b.UpdateInputComponent();
        //    }
        //}
    }

    private UIFieldBuilder AddItemBuilder(UIField itemField, bool selectItem)
    {
        int fieldCount = ItemBuilderCount;
        if (itemFieldBuilders != null && itemFieldBuilders.Count > 0)
        {
            UIFieldBuilder newItemFieldBuilder;
            if (fieldCount == 0)
            {
                newItemFieldBuilder = itemFieldBuilders[0];
                if (newItemFieldBuilder == null) return null;
                newItemFieldBuilder.gameObject.SetActive(true);
            }
            else
            {
                newItemFieldBuilder = Instantiate(itemFieldBuilders[0], arrayRoot);
                if (newItemFieldBuilder == null) return null;
                itemFieldBuilders.Add(newItemFieldBuilder);
            }
            newItemFieldBuilder.CurrentField = itemField;

            ISelectableField newSelectableItem = newItemFieldBuilder.GetComponentInChildren<ISelectableField>(true);
            if (newSelectableItem != null)
            {
                newSelectableItem.AddSelectListener(SelectField);
                if (selectItem) newSelectableItem.SelectField();
            }

            UpdateScrollBar();
            return newItemFieldBuilder;
        }
        return null;
    }

    private void RemoveItemBuilderAt(int builderIndex, bool selectAnother)
    {
        if (builderIndex == -1 || itemFieldBuilders == null) return;
        int fieldCount = ItemBuilderCount;
        if (builderIndex < fieldCount)
        {
            UIFieldBuilder builderToRemove = itemFieldBuilders[builderIndex];
            if (fieldCount > 1)
                itemFieldBuilders.RemoveAt(builderIndex);
            if (builderToRemove != null)
            {
                builderToRemove.onEndEdit.RemoveAllListeners();
                builderToRemove.onEdit.RemoveAllListeners();
                if (fieldCount == 1)
                    builderToRemove.gameObject.SetActive(false);
                else
                    Destroy(builderToRemove.gameObject);
            }

            for (int i = 0, iend = fieldCount - 1; i < iend; i++)
                itemFieldBuilders[i].UpdateLabelComponent();

            UpdateScrollBar();
            if (selectAnother)
                SelectClosestItem();
        }
    }

    private void RemoveAllItemBuilders()
    {
        int fieldCount = ItemBuilderCount;
        if (itemFieldBuilders != null && ItemBuilderCount > 0)
        {
            for (int i = 0; i < fieldCount; i++)
            {
                UIFieldBuilder removedFieldBuilder = itemFieldBuilders[i];
                if (removedFieldBuilder == null) continue;
                removedFieldBuilder.onEndEdit.RemoveAllListeners();
                removedFieldBuilder.onEdit.RemoveAllListeners();
                if (i == 0)
                    removedFieldBuilder.gameObject.SetActive(false);
                else
                    Destroy(removedFieldBuilder.gameObject);
            }

            itemFieldBuilders.RemoveRange(1, fieldCount - 1);
            SelectionIndex = -1;
            UpdateScrollBar();
        }
    }

    private void OnScrollBarMove(float scrollBarValue)
    {
        ScrollPosition = scrollBarValue * (ItemBuilderCount - MaxVisibleFields);
    }

    private void SelectField(ISelectableField target, bool select)
    {
        if (itemFieldBuilders == null) return;

        if (clearSelectionCoroutine != null) StopCoroutine(clearSelectionCoroutine);
        if (select == true)
        {
            SelectionIndex = itemFieldBuilders.FindIndex(b => b != null  && b.GetComponentInChildren<ISelectableField>(true) == target);
            if (SelectionIndex != -1)
            {
                if (removeItemButton != null) removeItemButton.interactable = true;
                if (downButton != null) downButton.interactable = SelectionIndex < ItemBuilderCount - 1;
                if (upButton != null) upButton.interactable = SelectionIndex > 0;
                UpdateScrollBar();
                KeepFocusOnSelection();
            }
        }
        else
            ClearSelection();
    }

    private void ClearSelection()
    {
        clearSelectionCoroutine = StartCoroutine(ClearSelectionCoroutine());
    }

    private IEnumerator ClearSelectionCoroutine()
    {
        if (removeItemButton != null)
        {
            yield return null;
            SelectionIndex = -1;
            if (removeItemButton != null) removeItemButton.interactable = false;
            if (downButton != null) downButton.interactable = false;
            if (upButton != null) upButton.interactable = false;
        }
    }

    private void OnPressAddItemButton()
    {
        if (CurrentField != null && CurrentField is UIArrayField)
        {
            UIField newField = (CurrentField as UIArrayField).AddField();
            AddItemBuilder(newField, true);
        }
    }

    private void OnPressRemoveItemButton()
    {
        if (CurrentField != null && CurrentField is UIArrayField)
        {
            (CurrentField as UIArrayField).RemoveFieldAt(SelectionIndex);
            RemoveItemBuilderAt(SelectionIndex, true);
        }
    }

    public void OnPressUpButton()
    {
        if (SelectionIndex > 0 && SelectionIndex < ItemBuilderCount) MoveField(SelectionIndex, SelectionIndex - 1);
    }

    public void OnPressDownButton()
    {
        if (SelectionIndex >= 0 && SelectionIndex < ItemBuilderCount - 1) MoveField(SelectionIndex, SelectionIndex + 1);
    }

    public bool MoveField(int fromIndex, int toIndex)
    {
        if (CurrentField != null && CurrentField is UIArrayField)
        {
            bool move = (CurrentField as UIArrayField).MoveField(fromIndex, toIndex);
            if (move == false) return false;
            UIField swapField = itemFieldBuilders[fromIndex].CurrentField;
            itemFieldBuilders[fromIndex].CurrentField = itemFieldBuilders[toIndex].CurrentField;
            itemFieldBuilders[toIndex].CurrentField = swapField;
            ISelectableField selectable = itemFieldBuilders[toIndex].GetComponentInChildren<ISelectableField>(true);
            if (selectable != null) selectable.SelectField();
            return true;
        }
        else
            return false;
    }

    private void UpdateScrollBar()
    {
        if (scrollBar != null)
        {
            scrollBar.interactable = ItemBuilderCount > MaxVisibleFields;
            if (scrollBar.interactable)
            {
                scrollBar.numberOfSteps = (ItemBuilderCount - MaxVisibleFields);
                scrollBar.size = 1f / (scrollBar.numberOfSteps + 1f);
                scrollBar.value = ScrollPosition / (ItemBuilderCount - MaxVisibleFields);
            }
            else
            {
                scrollBar.numberOfSteps = 0;
                scrollBar.size = 1f;
            }
        }
    }

    public void KeepFocusOnSelection()
    {
        if (SelectionIndex >= ScrollPosition + MaxVisibleFields)
            ScrollPosition = SelectionIndex + 1 - MaxVisibleFields;
        else if (SelectionIndex < ScrollPosition)
            ScrollPosition = SelectionIndex;
    }

    private void SelectClosestItem()
    {
        if (ItemBuilderCount > 0)
        {
            UIFieldBuilder anotherFieldBuilder = (SelectionIndex < itemFieldBuilders.Count) ? itemFieldBuilders[SelectionIndex] : itemFieldBuilders[SelectionIndex - 1];
            ISelectableField anotherSelectableItem = anotherFieldBuilder != null ? anotherFieldBuilder.GetComponentInChildren<ISelectableField>(true) : null;
            if (anotherSelectableItem != null) anotherSelectableItem.SelectField();
        }
        else SelectionIndex = -1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mouseScrollCoroutine == null)
            mouseScrollCoroutine = StartCoroutine(MouseScrollCoroutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseScrollCoroutine != null)
        {
            StopCoroutine(mouseScrollCoroutine);
            mouseScrollCoroutine = null;
        }
    }

    private IEnumerator MouseScrollCoroutine()
    {
        while (true)
        {
            float mouseScroll = Input.mouseScrollDelta.y;
            if (mouseScroll != 0f)
            {
                ScrollPosition -= mouseScroll;
                UpdateScrollBar();
            }
            yield return null;
        }
    }
}
