
using System.Windows;

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// Interaction logic for ModalMessageDialog.xaml
    /// </summary>
    public partial class ModalMessageDialog
    {
        public ModalMessageDialog()
        {
            InitializeComponent();
            _response = DialogResponse.Cancel;
        }

        internal DialogResponse ShowMessage(string message, string header, DialogType dialogueType, Window owner)
        {
            Owner = owner;
            DataTemplate figureTemplate = null;

            switch (dialogueType)
            {
                case DialogType.Message:
                    figureTemplate = Application.Current.TryFindResource("InformationTemplate") as DataTemplate;
                    btnCancel.Visibility = btnYes.Visibility = btnNo.Visibility = Visibility.Collapsed;
                    break;
                case DialogType.Error:
                    figureTemplate = Application.Current.TryFindResource("ErrorTemplate") as DataTemplate;
                    btnCancel.Visibility = btnYes.Visibility = btnNo.Visibility = Visibility.Collapsed;
                    break;
                case DialogType.Question:
                    figureTemplate = Application.Current.TryFindResource("IterrogationTemplate") as DataTemplate;
                    btnCancel.Visibility = btnOk.Visibility = Visibility.Collapsed;
                    break;
                case DialogType.QuestionWithCancel:
                    figureTemplate = Application.Current.TryFindResource("IterrogationTemplate") as DataTemplate;
                    btnOk.Visibility = Visibility.Collapsed;
                    break;
            }

            if (null != figureTemplate)
                imageShower.ContentTemplate = figureTemplate;

            txtHeader.Text = header;
            txtBody.Text = message;

            ShowDialog();
            return _response;
        }

        #region Private Event Handlers

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            _response = DialogResponse.Yes;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            _response = DialogResponse.No;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _response = DialogResponse.Ok;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _response = DialogResponse.Cancel;
            Close();
        }

        #endregion

        #region Private Members

        private DialogResponse _response;

        #endregion
    }
}
