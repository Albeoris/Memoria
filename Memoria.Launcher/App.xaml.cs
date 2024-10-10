using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Memoria.Launcher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SubscribeUnhandledExceptions();
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            HandleArguments(e);
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [DebuggerNonUserCode]
        public void InitializeComponent2()
        {
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }

        [STAThread]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [DebuggerNonUserCode]
        public static void Main()
        {
            IniFile.SanitizeMemoriaIni();

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private void SubscribeUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        private void TaskSchedulerOnUnobservedTaskException(Object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private void HandleException(Exception ex)
        {
            var message = "A critical error occurred during the operation of the program." + Environment.NewLine +
                          "Further work of the program may lead to data corruption. The application will be closed." + Environment.NewLine +
                          "Please press Ctrl+C to copy the exception details, and contact the author to fix the problem." + Environment.NewLine +
                          Environment.NewLine +
                          ex;

            MessageBox.Show(message, "Critical error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void HandleArguments(StartupEventArgs e)
        {
            for (Int32 i = 0; i < e.Args.Length; i++)
            {
                switch (e.Args[i].ToLower())
                {
                    case "-autorun":
                    case "-autorungame":    // Same format as the option in "Settings.ini"
                    case "/launch":         // Same format as the similar option in FF7's 7th Heaven
                        App.AutoRunGame = true;
                        break;
                }
            }
        }

        public static Boolean AutoRunGame = false;
    }
}
