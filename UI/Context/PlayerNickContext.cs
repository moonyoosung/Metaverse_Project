
using Slash.Unity.DataBind.Core.Data;

namespace MindPlus.Contexts.Player
{
    public class PlayerNickContext : Context
    {
        private readonly Property<string> _nickNameProperty = new Property<string>();
        public string NickName
        {
            get => _nickNameProperty.Value;
            set => _nickNameProperty.Value = value;
        }
    }
}
