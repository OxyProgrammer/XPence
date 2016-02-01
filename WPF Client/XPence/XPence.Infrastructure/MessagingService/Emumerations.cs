/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// Type of dialogue.
    /// </summary>
    public enum DialogType
    {
        Message = 0,
        Error,
        Question,
        QuestionWithCancel
    }

    /// <summary>
    /// The result as returned by the service.
    /// </summary>
    public enum DialogResponse
    {
        Ok = 0,
        Yes,
        No,
        Cancel,
    }
}
