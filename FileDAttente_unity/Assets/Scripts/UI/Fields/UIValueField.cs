using System;
using System.Globalization;

[Serializable]
public class UIValueField : UIField
{
    public string StringInput { get; private set; }

    public override void SetInput(object input)
    {
        StringInput = input != null ? input.ToString() : "";
        base.SetInput(StringInput);
    }

    public override object ApplyField(object fieldValue, out bool changeCheck)
    {
        object modifiedFieldValue = fieldValue;

        Type inputType = FieldType;
        if (inputType == typeof(int))
        {
            if (int.TryParse(StringInput, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                modifiedFieldValue = intValue;
            else
                StringInput = fieldValue != null ? fieldValue.ToString() : "";
        }
        else if (inputType == typeof(float))
        {
            if (StringInput != null)
                StringInput = StringInput.Replace(",", ".");
            if (float.TryParse(StringInput, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                modifiedFieldValue = floatValue;
            else
                StringInput = fieldValue != null ? fieldValue.ToString() : "";
        }
        else if (inputType == typeof(string))
            modifiedFieldValue = StringInput;

        changeCheck = modifiedFieldValue != fieldValue;
        if (changeCheck)
            return base.ApplyField(modifiedFieldValue, out changeCheck);
        else
            return fieldValue;
    }
}
