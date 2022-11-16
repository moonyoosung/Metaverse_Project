using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Foundation.Providers.Objects;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Data Bind/Foundation/Objects/[DB] InputField Provider")]
[DataTypeHintExplicit(typeof(InputField))]
public class InputFieldProvider : ConstantObjectProvider<InputField>
{
    public InputField inputField;
    public override InputField ConstantValue
    {
        get => inputField;
    }
}
