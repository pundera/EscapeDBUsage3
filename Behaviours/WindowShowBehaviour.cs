using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EscapeDBUsage.Behaviours
{
    public static class WindowShowBehaviour
    {
        public static void SetShow(DependencyObject target, bool value)
        {
            target.SetValue(ShowProperty, value);
        }

        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(bool), typeof(WindowShowBehaviour), new UIPropertyMetadata(false, OnShow));

        private static void OnShow(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);
                if (window != null)
                    window.ShowDialog();
            }
        }

        private static Window GetWindow(DependencyObject sender)
        {
            Window window = null;
            if (sender is Window)
                window = (Window)sender;
            if (window == null)
                window = Window.GetWindow(sender);
            return window;
        }
    }
}
