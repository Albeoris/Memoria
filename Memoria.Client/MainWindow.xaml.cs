using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
using Memoria.Client.Interaction;
using Memoria.Prime;
using Memoria.Test;

namespace Memoria.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty ConnectionViewVisibilityProperty = DependencyProperty.Register("ConnectionViewVisibility", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TabControlVisibilityProperty = DependencyProperty.Register("TabControlVisibility", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ConnectionViewTextProperty = DependencyProperty.Register("ConnectionViewText", typeof(String), typeof(MainWindow), new PropertyMetadata("Not connected."));

        public MainWindow()
        {
            InitializeComponent();

            NetworkClient.Connected += OnNetworkStreamConnected;
            NetworkClient.Disconnecting += OnNetworkStreamDisconnecting;
        }

        public Visibility ConnectionViewVisibility
        {
            get { return (Visibility)GetValue(ConnectionViewVisibilityProperty); }
            set { SetValue(ConnectionViewVisibilityProperty, value); }
        }

        public Visibility TabControlVisibility
        {
            get { return (Visibility)GetValue(TabControlVisibilityProperty); }
            set { SetValue(TabControlVisibilityProperty, value); }
        }

        public String ConnectionViewText
        {
            get { return (String)GetValue(ConnectionViewTextProperty); }
            set { SetValue(ConnectionViewTextProperty, value); }
        }

        public void ChangeConnectButtonVisibility(Boolean isVisible, String reason)
        {
            if (!CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => ChangeConnectButtonVisibility(IsVisible, reason)), DispatcherPriority.Send, null);
                return;
            }

            if (isVisible)
            {
                TabControlVisibility = Visibility.Collapsed;
                ConnectionViewVisibility = Visibility.Visible;
            }
            else
            {
                ConnectionViewVisibility = Visibility.Collapsed;
                TabControlVisibility = Visibility.Visible;
            }

            ConnectionViewText = reason;
        }

        private void OnNetworkStreamConnected()
        {
            ChangeConnectButtonVisibility(false, "Connected.");
        }

        private void OnNetworkStreamDisconnecting(String reason)
        {
            ChangeConnectButtonVisibility(true, reason);
        }

        private void OnConnectButtonClick(Object sender, RoutedEventArgs e)
        {
            try
            {
                NetworkClient.Reconnect();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(this, ex, "Failed to connect to the game.");
            }
        }
    }
}