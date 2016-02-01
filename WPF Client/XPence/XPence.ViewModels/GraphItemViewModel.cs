/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using XPence.Infrastructure.BaseClasses;

namespace XPence.ViewModels
{
    /// <summary>
    /// A UI friendly class responsible for catering to a section of the graph.
    /// </summary>
    public class GraphItemViewModel : ViewModelBase
    {
        #region Member Variables

        private string _description;
        private object _value;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the desciption of the graph itsm.
        /// This is sort of group key, the sections are formed by grouping over this key.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value)
                    return;
                _description = value;
                OnPropertyChanged(GetPropertyName(() => Description));
            }
        }

        /// <summary>
        /// Gets or sets a value associated to a a description.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                OnPropertyChanged(GetPropertyName(() => Value));
            }
        }

        #endregion
    }
}