using Slash.Unity.DataBind.Core.Data;
using Slash.Unity.DataBind.Foundation.Setters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCollectionSetter : SingleSetter<Collection<Image>>
{
    [SerializeField]
    public List<ImageProvider> dataProviders;
    protected override void OnValueChanged(Collection<Image> target)
    {
        foreach (var dataProvider in dataProviders)
        {
            if (!target.Contains(dataProvider.image))
            {
                target.Add(dataProvider.image);
            }
        }
    }
}
