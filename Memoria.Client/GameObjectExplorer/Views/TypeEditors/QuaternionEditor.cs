using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
    public class QuaternionEditor : TypeEditor<QuaternionControl>
    {
        protected override void SetValueDependencyProperty()
        {
            this.ValueProperty = QuaternionControl.ValueProperty;
        }
    }
}
