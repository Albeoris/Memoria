using Memoria.Client.Interaction;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Memoria.Client
{
	public sealed class UiGameObjectContent : UiGrid
	{
		private readonly TabControl _tabControl;
		private readonly TreeView _treeView;
		private readonly PropertyGrid _propertyGrid;

		public UiGameObjectContent()
		{
			#region Constructor

			SetCols(2);

			_tabControl = new TabControl();
			_tabControl.IsVisibleChanged += OnTabControlIsVisibleChanged;
			_treeView = CreateNewTab("All objects");
			AddUiElement(_tabControl, 0, 0);

			_propertyGrid = new PropertyGrid { AutoGenerateProperties = true };
			AddUiElement(_propertyGrid, 0, 1);

			#endregion Constructor

			InteractionService.GameObjectsController = this;
			InteractionService.RemoteGameObjects.InfoLost += RemoveItems;
			InteractionService.RemoteGameObjects.InfoProvided += ChangeItems;
		}

		private void OnTabControlIsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_tabControl.SelectedItem == null)
				_tabControl.SelectedItem = _tabControl.Items[0];
		}

		public TreeView CreateNewTab(String header)
		{
			return (TreeView)CreateNewTabInternal(header).Content;
		}

		private TabItem CreateNewTabInternal(String header)
		{
			Tree treeView = new Tree();
			treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;

			TabItem tabItem = new TabItem { Header = header };
			tabItem.Content = treeView;

			ContextMenu contextMenu = new ContextMenu();
			FilterCommand filterCommand = new FilterCommand(this, treeView);
			contextMenu.Items.Add(new MenuItem { Header = "Filter", Command = filterCommand });

			if (_tabControl.Items.Count > 0)
			{
				CloseTabCommand closeTabCommand = new CloseTabCommand(_tabControl, tabItem);

				contextMenu.Items.Add(new MenuItem { Header = "Close", Command = closeTabCommand });

				tabItem.MouseDown += (s, e) =>
				{
					if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
						closeTabCommand.Execute(null);
				};
			}

			tabItem.ContextMenu = contextMenu;

			_tabControl.Items.Add(tabItem);
			_tabControl.SelectedItem = tabItem;
			tabItem.Focus();

			return tabItem;
		}

		public void CreateFilterTab(String header, String filter)
		{
			TabItem result = CreateNewTabInternal(header);
			Tree treeView = (Tree)result.Content;
			ItemCollection root = treeView.Items;

			Dictionary<ObjectAbstractView, Boolean> dic = new Dictionary<ObjectAbstractView, Boolean>();
			foreach (ObjectAbstractView item in _treeView.Items)
			{
				if (IsNeedToShowInternal(item, dic))
				{
					treeView.Items.Add(item);
				}
			}
		}

		private sealed class CloseTabCommand : ICommand
		{
			private readonly TabControl _tabControl;
			private readonly TabItem _tabItem;

			public CloseTabCommand(TabControl tabControl, TabItem tabItem)
			{
				_tabControl = tabControl;
				_tabItem = tabItem;
			}

			public Boolean CanExecute(Object parameter)
			{
				return true;
			}

			public void Execute(Object parameter)
			{
				_tabControl.Items.Remove(_tabItem);
			}

			public event EventHandler CanExecuteChanged;
		}

		private sealed class FilterCommand : ICommand
		{
			private readonly UiGameObjectContent _content;
			private readonly Tree _treeView;

			public FilterCommand(UiGameObjectContent content, Tree treeView)
			{
				_content = content;
				_treeView = treeView;
			}

			public Boolean CanExecute(Object parameter)
			{
				return true;
			}

			public void Execute(Object parameter)
			{
				_content.CreateFilterTab("Filter", "Yes");
			}

			public event EventHandler CanExecuteChanged;
		}

		private void RemoveItems(RemoteGameObjects oldObjects)
		{
			ChangeItems(null);
		}

		public void ChangeItems(RemoteGameObjects newObjects)
		{
			if (newObjects == null)
			{
				ChangeItemSource(null);
				return;
			}

			ListCollectionView items = newObjects.GetRootObjectsView();
			ChangeItemSource(items);
		}

		private void ChangeItemSource(ListCollectionView views)
		{
			if (CheckAccess())
			{
				Object firstTab = _tabControl.Items[0];
				_tabControl.Items.Clear();
				_tabControl.Items.Add(firstTab);

				_treeView.ItemsSource = views;
			}
			else
			{
				_treeView.Dispatcher.BeginInvoke(new Action(() => ChangeItemSource(views)), DispatcherPriority.DataBind);
			}
		}

		private void OnTreeViewSelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			_propertyGrid.SelectedObject = ((TreeView)sender).SelectedItem as ObjectAbstractView;
		}

		// =======================================================================

		private sealed class Tree : TreeView
		{
			public Tree()
			{
				ItemTemplate = CreateDataTemplate();
			}

			private DataTemplate CreateDataTemplate()
			{
				HierarchicalDataTemplate template = new HierarchicalDataTemplate
				{
					ItemsSource = new Binding("BindableChilds")
				};

				FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
				textBlock.SetBinding(TextBlock.TextProperty, new Binding("Title"));
				textBlock.SetBinding(ContextMenuProperty, new Binding("BindableContextMenu"));

				template.VisualTree = textBlock;
				return template;
			}
		}

		private Boolean IsNeedToShowInternal(ObjectAbstractView view, Dictionary<ObjectAbstractView, Boolean> dic)
		{
			Boolean result;
			if (dic.TryGetValue(view, out result))
				return result;

			if (view.Title.Contains("Yes"))
			{
				dic.Add(view, true);
				return true;
			}

			foreach (ObjectAbstractView child in view.BindableChilds)
			{
				if (IsNeedToShowInternal(child, dic))
				{
					dic.Add(view, true);
					return true;
				}
			}

			dic.Add(view, false);
			return false;
		}

		public Boolean IsNeedToShow(ObjectAbstractView view)
		{
			return IsNeedToShowInternal(view);
			//return IsNeedToShowStack.GetOrAdd(view, IsNeedToShowInternal);
		}

		private Boolean IsNeedToShowInternal(ObjectAbstractView view)
		{
			if (view.Title.Contains("Yes"))
				return true;

			return view.BindableChilds.Count > 0;
		}

		//private static readonly ConcurrentDictionary<ObjectAbstractView, Boolean> IsNeedToShowStack = new ConcurrentDictionary<ObjectAbstractView, Boolean>();
	}
}
