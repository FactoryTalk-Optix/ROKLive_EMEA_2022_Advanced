#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.Retentivity;
using FTOptix.Report;
using FTOptix.DataLogger;
#endregion

public class LoginChangePasswordButtonLogic : BaseNetLogic
{
    [ExportMethod]
    public void PerformChangePassword(string oldPassword, string newPassword, string confirmPassword, out int resultCode)
    {
        if (newPassword != confirmPassword)
        {
            resultCode = 7;
            return;
        }

        var username = Session.User.BrowseName;
        try
        {
            var userWithExpiredPassword = Owner.GetAlias("UserWithExpiredPassword");
            if (userWithExpiredPassword != null)
                username = userWithExpiredPassword.BrowseName;
        }
        catch
        {
        }

        var result = Session.ChangePassword(username, newPassword, oldPassword);
        resultCode = (int)result.ResultCode;

        var parentDialog = Owner.Owner?.Owner?.Owner as Dialog;
        if (parentDialog != null && result.Success)
            parentDialog.Close();
    }
}
