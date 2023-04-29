using PatternsUI.MVVM;
using System;
using System.Windows.Controls;

namespace PatternsUI.View
{
    /// <summary>
    /// A base view which houses some common functionality
    /// </summary>
    public class PatternzView : UserControl
    {
        /// <summary>
        /// Name of the view model property
        /// </summary>
        private const string ViewModelName = "ViewModel";

        /// <summary>
        /// The viewmodel which controls the logic for this view
        /// </summary>
        public ViewModelBase? ViewModel { get; private set; }

        public PatternzView()
        {
            
        }

        /// <summary>
        /// Once the view is initialized, try to find the viewmodel from a property as set on the view
        /// </summary>
        /// <param name="e"></param>
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
