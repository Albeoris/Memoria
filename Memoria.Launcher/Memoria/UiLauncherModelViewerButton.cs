using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Timer = System.Threading.Timer;

namespace Memoria.Launcher
{
    public sealed class UiLauncherModelViewerButton : UiModManagerButton
    {
        //public GameSettingsControl GameSettings { get; set; }
        public UiLauncherModelViewerButton()
        {
            Label = Lang.Launcher.ModelViewer;
        }

        protected override async Task DoAction()
        {
            try
            {
                Window adv = (Window)this.GetRootElement();
                MainWindow mainWindow = (MainWindow)adv.Owner;
                mainWindow.PlayButton.Click(true);
            }
            catch (Exception) {}
        }
    }
}
