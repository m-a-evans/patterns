using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PatternsUI.View
{
    /// <summary>
    /// Interaction logic for DataRecordsView.xaml
    /// </summary>
    public partial class DataRecordsView : PatternzView
    {
        public DataRecordsView()
        {
            InitializeComponent();
            TitleTextbox.IsVisibleChanged += FocusBoxOnVisible;
        }

        /// <summary>
        /// Focuses the text box when it becomes visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusBoxOnVisible(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool wasVisible = (bool)e.OldValue;
            bool isVisible = (bool)e.NewValue;

            if (!wasVisible && isVisible && sender is TextBox box) 
            {
                box.Focus();
            }
        }

        private void TitleTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                SaveFileNameButton.Command.Execute(TitleTextbox.Text);
            }
        }
    }
}
