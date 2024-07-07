using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
    public class Vector3Editor : TypeEditor<Vector3Control>
    {
        protected override void SetValueDependencyProperty()
        {
            this.ValueProperty = Vector3Control.ValueProperty;
        }
    }
}
