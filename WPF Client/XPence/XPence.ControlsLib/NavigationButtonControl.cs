/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XPence.ControlsLib
{
    /// <summary>
    /// A replacement for <see cref="System.Windows.Controls.Primitives.ButtonBase"/> class that lets the consumer have a content and overridable style.
    /// when mouse is pressed over it.
    /// </summary>
    public class NavigationButtonControl : ContentControl, ICommandSource
    {
        #region Dependency Properties.

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(NavigationButtonControl), new PropertyMetadata(null, CommandChanged));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(NavigationButtonControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(NavigationButtonControl), new PropertyMetadata(false));

        #endregion

        #region Public properties

        /// <summary>
        /// The is pressed property to indicate the state of the control
        /// when mouse is pressed over it.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        #endregion

        #region ICommandSource Members

        /// <summary>
        /// Gets or sets <see cref="ICommand"/> implementation that will be fired when the control is clicked.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// The prameter, the command would pass in the handling method.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return this; }
        }

        #endregion

        #region Private Static Event handlers

        /// <summary>
        /// Handles whent he Command dependency proeprty is changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NavigationButtonControl;
            if (null != control)
            {
                control.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Does the cleaning of old command and registers to the events of new commands.
        /// </summary>
        /// <param name="oldCommand"></param>
        /// <param name="newCommand"></param>
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers. 
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand);
            }
            AddCommand(newCommand);
        }

        /// <summary>
        /// Removes the CanExecuted handler from the old command.
        /// </summary>
        /// <param name="oldCommand"></param>
        private void RemoveCommand(ICommand oldCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        /// <summary>
        /// Add the event handlers for the new commands.
        /// </summary>
        /// <param name="newCommand"></param>
        private void AddCommand(ICommand newCommand)
        {
            var handler = new EventHandler(CanExecuteChanged);
            var canExecuteChangedHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }

        /// <summary>
        /// If can executed changes for the ICommand, the control should change its IsEnabled proeprty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (Command == null)
                return;
            var command = Command as RoutedCommand;
            //Different handling for routed commands and for plain implementation of ICommand.
            IsEnabled = command != null ? command.CanExecute(CommandParameter, CommandTarget) : Command.CanExecute(CommandParameter);
        }

        /// <summary>
        /// This method fires the command.
        /// </summary>
        private void FireAtWill()
        {
            if (Command == null)
                return;
            var command = Command as RoutedCommand;
            if (command != null)
            {
                command.Execute(CommandParameter, CommandTarget);
            }
            else
            {
                Command.Execute(CommandParameter);
            }
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Handles the Mouse down event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            IsPressed = true;
            FireAtWill();
        }

        /// <summary>
        /// Handles the mouse leave event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsPressed = false;
        }

        #endregion

        #region Static Constructor

        /// <summary>
        /// Static constructor to take care of the static properties.
        /// </summary>
        static NavigationButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationButtonControl),
                                                     new FrameworkPropertyMetadata(typeof(NavigationButtonControl)));
        }

        #endregion
    }

    #region More accurate NavigationButtonControl
    /*****************************************************************************
     * Below is the commented code for a more accurate NavigationButtonControl.
     * Mahapps metro seems to eat up the Mouse up event and hence mouse up event never 
     * gets fired in this control if used in a mahapps window.
     * The below control will act perfect if used within a normal WPF window.
     * ****************************************************************************/
    /*
    /// <summary>
    /// A replacement for <see cref="System.Windows.Controls.Primitives.ButtonBase"/> class that lets the consumer have a content and overridable style.
    /// when mouse is pressed over it.
    /// </summary>
    public class NavigationButtonControl : ContentControl, ICommandSource
    {
        #region Dependency Properties.

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(NavigationButtonControl), new PropertyMetadata(null, CommandChanged));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(NavigationButtonControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(NavigationButtonControl), new PropertyMetadata(false));

        #endregion

        #region Public properties

        /// <summary>
        /// The is pressed property to indicate the state of the control
        /// when mouse is pressed over it.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        #endregion

        #region ICommandSource Members

        /// <summary>
        /// Gets or sets <see cref="ICommand"/> implementation that will be fired when the control is clicked.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// The prameter, the command would pass in the handling method.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return this; }
        }

        #endregion

        #region Private Static Event handlers

        /// <summary>
        /// Handles whent he Command dependency proeprty is changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NavigationButtonControl;
            if (null != control)
            {
                control.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Does the cleaning of old command and registers to the events of new commands.
        /// </summary>
        /// <param name="oldCommand"></param>
        /// <param name="newCommand"></param>
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers. 
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand);
            }
            AddCommand(newCommand);
        }

        /// <summary>
        /// Removes the CanExecuted handler from the old command.
        /// </summary>
        /// <param name="oldCommand"></param>
        private void RemoveCommand(ICommand oldCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        /// <summary>
        /// Add the event handlers for the new commands.
        /// </summary>
        /// <param name="newCommand"></param>
        private void AddCommand(ICommand newCommand)
        {
            var handler = new EventHandler(CanExecuteChanged);
            var canExecuteChangedHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }

        /// <summary>
        /// If can executed changes for the ICommand, the control should change its IsEnabled proeprty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (Command == null)
                return;
            var command = Command as RoutedCommand;
            //Different handling for routed commands and for plain implementation of ICommand.
            IsEnabled = command != null ? command.CanExecute(CommandParameter, CommandTarget) : Command.CanExecute(CommandParameter);
        }

        /// <summary>
        /// This method fires the command.
        /// </summary>
        private void FireAtWill()
        {
            if (Command == null)
                return;
            var command = Command as RoutedCommand;
            if (command != null)
            {
                command.Execute(CommandParameter, CommandTarget);
            }
            else
            {
                Command.Execute(CommandParameter);
            }
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Handles the Mouse down event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _hasMouseDownOccurred = true;
            IsPressed = true;
        }

        /// <summary>
        /// Handles the mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_hasMouseDownOccurred)
            {
                IsPressed = false;
                _hasMouseDownOccurred = false;
                FireAtWill();
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Handles the mouse leave event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _hasMouseDownOccurred = false;
            IsPressed = false;
        }

        #endregion

        #region Static Constructor

        /// <summary>
        /// Static constructor to take care of the static properties.
        /// </summary>
        static NavigationButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (NavigationButtonControl),
                                                     new FrameworkPropertyMetadata(typeof (NavigationButtonControl)));
        }

        #endregion

        #region Member Variables

        private bool _hasMouseDownOccurred;

        #endregion
    }
     */
    #endregion
}
