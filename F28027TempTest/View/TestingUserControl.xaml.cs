using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace F28027TempTest.View
{
    /// <summary>
    /// Логика взаимодействия для TestingUserControl.xaml
    /// </summary>
    public partial class TestingUserControl : UserControl
    {
        public TestingUserControl()
        {
            InitializeComponent();
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
