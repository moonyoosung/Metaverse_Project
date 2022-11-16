namespace MindPlus.Contexts
{
    using Slash.Unity.DataBind.Core.Data;
    using MindPlus.Contexts.MeditationView;
    using System;

    public class MasterMeditationContext : Context
    {
        private readonly Property<BreathViewContext> _breathContextProperty = new Property<BreathViewContext>();
        public BreathViewContext BreathContext
        {
            get => _breathContextProperty.Value;
            set => _breathContextProperty.Value = value;
        }
    }
}