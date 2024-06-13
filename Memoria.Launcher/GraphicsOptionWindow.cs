using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Ini;
using ListView = System.Windows.Controls.ListView;
using GridView = System.Windows.Controls.GridView;
using GridViewColumnHeader = System.Windows.Controls.GridViewColumnHeader;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Application;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;

namespace Memoria.Launcher
{
    public partial class GraphicsOptionWindow : Window, IComponentConnector
    {
        public String StatusMessage = "";
        public GraphicsOptionWindow()
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
            ((MainWindow)this.Owner).GraphicsOptionWindow = null;
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
