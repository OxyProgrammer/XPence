/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;

namespace XPence.Infrastructure.CoreClasses
{
    /// <summary>
    /// Extends ObservableCollection<T> for adding collection to ObservableCollection
    /// Provides a flag for thread safe operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        #region Private Members

        /// <summary>
        /// Whether to suppress notification or not
        /// </summary>
        private bool _suppressNotification;

        /// <summary>
        /// Variable used for lock purpose
        /// </summary>
        private object _syncObject;

        /// <summary>
        /// whether thread safety is required or not
        /// </summary>
        private bool _threadSafetyRequired;

        #endregion

        #region Base Class Overrides

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises CollectionChanged event if notification need not be suppressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressNotification)
                return;

            base.OnCollectionChanged(e);
            OnCollectionChangedMultiItem(e);
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Raises notification after range operation
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handlers = CollectionChanged;

            if (handlers == null)
                return;

            foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
            {
                if (handler.Target is CollectionView)
                    ((CollectionView)handler.Target).Refresh();
                else
                    handler(this, e);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of ExtendedObservableCollection
        /// </summary>
        /// <param name="threadSafetyRequired"></param>
        public ExtendedObservableCollection(bool threadSafetyRequired)
        {
            Initialize(threadSafetyRequired);
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ExtendedObservableCollection()
            : this(false)
        {
        }

        /// <summary>
        /// Creates an ExtendedObservableCollection using input list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="threadSafetyRequired"></param>
        public ExtendedObservableCollection(IEnumerable<T> list, bool threadSafetyRequired)
            : base(list)
        {
            Initialize(threadSafetyRequired);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds collection to ExtendedObservableCollection
        /// </summary>
        /// <param name="list"></param>
        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                return;
            if (_threadSafetyRequired)
                lock (_syncObject)
                {
                    AddBulk(list);
                }
            else
                AddBulk(list);
        }

        /// <summary>
        /// Removes item at specified index.
        /// Hides the member with same name from the base class.
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            if (_threadSafetyRequired)
                lock (_syncObject)
                {
                    base.RemoveAt(index);
                }
            else
                base.RemoveAt(index);
        }

        /// <summary>
        /// Adds an item to the collection.
        /// Hides the member with same name from the base class.
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            if (_threadSafetyRequired)
                lock (_syncObject)
                {
                    base.Add(item);
                }
            else
                base.Add(item);
        }

        /// <summary>
        /// Removes an item to the collection.
        /// Hides the member with same name from the base class.
        /// </summary>
        /// <param name="item"></param>
        public new void Remove(T item)
        {
            if (_threadSafetyRequired)
                lock (_syncObject)
                {
                    base.Remove(item);
                }
            else
                base.Remove(item);
        }

        /// <summary>
        /// Returns synchronization lock
        /// </summary>
        /// <returns></returns>
        public object ReturnLock()
        {
            return _syncObject;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds all the items in a bulk.
        /// </summary>
        /// <param name="list"></param>
        private void AddBulk(IEnumerable<T> list)
        {
            bool isListHasItem = false;
            _suppressNotification = true;
            foreach (T item in list)
            {
                Add(item);
                isListHasItem = true;
            }
            _suppressNotification = false;

            if (isListHasItem)
            {
                var obEvtArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,list);
                OnCollectionChangedMultiItem(obEvtArgs);
            }
        }

        /// <summary>
        /// Initializes the instance of ExtendedObservableCollection in initializing required variables..
        /// </summary>
        /// <param name="threadSafetyRequired"></param>
        private void Initialize(bool threadSafetyRequired)
        {
            _threadSafetyRequired = threadSafetyRequired;
            if (_threadSafetyRequired)
                _syncObject = new object();
        }

        #endregion
    }
}
