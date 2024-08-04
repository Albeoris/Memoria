using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Memoria.Launcher
{
    public partial class AdvOptionsWindow : Window, IComponentConnector
    {
        public String StatusMessage = "";
        public AdvOptionsWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closing += new CancelEventHandler(OnClosing);
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
        }

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            ((MainWindow)this.Owner).AdvOptionsWindow = null;
            ((MainWindow)this.Owner).MemoriaIniControl.ComeBackToLauncherFromModManager();
        }
        
        private bool _isOption1Checked;
        public bool IsOption1Checked
        {
            get { return _isOption1Checked; }
            set
            {
                if (_isOption1Checked != value)
                {
                    _isOption1Checked = value;
                    OnPropertyChanged(nameof(IsOption1Checked));
                }
            }
        }

        // hover images
        private void Shader_comparison1_MouseEnter(Object sender, System.Windows.Input.MouseEventArgs e) => shader_comparison1_full.Visibility = Visibility.Visible;
        private void Shader_comparison1_MouseLeave(Object sender, System.Windows.Input.MouseEventArgs e) => shader_comparison1_full.Visibility = Visibility.Collapsed;
        private void Shader_comparison2_MouseEnter(Object sender, System.Windows.Input.MouseEventArgs e) => shader_comparison2_full.Visibility = Visibility.Visible;
        private void Shader_comparison2_MouseLeave(Object sender, System.Windows.Input.MouseEventArgs e) => shader_comparison2_full.Visibility = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern Boolean ReleaseCapture();

        private const String INI_PATH = "./Memoria.ini";
        private const String CATALOG_PATH = "./ModCatalog.xml";
        private const String CATALOG_URL = "https://raw.githubusercontent.com/Albeoris/Memoria/main/Memoria.Launcher/Catalogs/MemoriaCatalog.xml";
    }
}
