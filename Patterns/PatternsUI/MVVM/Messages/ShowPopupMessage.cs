using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsUI.MVVM.Messages
{
    public class ShowPopupMessage : IMessage 
    {
        public string Title { get; private set; }
        public string Contents { get; private set; }
        public ShowPopupMessage(string title, string contents) 
        {
            Title = title;
            Contents = contents;
        }
    }
}
