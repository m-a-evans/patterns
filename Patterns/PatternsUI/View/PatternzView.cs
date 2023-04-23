using PatternsUI.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PatternsUI.View
{
    public class PatternzView : UserControl
    {
        private const string ViewModelName = "ViewModel";
        public ViewModelBase? ViewModel { get; private set; }
        public ObservableCollection<MenuItem> FileMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> EditMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> HelpMenuItems { get; protected set; } = new ObservableCollection<MenuItem>();

        public PatternzView()
        {
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (TryFindResource(ViewModelName) is ViewModelBase viewModel)
            {
                ViewModel = viewModel;
            }
        }
    }
}
