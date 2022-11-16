
namespace MindPlus.Contexts
{
    using MindPlus.Contexts.Master;
    using Slash.Unity.DataBind.Core.Data;
    using UnityEngine;

    public class MasterContext : Context
    {
        private readonly Property<MenuViewContext> _menuViewContextProperty = new Property<MenuViewContext>();
        public MenuViewContext MenuViewContext
        {
            get => _menuViewContextProperty.Value;
            set => _menuViewContextProperty.Value = value;
        }
        private readonly Property<MainViewContext> _mobcontrollerContextProperty = new Property<MainViewContext>();
        public MainViewContext MobileControllerContext
        {
            get => _mobcontrollerContextProperty.Value;
            set => _mobcontrollerContextProperty.Value = value;
        }
        private readonly Property<NotifyContext> _notifyContextProperty = new Property<NotifyContext>();
        public NotifyContext Notify
        {
            get => _notifyContextProperty.Value;
            set => _notifyContextProperty.Value = value;
        }
  
        private readonly Property<ProfileViewContext> _profileViewProperty = new Property<ProfileViewContext>();
        public ProfileViewContext ProfileViewContext
        {
            get => _profileViewProperty.Value;
            set => _profileViewProperty.Value = value;
        }
    }
}


