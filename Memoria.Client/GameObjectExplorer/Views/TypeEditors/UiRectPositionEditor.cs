using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
	public class UiRectPositionEditor : TypeEditor<UiRectPositionControl>
	{
		protected override void SetValueDependencyProperty()
		{
			this.ValueProperty = UiRectPositionControl.ValueProperty;
		}
	}
}
