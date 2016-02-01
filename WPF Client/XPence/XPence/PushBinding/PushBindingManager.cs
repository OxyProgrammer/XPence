/*************************************************************************************************************************************
 * Have been copied from http://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
 * ***********************************************************************************************************************************/
using System.Windows;

namespace XPence.PushBinding
{
    public class PushBindingManager
    {
        public static DependencyProperty PushBindingsProperty =
            DependencyProperty.RegisterAttached("PushBindingsInternal",
                                                typeof(PushBindingCollection),
                                                typeof(PushBindingManager),
                                                new UIPropertyMetadata(null));

        public static PushBindingCollection GetPushBindings(FrameworkElement obj)
        {
            if (obj.GetValue(PushBindingsProperty) == null)
            {
                obj.SetValue(PushBindingsProperty, new PushBindingCollection(obj));
            }
            return (PushBindingCollection)obj.GetValue(PushBindingsProperty);
        }
        public static void SetPushBindings(FrameworkElement obj, PushBindingCollection value)
        {
            obj.SetValue(PushBindingsProperty, value);
        }
    }
}
