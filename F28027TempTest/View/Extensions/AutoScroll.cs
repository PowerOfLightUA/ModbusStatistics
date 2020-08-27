using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace F28027TempTest.View.Extensions
{
    public class AutoScroller : Behavior<ScrollViewer>
    {
        public object AutoScrollTrigger
        {
            get { return (object)GetValue(AutoScrollTriggerProperty); }
            set { SetValue(AutoScrollTriggerProperty, value); }
        }

        public static readonly DependencyProperty AutoScrollTriggerProperty =
            DependencyProperty.Register(
                "AutoScrollTrigger",
                typeof(object),
                typeof(AutoScroller),
                new PropertyMetadata(null, ASTPropertyChanged));


        public bool AutoScroll
        {
            get { return (bool)GetValue(AutoScrollProperty); }
            set { SetValue(AutoScrollProperty, value); }
        }

        public static readonly DependencyProperty AutoScrollProperty = DependencyProperty.Register(
                        "AutoScroll",
                        typeof(bool),
                        typeof(AutoScroller),
                        new PropertyMetadata(false));


        private static void ASTPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var ts = d as AutoScroller;
            if (ts == null)
                return;

            // must be attached to a ScrollViewer
            var sv = ts.AssociatedObject as ScrollViewer;

            // check if we are attached to an ObservableCollection, in which case we
            // will subscribe to CollectionChanged so that we scroll when stuff is added/removed
            var ncol = args.NewValue as INotifyCollectionChanged;
            // new event handler
            if (ncol != null)
                ncol.CollectionChanged += ts.OnCollectionChanged;

            // remove old eventhandler
            var ocol = args.OldValue as INotifyCollectionChanged;
            if (ocol != null)
                ocol.CollectionChanged -= ts.OnCollectionChanged;


            // also scroll to bottom when the bound object itself changes
            if (sv != null && ts.AutoScroll)
                sv.ScrollToBottom();
        }

        private void OnCollectionChanged(object sender, EventArgs args)
        {
            if (AutoScroll)
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    (this.AssociatedObject as ScrollViewer).ScrollToBottom();
                });
            }
        }
    }
}
