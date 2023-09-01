#region Using directives
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.Core;
using FTOptix.UI;
using FTOptix.Report;
using FTOptix.DataLogger;
#endregion

public class LoginButtonLogic : BaseNetLogic
{
    [ExportMethod]
    public void PerformLogin(string username, string password)
    {
        var usersAlias = LogicObject.GetAlias("Users");
        if (usersAlias == null || usersAlias.NodeId == NodeId.Empty)
        {
            Log.Error("LoginButtonLogic", "Missing Users alias");
            return;
        }

        var user = usersAlias.Get<User>(username);
        if (user == null)
        {
            Log.Error("LoginButtonLogic", "Could not find user " + username);
            return;
        }

        var passwordExpiredDialogType = LogicObject.GetAlias("PasswordExpiredDialogType") as DialogType;
        if (passwordExpiredDialogType == null)
        {
            Log.Error("LoginButtonLogic", "Missing PasswordExpiredDialogType alias");
            return;
        }

        try
        {
            var loginResult = Session.Login(username, password);
            if (loginResult.ResultCode == ChangeUserResultCode.PasswordExpired)
            {
                var ownerButton = (Button)Owner;
                ownerButton.OpenDialog(passwordExpiredDialogType, user.NodeId);
                return;
            }

            var outputMessageLabel = Owner.Owner.GetObject("LoginFormOutputMessage");
            var outputMessageLabelResultCode = outputMessageLabel.GetVariable("LoginResultCode");
            outputMessageLabelResultCode.Value = loginResult.Success ? 0 : 1;

            string outputMessage = outputMessageLabel.GetVariable("Message").Value;
            var outputMessageLogic = outputMessageLabel.GetObject("LoginFormOutputMessageLogic");
            outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { outputMessage });
        }
        catch (Exception e)
        {
            Log.Error("LoginButtonLogic", e.Message);
        }
    }

}
