using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace Memoria.Launcher
{
    public abstract partial class UiModManagerButton
    {
        protected UiModManagerButton()
        {
            InitializeComponent();
        }

        public void Click()
        {
            if (!IsEnabled)
                return;

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

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(String), typeof(UiModManagerButton), new PropertyMetadata("Button Label"));

        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
    }
}
