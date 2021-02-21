using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

[ExecuteAlways]
public class UICardBuilder : MonoBehaviour
{
    [SerializeField] [DataPack] private string currentDataType;
    public bool isClosable;

    [Header("Components")]
    public Text headerLabel;
    public Button validateButton;
    public Button closeButton;
    public UIFieldBuilder[] fieldPrefabs;
    public RectTransform fieldInstancesRoot;

    [Header("Look")]
    public float fieldSpacing;

    public UICard CurrentCard { get; private set; }
    public static string[] FieldTypeNames = new string[] { "UIValueField", "UIDataPackField", "UIValueArrayField", "UIDataPackArrayField", "UIDatabaseItemField" };

    public UnityEvent onCardChange;

    private List<UIFieldBuilder> fieldInstances;

    private void OnEnable()
    {
        if (closeButton != null) closeButton.onClick.AddListener(CloseCard);
        if (validateButton != null) validateButton.onClick.AddListener(SaveAndCloseCard);
    }

    private void OnDisable()
    {
        if (closeButton != null) closeButton.onClick.RemoveListener(CloseCard);
        if (validateButton != null) validateButton.onClick.RemoveListener(SaveAndCloseCard);
    }

    //private void Update()
    //{
    //    RebuildCard();
    //}

    public void BuildCard(UICard card)
    {
        ClearCard();
        if (card == null)
            return;

        if (headerLabel != null) headerLabel.text = card.HeaderText;
        isClosable = card.closable;
        if (closeButton != null) closeButton.gameObject.SetActive(isClosable);
        if (validateButton != null) validateButton.gameObject.SetActive(isClosable && card.isEdited);

        fieldInstances = new List<UIFieldBuilder>(card.FieldCount);
        float totalFieldHeight = 0;
        foreach (UIField f in card.fields)
        {
            BuildField(f, out float height);
            totalFieldHeight += height;
        }
        fieldInstancesRoot.sizeDelta = new Vector2(fieldInstancesRoot.sizeDelta.x, (1f + fieldSpacing) * totalFieldHeight);
        CurrentCard = card;
        currentDataType = card.GetDataType().AssemblyQualifiedName;
        onCardChange.Invoke();
        AddCardListeners();
    }

    public void ClearCard()
    {
        RemoveCardListeners();
        if (headerLabel != null) headerLabel.text = "";
        if (closeButton != null) closeButton.gameObject.SetActive(false);
        if (validateButton != null) validateButton.gameObject.SetActive(false);

        Transform[] fieldRootChildren = fieldInstancesRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0, iend = fieldRootChildren.Length; i < iend; i++)
        {
            Transform child = fieldRootChildren[i];
            if (child != null && child != fieldInstancesRoot)
                DestroyImmediate(child.gameObject);
        }
        fieldInstancesRoot.sizeDelta = new Vector2(fieldInstancesRoot.sizeDelta.x, 0f);
        if (fieldInstances != null) fieldInstances.Clear();
        CurrentCard = null;
        currentDataType = "(null)";
        onCardChange.Invoke();
    }

    //public void RebuildCard()
    //{
    //    Type dataType = currentDataType != null ? Type.GetType(currentDataType) : null;
    //    if (dataType != null)
    //    {
    //        if (CurrentCard == null || dataType != CurrentCard.GetDataType() || isClosable != CurrentCard.closable)
    //            BuildCard(UICard.NewCard(dataType, isClosable));
    //        else
    //            BuildCard(CurrentCard);
    //    }
    //    else if (CurrentCard != null)
    //        ClearCard();
    //}

    public void SaveAndCloseCard()
    {
        if (CurrentCard != null)
        {
            CurrentCard.Save();
            CurrentCard.Close();
        }
    }

    public void CloseCard()
    {
        if (CurrentCard != null)
            CurrentCard.Close();
    }

    private void BuildField(UIField f, out float fieldHeight)
    {
        fieldHeight = 0f;
        if (f == null) return;
        string fieldTypeName = f.GetType().Name;
        int fieldPrefabIndex = Array.IndexOf(FieldTypeNames, fieldTypeName);
        UIFieldBuilder prefab = fieldPrefabIndex >= 0 && fieldPrefabIndex < fieldPrefabs.Length ? fieldPrefabs[fieldPrefabIndex] : null;
        if (prefab == null) return;
        UIFieldBuilder fieldInstance = Instantiate(prefab, fieldInstancesRoot);
        fieldInstance.CurrentField = f;
        fieldHeight = prefab.GetComponent<RectTransform>().rect.height;
        fieldInstances.Add(fieldInstance);
    }

    private void AddCardListeners()
    {
        if (CurrentCard != null)
        {
            CurrentCard.onClose += OnCloseCard;
            CurrentCard.onEdit += OnEditCard;
            CurrentCard.onEndEdit += OnEndEditCard;
            CurrentCard.onSetData += OnSetCurrentCardData;
        }
    }

    private void RemoveCardListeners()
    {
        if (CurrentCard != null)
        {
            CurrentCard.onClose -= OnCloseCard;
            CurrentCard.onEdit -= OnEditCard;
            CurrentCard.onEndEdit -= OnEndEditCard;
            CurrentCard.onSetData -= OnSetCurrentCardData;
        }
    }

    private void OnCloseCard(UICard card)
    {
        if (card == CurrentCard)
            ClearCard();
    }

    private void OnEditCard(UICard card)
    {
        if (card == null || card != CurrentCard) return;
        if (validateButton != null) validateButton.gameObject.SetActive(isClosable);
    }

    private void OnEndEditCard(UICard card)
    {
        if (card == null || card != CurrentCard) return;
    }

    private void OnSetCurrentCardData(UICard card)
    {
        if (card == null || card != CurrentCard || fieldInstances == null) return;

        foreach(UIFieldBuilder fieldBuilder in fieldInstances)
        {
            if (fieldBuilder != null)
                fieldBuilder.UpdateInputComponent();
        }
    }
}
