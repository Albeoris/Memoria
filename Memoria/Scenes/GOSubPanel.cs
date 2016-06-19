using UnityEngine;

namespace Memoria
{
    internal sealed class GOSubPanel : GOBase
    {
        public readonly UIPanel Panel;
        public readonly UIScrollView ScrollView;
        public readonly Rigidbody Rigidbody;
        public readonly SnapDragScrollView SnapDragScrollView;
        public readonly RecycleListPopulator RecycleListPopulator;

        public readonly GOTable<GOBase> Table;

        public GOSubPanel(GameObject obj)
            : base(obj)
        {
            Panel = obj.GetExactComponent<UIPanel>();
            ScrollView = obj.GetExactComponent<UIScrollView>();
            Rigidbody = obj.GetExactComponent<Rigidbody>();
            SnapDragScrollView = obj.GetExactComponent<SnapDragScrollView>();
            RecycleListPopulator = obj.GetExactComponent<RecycleListPopulator>();

            Table = new GOTable<GOBase>(obj.GetChild(0));
        }
    }
}