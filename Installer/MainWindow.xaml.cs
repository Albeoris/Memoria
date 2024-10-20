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

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleGrid_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseBtnTxt_MouseEnter(Object sender, MouseEventArgs e)
        {
            CloseBtnTxt.Foreground = Brushes.Red;
        }

        private void CloseBtnTxt_MouseLeave(Object sender, MouseEventArgs e)
        {
            CloseBtnTxt.Foreground = Brushes.White;
        }

        private void CloseBtnTxt_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
