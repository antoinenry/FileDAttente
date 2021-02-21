using System;
using System.Collections.Generic;

public class UIValueArrayFieldBuilder : UIArrayFieldBuilder
{
    public string[] Inputs
    {
        get
        {            
            if (itemFieldBuilders == null)
                return new string[0];
            else
            {
                int itemCount = ItemBuilderCount;
                List<string> stringInputs = new List<string>(itemCount);
                for (int i = 0; i < itemCount; i++)
                {
                    UIValueFieldBuilder fieldBuilder = GetFieldBuilderAt(i);
                    if (fieldBuilder != null && fieldBuilder.CurrentField != null && fieldBuilder.CurrentField is UIValueField)
                        stringInputs.Add((fieldBuilder.CurrentField as UIValueField).StringInput);
                    else
                        stringInputs.Add("");
                }

                return stringInputs.ToArray();
            }
        }
    }

    private UIValueFieldBuilder GetFieldBuilderAt(int index)
    {
        if (itemFieldBuilders == null || index < 0 || index > itemFieldBuilders.Count || itemFieldBuilders[index] is UIValueFieldBuilder == false)
            return null;
        else
            return itemFieldBuilders[index] as UIValueFieldBuilder;
    }

    protected override void OnSetDatabaseIndex(UIField f)
    {
        throw new NotImplementedException();
    }

    public override Type GetUIFieldType()
    {
        return typeof(UIValueArrayField);
    }
}
