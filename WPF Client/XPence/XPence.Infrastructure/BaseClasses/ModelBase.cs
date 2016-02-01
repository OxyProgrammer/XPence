/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.ComponentModel;

namespace XPence.Infrastructure.BaseClasses
{
    /// <summary>
    /// An abstract base class for all models used in the pplication.
    /// </summary>
    public abstract class ModelBase:IDataErrorInfo
    {
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

        #endregion
        
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
    }
}
