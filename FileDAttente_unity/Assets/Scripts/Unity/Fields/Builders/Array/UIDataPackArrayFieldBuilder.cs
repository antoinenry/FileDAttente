using System;
using System.Collections.Generic;

public class UIDataPackArrayFieldBuilder : UIArrayFieldBuilder
{
    public IDatapack[] Inputs
    {
        get
        {
            if (itemFieldBuilders == null)
                return new IDatapack[0];
            else
            {
                int itemCount = ItemBuilderCount;
                List<IDatapack> stringInputs = new List<IDatapack>(itemCount);
                for (int i = 0; i < itemCount; i++)
                {
                    UIDataPackFieldBuilder fieldBuilder = GetFieldBuilderAt(i);
                    if (fieldBuilder != null && fieldBuilder.CurrentField != null && fieldBuilder.CurrentField is UIDataPackField)
                        stringInputs.Add((fieldBuilder.CurrentField as UIDataPackField).dataInput);
                    else
                        stringInputs.Add(null);
                }

                return stringInputs.ToArray();
            }
        }
    }

    private UIDataPackFieldBuilder GetFieldBuilderAt(int index)
    {
        if (itemFieldBuilders == null || index < 0 || index > itemFieldBuilders.Count || itemFieldBuilders[index] is UIDataPackFieldBuilder == false)
            return null;
        else
            return itemFieldBuilders[index] as UIDataPackFieldBuilder;
    }

    protected override void OnSetDatabaseIndex(UIField f)
    {
        throw new NotImplementedException();
    }

    public override Type GetUIFieldType()
    {
        return typeof(UIDataPackArrayField);
    }
}
