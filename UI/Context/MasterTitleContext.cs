namespace MindPlus.Contexts
{
    using MindPlus.Contexts.TitleView;
    using Slash.Unity.DataBind.Core.Data;
    public class MasterTitleContext : Context
    {
        private readonly Property<NickNameViewContext> _titleContextProperty = new Property<NickNameViewContext>();
        public NickNameViewContext TitleContext
        {
            get => _titleContextProperty.Value;
            set => _titleContextProperty.Value = value;
        }
        private readonly Property<LoginViewContext> _loginContextProperty = new Property<LoginViewContext>();
        public LoginViewContext LoginViewContext
        {
            get => _loginContextProperty.Value;
            set => _loginContextProperty.Value = value;
        }
    }
}
