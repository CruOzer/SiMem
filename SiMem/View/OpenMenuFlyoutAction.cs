using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace SiMem.View
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        private object parameter;
        public object Execute(object sender, object parameter)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            Debug.WriteLine(parameter);

            flyoutBase.ShowAt(senderElement);
            return null;
        }
    }
}
