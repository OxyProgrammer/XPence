/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.Windows;

namespace XPence.Infrastructure.Utility
{
    public static class Util
    {
        #region Static Constructor

        /// <summary>
        /// Static constructor to initialize static variables.
        /// </summary>
        static Util()
        {
            _salt = "funnySalt";
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the encrypter string value for a given string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            return RijndaelManagedEncryption.EncryptRijndael(value,_salt);
        }

        /// <summary>
        /// Gets the decrypted string value for a given encrypted string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecryptValue(string value)
        {
            return RijndaelManagedEncryption.DecryptRijndael(value, _salt);
        }

        #endregion

        public static Window AppMainWindow { get { return Application.Current.MainWindow; } }

        #region Member variables.

        private static string _salt;

        #endregion
    }
}
