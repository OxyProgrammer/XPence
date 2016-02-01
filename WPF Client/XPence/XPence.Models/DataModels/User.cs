
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.Utility;
using XPence.Shared;

namespace XPence.Models.DataModels
{
    /// <summary>
    /// A model class for user.
    /// </summary>
    public class User : ModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id of the user.
        /// This is an identifier property.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of a user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                _encryptedPassword = Util.Encrypt(_password);
            }
        }

        /// <summary>
        /// Gets or sets the encrypted password of the user.
        /// </summary>
        public string EncryptedPassword
        {
            get { return _encryptedPassword; }
            set
            {
                _encryptedPassword = value;
                _password = Util.DecryptValue(_encryptedPassword);
            }
        }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the selected theme by the user.
        /// </summary>
        public string SelectedTheme { get; set; }

        /// <summary>
        /// Gets or sets the selected accent by the user.
        /// </summary>
        public string SelectedAccent { get; set; }

        /// <summary>
        /// Gets or sets the user's picture.
        /// </summary>
        public byte[] Picture { get; set; }

        #endregion

        #region Overriden members

        /// <summary>
        /// Validates the property values against their names.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string GetErrorForProperty(string propertyName)
        {
            string error = null;
            switch (propertyName)
            {
                case "Name":
                    if (string.IsNullOrEmpty(Name))
                        error = ErrorMessages.ERR_NAME_CANNOT_BE_EMPTY;
                    else if (Name.Length > 30)
                        error = ErrorMessages.ERR_NAME_CANNOT_BE_LONG;
                    break;
                case "Username":
                    if (string.IsNullOrEmpty(Username))
                        error = ErrorMessages.ERR_USERNAME_CANNOT_BE_EMPTY;
                    else if (Username.Length > 10)
                        error = ErrorMessages.ERR_USERNAME_CANNOT_BE_LONG;
                    break;
            }

            return error;
        }

        #endregion

        private string _password;
        private string _encryptedPassword;
    }
}
