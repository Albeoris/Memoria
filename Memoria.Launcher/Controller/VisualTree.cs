using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Memoria.Launcher.Controller
{
    internal static class VisualTree
    {
        public static IEnumerable<T> Enumerate<T>(DependencyObject root) where T : DependencyObject
        {
            if (root == null)
                yield break;

            if (root is T typed)
                yield return typed;

            Int32 count = VisualTreeHelper.GetChildrenCount(root);
            for (Int32 index = 0; index < count; index++)
            {
                foreach (T descendant in Enumerate<T>(VisualTreeHelper.GetChild(root, index)))
                    yield return descendant;
            }
        }

        public static T FindAncestor<T>(DependencyObject element, Boolean includeSelf) where T : DependencyObject
        {
            DependencyObject current = includeSelf ? element : GetParent(element);
            while (current != null)
            {
                if (current is T typed)
                    return typed;
                current = GetParent(current);
            }

            return null;
        }

        public static T FindDescendant<T>(DependencyObject root) where T : DependencyObject =>
            Enumerate<T>(root).FirstOrDefault();

        public static Boolean IsDescendantOf(DependencyObject element, DependencyObject ancestor)
        {
            DependencyObject current = element;
            while (current != null)
            {
                if (ReferenceEquals(current, ancestor))
                    return true;
                current = GetParent(current);
            }

            return false;
        }

        public static DependencyObject GetParent(DependencyObject element)
        {
            if (element == null)
                return null;
            if (element is Visual || element is System.Windows.Media.Media3D.Visual3D)
                return VisualTreeHelper.GetParent(element);
            if (element is ContentElement content)
                return ContentOperations.GetParent(content) ?? LogicalTreeHelper.GetParent(element);
            return LogicalTreeHelper.GetParent(element);
        }

        public static Int32 GetDepth(DependencyObject element)
        {
            Int32 depth = 0;
            while ((element = GetParent(element)) != null)
                depth++;
            return depth;
        }
    }
}
