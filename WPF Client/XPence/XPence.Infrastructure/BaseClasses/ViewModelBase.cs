/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows;

namespace XPence.Infrastructure.BaseClasses
{
    /// <summary>
    /// A base class for all view models.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, IDisposable,IDataErrorInfo
    {
        #region Public Methods

        /// <summary>
        /// Returns the property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression != null)
                return memberExpression.Member.Name;

            return null;
        }

        /// <summary>
        /// This method serves as a placeholder for code that requires self initialization of 
        /// Bindable Properties or state etc..
        /// </summary>
        public void Initialize()
        {
            OnInitialize();
            IsInitialized = true;
        }

        #endregion
        
        #region IDataErrorInfo Members

        /// <summary>
        /// Returns error against an object.
        /// </summary>
        public string Error
        {
            get { return null; }
        }

        /// <summary>
        /// Gets error against a property name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get { return GetErrorForProperty(columnName); }
        }


        #region Protected Members

        /// <summary>
        /// A virtual overridable method for returning an error against a property value.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual string GetErrorForProperty(string propertyName)
        {
            return null;
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler == null)
                return;
            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets and sets unique ViewName
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged(GetPropertyName(() => DisplayName));
            }
        }

        /// <summary>
        /// Gets if this instance of <see cref="ViewModelBase"/> is Initialized.
        /// Can be set in a child class.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method aides in running any method in the UI thread.
        /// </summary>
        /// <param name="routine">The method that needs to be run in a thread safe manner.</param>
        protected void ThreadSafeInvoke(Action routine)
        {
            var dispatcher = Application.Current.Dispatcher;
            if(null!=dispatcher)
            {
                if(!dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(routine);
                }
                else
                {
                    routine();
                }
            }
        }

        /// <summary>
        /// A protected overridable method for initializing.
        /// </summary>
        protected virtual void OnInitialize()
        {
            
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            IsInitialized = true;
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {

        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            var msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
            Debug.WriteLine(msg);
        }
#endif

        #endregion

        #region Private Members

        private string _displayName;

        #endregion

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                    throw new Exception(msg);

                Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion
    }
}
