using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using XPence.Infrastructure.MessagingService;
using XPence.Infrastructure.Utility;
using XPence.Shared;
using XPence.ViewModels;
using XPence.Views;

namespace XPence
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MainWindowViewModel _viewModel;

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogUtil.LogError("App","AppDispatcherUnhandledException",e.Exception);
            e.Handled = true;
        }

        static App()
        {
            // This code is used change the date time format.
            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            //System.Threading.Thread.CurrentThread.CurrentCulture =
            //    System.Threading.Thread.CurrentThread.CurrentUICulture =
            //        new System.Globalization.CultureInfo("it-IT");


            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            //
            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement),
              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LogUtil.LogInfo("App", "OnStartup", "Staring up.");
            base.OnStartup(e);
            var window = new Shell();
            // Create the ViewModel to which 
            // the main window binds.
            _viewModel = new MainWindowViewModel(MessageServiceFactory.GetMessagingServiceInstance());
            //Set the view models to the windows data context.
            window.DataContext = _viewModel;
            window.Show();

            //Register views for secondary windows.
            ModalViewRegistry.Instance.RegisterView(Constants.HELP_VIEW, typeof(HelpView));
            ModalViewRegistry.Instance.RegisterView(Constants.PICTURE_PICKER_VIEW, typeof(ImagePickerView));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (null != _viewModel)
                _viewModel.Dispose();
            LogUtil.LogInfo("App", "OnExit", "Application closing.");
        }
    }
}
