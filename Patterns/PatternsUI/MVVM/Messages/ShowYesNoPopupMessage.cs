using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsUI.MVVM.Messages
{
    internal class ShowYesNoPopupMessage : IMessage
    {
        public string ConfirmText { get; set; } = "OK";
        public string DenyText { get; set; } = "Cancel";
        public string Title { get; private set; }
        public string Contents { get; private set; }

        public Action<bool> OnUserSelection { get; private set; }

        public ShowYesNoPopupMessage(string title, string contents, Action<bool> onUserSelection) 
        {
            Title = title;
            Contents = contents;
            OnUserSelection = onUserSelection;
        }
    }
}
