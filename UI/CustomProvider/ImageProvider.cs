using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Foundation.Providers.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Data Bind/Foundation/Objects/[DB] Image Provider")]
[DataTypeHintExplicit(typeof(Image))]
public class ImageProvider : ConstantObjectProvider<Image>
{
    public Image image;
    public override Image ConstantValue
    {
        get => image;
    }
}
