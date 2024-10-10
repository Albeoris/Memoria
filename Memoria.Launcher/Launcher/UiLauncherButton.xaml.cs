using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace Memoria.Launcher
{
    public abstract partial class UiLauncherButton
    {
        public Boolean LaunchModelViewer = false;

        protected UiLauncherButton()
        {
            InitializeComponent();
        }

        public new void Click(Boolean launchModelViewer = false)
        {
            if (!IsEnabled)
                return;

            LaunchModelViewer = launchModelViewer;

            OnClick();
        }

        protected abstract Task DoAction();

        protected override async void OnClick()
        {
            try
            {
                IsEnabled = false;

                String label = Label;
                try
                {
                    await DoAction();
                }
                finally
                {
                    Label = label;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(this, ex);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(String), typeof(UiLauncherButton), new PropertyMetadata("Button Label"));

        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
    }
}
