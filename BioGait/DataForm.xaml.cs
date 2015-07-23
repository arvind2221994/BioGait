using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace BioGait
{
    /// <summary>
    /// Interaction logic for DataForm.xaml
    /// </summary>
    public partial class DataForm : UserControl,ISwitchable
    {
        public DataForm()
        {
            InitializeComponent();
        }

        private void Buttonhandler1(object sender, RoutedEventArgs e)
        {
            // Add code to perform some action here.
            if (string.IsNullOrWhiteSpace(this.txtName1.Text) || string.IsNullOrWhiteSpace(this.txtName3.Text) || string.IsNullOrWhiteSpace(this.txtName4.Text) || string.IsNullOrWhiteSpace(this.txtName5.Text) || string.IsNullOrWhiteSpace(this.txtName6.Text))
            {
                MessageBox.Show("TextBox is empty","Error!");
            }
            else
            {
                UserDetails User = new UserDetails(Convert.ToString( txtName1.Text), Convert.ToString( txtName6.Text), Convert.ToInt32(txtName3.Text),Convert.ToSingle( txtName4.Text) , Convert.ToSingle( txtName5.Text), Convert.ToString( Combo1.Text));
                
                Switcher.Switch(new MainWindow());
            }
        }

        private void CharacterValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void ComboBox_Selection(object sender, SelectionChangedEventArgs e)
        {
            // Add code to perform some action here.
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion



    }


}
