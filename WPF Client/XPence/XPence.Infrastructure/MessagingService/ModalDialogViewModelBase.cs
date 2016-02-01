/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Windows.Input;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// A base class for all view Models that cater to modal dialogs.
    /// </summary>
    public abstract class ModalDialogViewModelBase : ViewModelBase
    {
        #region Public Members

        /// <summary>
        /// Gets or sets if the user has cancelled the dialog.
        /// </summary>
        public bool IsCancelled
        {
            get { return _isCancelled; }
            set
            {
                if (value != _isCancelled)
                {
                    _isCancelled = value;
                    OnPropertyChanged(GetPropertyName(() => IsCancelled));
                }
            }
        }

        /// <summary>
        /// Gets or sets if the User has chosen Ok for the dialog.
        /// </summary>
        public bool IsOk
        {
            get { return _isOk; }
            set
            {
                if (value != _isOk)
                {
                    _isOk = value;
                    OnPropertyChanged(GetPropertyName(() => IsOk));
                }
            }
        }

        /// <summary>
        /// Gets or sets the title text for the window.
        /// </summary>
        public string TitleText
        {
            get { return _titleText; }
            set
            {
                if (value != _titleText)
                {
                    _titleText = value;
                    OnPropertyChanged(GetPropertyName(() => TitleText));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the Ok dialog command.
        /// Bind this to the button that that selects Ok option for the dialog.
        /// </summary>
        public ICommand OkSelectedCommand { get; private set; }

        /// <summary>
        /// Gets the cancelled dialog command. 
        /// Bind this to the button that cancels the dialog.
        /// </summary>
        public ICommand CancelSelectedCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initilaizes instance of <see cref="ModalDialogViewModelBase"/>
        /// </summary>
        protected ModalDialogViewModelBase()
        {
            OkSelectedCommand = new RelayCommand(OkSelected);
            CancelSelectedCommand = new RelayCommand(CancelSelected);
        }

        #endregion

        #region Protected Overidable methods

        /// <summary>
        /// Fired on OkCommand selected.
        /// </summary>
        protected virtual void OnOkSelected()
        {

        }

        /// <summary>
        /// Fired on Cancel selected.
        /// </summary>
        protected virtual void OnCancelSelected()
        {

        }

        #endregion

        #region Internal Events

        /// <summary>
        /// An event used internally by the <see cref="ModalCustomMessageDialog"/> to listen to the Ok or cancelled option selected by user.
        /// </summary>
        internal event EventHandler DialogResultSelected
        {
            add { _dialogResultSelected += value; }
            remove { _dialogResultSelected -= value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Caters the <see cref="OkSelectedCommand"/>.
        /// </summary>
        private void OkSelected()
        {
            IsOk = true;
            NotifyView(DialogResponse.Ok);
        }

        /// <summary>
        /// Caters the <see cref="CancelSelectedCommand"/>.
        /// </summary>
        private void CancelSelected()
        {
            IsCancelled = true;
            NotifyView(DialogResponse.Cancel);
        }

        /// <summary>
        /// Fires the <see cref="DialogResultSelected"/> event.
        /// </summary>
        /// <param name="dialogResponse"></param>
        private void NotifyView(DialogResponse dialogResponse)
        {
            if (null != _dialogResultSelected)
                _dialogResultSelected(this, EventArgs.Empty);
        }

        #endregion

        #region Private Members

        private bool _isCancelled;
        private bool _isOk;
        private string _titleText;
        internal event EventHandler _dialogResultSelected;

        #endregion
    }
}
