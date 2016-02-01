/*************************************************************************************************************************************
 * Have been copied from http://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
 * ***********************************************************************************************************************************/
using System.Windows;
using System.Collections.Specialized;

namespace XPence.PushBinding
{
    public class PushBindingCollection : FreezableCollection<PushBinding>
    {
        public PushBindingCollection(FrameworkElement targetObject)
        {
            TargetObject = targetObject;
            ((INotifyCollectionChanged)this).CollectionChanged += CollectionChanged;
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PushBinding pushBinding in e.NewItems)
                {
                    pushBinding.SetupTargetBinding(TargetObject);
                }
            }
        }

        public FrameworkElement TargetObject
        {
            get;
            private set;
        }
    }
}
