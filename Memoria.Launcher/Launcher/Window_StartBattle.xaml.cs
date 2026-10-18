using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Memoria.Launcher
{
    public partial class Window_StartBattle : UserControl
    {
        private readonly Action<GameLaunchOptions> _launchBattle;

        public Window_StartBattle(Action<GameLaunchOptions> launchBattle)
        {
            _launchBattle = launchBattle ?? throw new ArgumentNullException(nameof(launchBattle));
            InitializeComponent();
            BattleIdTextBox.Text = "336";
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                BattleIdTextBox.Focus();
                Keyboard.Focus(BattleIdTextBox);
                BattleIdTextBox.SelectAll();
            }));
        }

        private void BattleId_TextChanged(Object sender, TextChangedEventArgs e)
        {
            Boolean valid = Int32.TryParse(BattleIdTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 battleId) && battleId >= 0 && battleId <= Int16.MaxValue;
            Ok.IsEnabled = valid;
            ValidationMessage.Visibility = valid ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Ok_Click(Object sender, RoutedEventArgs e)
        {
            if (!Ok.IsEnabled)
            {
                return;
            }

            GameLaunchOptions options = GameLaunchOptions.Parse(new String[] { "-memoria-battle", BattleIdTextBox.Text });
            Close(this, e);
            _launchBattle(options);
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            ((Panel)Parent).Children.Remove(this);
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.BattleLaunchButton.Focus();
            Keyboard.Focus(mainWindow.BattleLaunchButton);
        }

        private void OnPreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close(this, e);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && e.OriginalSource == BattleIdTextBox)
            {
                Ok_Click(this, e);
                e.Handled = true;
            }
        }
    }
}
