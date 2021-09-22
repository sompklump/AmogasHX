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
using System.Windows.Shapes;

namespace AmongUsHack
{
    /// <summary>
    /// Interaction logic for SetValue.xaml
    /// </summary>
    public partial class SetValue : Window
    {
        public string value;
        public FormButtons formButtons;
        public SetValue()
        {
            InitializeComponent();
        }

        public void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            formButtons = FormButtons.Ok;
            value = value_input.Text;
            this.Close();
        }
        public enum FormButtons
        {
            Ok,
            Cancel
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            formButtons = FormButtons.Cancel;
            this.Close();
        }
    }
}
