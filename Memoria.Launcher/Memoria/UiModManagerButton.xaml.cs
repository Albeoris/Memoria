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

        public static readonly DependencyProperty BlueRectVisibilityProperty = DependencyProperty.Register("BlueRectVisibility", typeof(Visibility), typeof(UiModManagerButton), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(String), typeof(UiModManagerButton), new PropertyMetadata("Button Label"));
        public static readonly DependencyProperty SubLabelProperty = DependencyProperty.Register("SubLabel", typeof(String), typeof(UiModManagerButton), new PropertyMetadata(null));
        public static readonly DependencyProperty SubLabelColorProperty = DependencyProperty.Register("SubLabelColor", typeof(Brush), typeof(UiModManagerButton), new PropertyMetadata(Brushes.DarkGray));

        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public String SubLabel
        {
            get { return (String)GetValue(SubLabelProperty); }
            set { SetValue(SubLabelProperty, value); }
        }

        public Brush SubLabelColor
        {
            get { return (Brush)GetValue(SubLabelColorProperty); }
            set { SetValue(SubLabelColorProperty, value); }
        }
    }
}
