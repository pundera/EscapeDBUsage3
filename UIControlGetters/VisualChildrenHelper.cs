using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EscapeDBUsage.UIControlGetters
{
    public static class VisualChildrenHelper
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            T ret = null;

            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        if (child.GetType() is T)
                        {
                            ret = child as T;
                            break;
                        }
                        ret = FindVisualChild<T>(child);
                    }
                }
            }

            return ret;
        }

        public static void FreezeAll(DependencyObject depObj)
        {
            foreach (Freezable item in FindVisualChildren<Freezable>(depObj))
              if( item != null && item.CanFreeze)item.Freeze();
        }

    }
}
