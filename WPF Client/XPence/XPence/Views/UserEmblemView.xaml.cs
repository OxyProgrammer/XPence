
using System.Windows;
using System.Windows.Controls;

namespace XPence.Views
{
    /// <summary>
    /// Interaction logic for UserEmblemView.xaml
    /// </summary>
    public partial class UserEmblemView
    {
        public UserEmblemView()
        {
            InitializeComponent();
            SetAppearance();
        }

        /// <summary>
        /// Gets or sets if this instance of <see cref="UserEmblemView"/> is used in a template.
        /// </summary>
        public bool IsTemplate
        {
            get { return _isTemplate; }
            set
            {
                if (_isTemplate == value) 
                    return;
                _isTemplate = value;
                SetAppearance();
            }
        }

        /// <summary>
        /// Sets appearance as per requirement.
        /// </summary>
        private void SetAppearance()
        {
            if(_isTemplate)
            {
                roleLogoFigure.Visibility = Visibility.Visible;
                txtName.SetResourceReference(TextBlock.ForegroundProperty, "BlackBrush");
            }
            else
            {
                roleLogoFigure.Visibility = Visibility.Collapsed;
                txtName.SetResourceReference(TextBlock.ForegroundProperty,"AccentColorBrush");
            }
        }

        #region Member Variables

        private bool _isTemplate;

        #endregion

    }
}
