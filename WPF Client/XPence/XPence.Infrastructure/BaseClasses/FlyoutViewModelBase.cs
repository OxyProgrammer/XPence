/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

namespace XPence.Infrastructure.BaseClasses
{
    public abstract class FlyoutViewModelBase : ViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the header text of the flyout instance.
        /// </summary>
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header == value)
                    return;
                _header = value;
                OnPropertyChanged(GetPropertyName(() => Header));
            }
        }

        /// <summary>
        /// Gets or sets the position of the flyout instance.
        /// </summary>
        public VisibilityPosition Position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                    return;
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Gets or sets if the flyout insatnce is visible or collapsed.
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen == value)
                    return;
                _isOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }

        /// <summary>
        /// Gets or sets the theme of the flyout instance.
        /// </summary>
        public FlyoutTheme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme == value)
                    return;
                _theme = value;
                OnPropertyChanged("Theme");
            }
        }

        #endregion

        #region protected members.

        protected string _header;
        protected VisibilityPosition _position;
        protected bool _isOpen;
        protected FlyoutTheme _theme;

        #endregion

    }

    /// <summary>
    /// Enumeration distinguising the position of a flyout.
    /// </summary>
    public enum VisibilityPosition
    {
        Right = 0,
        Bottom,
        Left,
        Top
    };

    /// <summary>
    /// Enumeration distinguishing the theme of a flyout.
    /// </summary>
    public enum FlyoutTheme
    {
        AccentedTheme = 0,
        BaseColorTheme,
        InverseTheme,

    }
}
