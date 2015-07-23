using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BioGait
{
    public static class Utilities
    {
        public static IEnumerable<DependencyObject> GetLogicalChildren<T>(DependencyObject depObj)
        {
            if (depObj != null)
            {
                foreach (object obj in LogicalTreeHelper.GetChildren(depObj))
                {
                    var dependencyObject = obj as DependencyObject;

                    if (dependencyObject != null && dependencyObject is T)
                    {
                        yield return dependencyObject;
                    }

                    foreach (var childOfChild in GetLogicalChildren<T>(dependencyObject))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
                return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }

}
