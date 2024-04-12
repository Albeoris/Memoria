using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
	public class RectEditor : TypeEditor<RectControl>
	{
		protected override void SetValueDependencyProperty()
		{
			this.ValueProperty = RectControl.ValueProperty;
		}
	}
}
