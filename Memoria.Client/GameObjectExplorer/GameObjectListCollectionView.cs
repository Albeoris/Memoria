using JetBrains.Annotations;
using Memoria.Test;
using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;

namespace Memoria.Client
{
    public sealed class GameObjectListCollectionView : ListCollectionView
    {
        private static readonly SortDescription SortByTitleAscending = CreateSortByTitle(ListSortDirection.Ascending);

        public GameObjectListCollectionView([NotNull] GameObjectObservableCollection observableCollection)
            : base(observableCollection)
        {
            this.SortDescriptions.Add(SortByTitleAscending);
            this.IsLiveFiltering = true;
        }

        private static SortDescription CreateSortByTitle(ListSortDirection direction)
        {
            return new SortDescription("Title", direction);
        }
    }
}
