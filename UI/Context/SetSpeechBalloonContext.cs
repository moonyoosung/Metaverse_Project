using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Slash.Unity.DataBind.Core.Data;

namespace MindPlus.Contexts.Player
{
    public class SetSpeechBalloonContext : Context
    {
        private readonly Property<string> _textProperty = new Property<string>();
        public string SpeechBalloonText
        {
            get => _textProperty.Value;
            set => _textProperty.Value = value;
        }

        private readonly Property<bool> _isActiveBalloonProperty = new Property<bool>();
        public bool IsActiveBalloon
        {
            get => _isActiveBalloonProperty.Value;
            set
            {
                _isActiveBalloonProperty.Value = value;
            }
        }
    }
}
