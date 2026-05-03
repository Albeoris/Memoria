using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
    public class Vector4Editor : TypeEditor<Vector4Control>
    {
        protected override void SetValueDependencyProperty()
        {
            this.ValueProperty = Vector4Control.ValueProperty;
        }
    }
}
