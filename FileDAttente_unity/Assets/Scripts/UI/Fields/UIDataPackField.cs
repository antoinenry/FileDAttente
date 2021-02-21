using System;

[Serializable]
public class UIDataPackField : UIField
{
    public IDatapack dataInput;

    private UICard inputCard;

    public UICard InputCard
    {
        get => inputCard;
        set
        {
            RemoveInputCardListeners();
            inputCard = value;
            AddInputCardListeners();
        }
    }

    public override void SetInput(object input)
    {
        dataInput = (IDatapack)input;
        base.SetInput(dataInput);
    }

    public override object ApplyField(object fieldValue, out bool changeCheck)
    {
        object modifiedFieldValue = fieldValue;

        if (typeof(IDatapack).IsAssignableFrom(FieldType))// && inputCard != null)
        {
            changeCheck = true;
            modifiedFieldValue = dataInput;
        }
        else
            changeCheck = false;

        return base.ApplyField(modifiedFieldValue, out changeCheck);
    }

    public override void StartEdit()
    {
        if (InputCard == null)
        {
            bool readOnlyCard = ReadOnly || GetDatabaseReferenceAttribute() != null;
            InputCard = UICard.NewCard(FieldType, readOnlyCard, true, dataInput);
            //inputCard.Data = dataInput; // Attention si data est une classe: ref
            //AddInputCardListeners();
        }
        if (InputCard != null)
            base.StartEdit();
    }

    //public override void EndEdit()
    //{
    //    if (inputCard != null)
    //    {
    //        inputCard.Close();
    //        inputCard = null;
    //    }
    //    base.EndEdit();
    //}

    public override void Destroy()
    {
        if (InputCard != null)
            InputCard.Close();
    }

    public override void ApplyChanges()
    {
        if (InputCard == null) return;
        dataInput = InputCard.Data;
        base.ApplyChanges();
    }

    private void AddInputCardListeners()
    {
        if (InputCard == null) return;
        InputCard.onEdit += OnInputCardEdited;
        InputCard.onEndEdit += OnInputCardEndEdit;
        InputCard.onSave += OnInputCardSaved;
        InputCard.onClose += OnInputCardClosed;
    }

    private void RemoveInputCardListeners()
    {
        if (InputCard == null) return;
        InputCard.onEdit -= OnInputCardEdited;
        InputCard.onEndEdit -= OnInputCardEndEdit;
        InputCard.onSave -= OnInputCardSaved;
        InputCard.onClose -= OnInputCardClosed;
    }

    private void OnInputCardEdited(UICard card)
    {
        if (card != InputCard) return;
        Edit();
    }

    private void OnInputCardEndEdit(UICard card)
    {
        if (card != InputCard) return;
        //DatabaseItemAttribute databaseItemAttribute = GetDatabaseItemAttribute();
        //if (databaseItemAttribute != null)
        //{
        //    IDatapack currentCardData = InputCard.Data;
        //    string currentInputDataID = currentCardData.DataID;
        //    bool getItem = databaseItemAttribute.TryGetDatabaseItem(currentInputDataID, out object databaseItem);
        //    if (getItem == true)
        //    {
        //        currentCardData.DataID = currentInputDataID + "*";
        //        InputCard.Data = currentCardData;
        //    }
        //}
        EndEdit();
    }

    private void OnInputCardSaved(UICard card)
    {
        if (card != InputCard) return;
        Edit();
        ApplyChanges();
        //EndEdit();
    }

    private void OnInputCardClosed(UICard card)
    {
        if (card != InputCard) return;
        //RemoveInputCardListeners();
        InputCard = null;
        EndEdit();
    }

    public override bool SetInputFromDatabase(int indexInDatabase)
    {
        if (indexInDatabase < 0) return false;
        DatabaseReferenceAttribute databaseReference = GetDatabaseReferenceAttribute();
        if (databaseReference == null) return false;
        bool getItem = databaseReference.TryGetSourceItem(indexInDatabase, out object item);
        if (getItem == true)
        {
            if (InputCard == null)
            {
                base.StartEdit();
                Edit();
                dataInput = item as IDatapack;
                base.ApplyChanges();
                EndEdit();
            }
            else
            {
                InputCard.Close();
                dataInput = item as IDatapack;
                base.ApplyChanges();
                StartEdit();
            }
            return true;
        }
        else
            return false;
    }
}
