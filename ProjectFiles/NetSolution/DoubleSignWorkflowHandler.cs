#region Using directives

using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.Store;
using FTOptix.UI;
using FTOptix.EventLogger;
using FTOptix.SQLiteStore;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.AuditSigning;
using FTOptix.Report;
using FTOptix.DataLogger;

#endregion

public class DoubleSignWorkflowHandler : BaseNetLogic
{
    private Button _confirmButton;

    public override void Start()
    {
        _confirmButton = Owner.Get<Button>("Content/Buttons/Confirm");
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void CheckResult(SignResult signResult)
    {
        switch(signResult)
        {
            case SignResult.Succeeded:
                {
                    var auditDialog = (AuditDialogBox)LogicObject.GetAlias("AuditDialog");
                    auditDialog.Close();
                    return;
                }
            case SignResult.FirstUserLoginFailed:
                {
                    ProcessWorkflowResult("Login failed for user", 1);
                    return;
                }
            case SignResult.SecondUserLoginFailed:
                {
                    ProcessWorkflowResult("Login failed for user", 2);
                    return;
                }
            case SignResult.FirstUserNotAuthorized:
                {
                    ProcessWorkflowResult("user is not authorized", 1);
                    return;
                }
            case SignResult.SecondUserNotAuthorized:
                {
                    ProcessWorkflowResult("user is not authorized", 2);
                    return;
                }
        }
    }

    private void ProcessWorkflowResult(string outputText, int approverNumber) {
        //Log.Error(outputText);
        var wrongPasswordDialog = (DialogType)((IUAObject)LogicObject.Owner).ObjectType.Owner.Get("WorkflowFailDialog");
        string approverUserName = "";
        if (approverNumber == 1) {
            approverUserName = InformationModel.Get(Owner.Get<ComboBox>("Content/Columns/LeftCol/Line1/FirstUserName").SelectedItem).BrowseName;
        } else {
            approverUserName = InformationModel.Get(Owner.Get<ComboBox>("Content/Columns/RightCol/Line1/FirstUserName").SelectedItem).BrowseName;
        }
        
        wrongPasswordDialog.Get<ColumnLayout>("VerticalLayout1").Get<Label>("User").Text = outputText.Replace("user", approverUserName);
        _confirmButton.OpenDialog(wrongPasswordDialog);
    }
}
