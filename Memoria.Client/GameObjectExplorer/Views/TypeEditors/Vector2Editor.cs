using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
	public class Vector2Editor : TypeEditor<Vector2Control>
	{
		protected override void SetValueDependencyProperty()
		{
			this.ValueProperty = Vector2Control.ValueProperty;
		}
	}
}
