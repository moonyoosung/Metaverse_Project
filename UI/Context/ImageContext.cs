
namespace MindPlus.Contexts.Components
{

    using Slash.Unity.DataBind.Core.Data;
    using UnityEngine.UI;

    public class ImageContext : Context
    {
        private readonly Property<Image> _imageProperty = new Property<Image>();

        public Image Image
        {
            get => _imageProperty.Value;
            set => _imageProperty.Value = value;
        }
    }
}
