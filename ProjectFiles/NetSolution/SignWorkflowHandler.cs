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

public class SignWorkflowHandler : BaseNetLogic {
    private Button _confirmButton;

    public override void Start() {
        _confirmButton = Owner.Get<Button>("Content/Line4/Confirm");
    }

    public override void Stop() {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void CheckResult(SignResult signResult) {
        switch (signResult) {
            case SignResult.Succeeded:
                var auditDialog = (AuditDialogBox)LogicObject.GetAlias("AuditDialog");
                auditDialog.Close();
                return;

            case SignResult.FirstUserLoginFailed:
                ProcessWorkflowResult("Login failed for user");
                return;

            case SignResult.FirstUserNotAuthorized:
                ProcessWorkflowResult("user is not authorized");
                return;
        }
    }

    private void ProcessWorkflowResult(string outputText) {
        //Log.Error(outputText);
        var wrongPasswordDialog = (DialogType)((IUAObject)LogicObject.Owner).ObjectType.Owner.Get("WorkflowFailDialog");
        var approverUserName = InformationModel.Get(Owner.Get<ComboBox>("Content/Line1/Username").SelectedItem).BrowseName;
        wrongPasswordDialog.Get<ColumnLayout>("VerticalLayout1").Get<Label>("User").Text = outputText.Replace("user", approverUserName);
        _confirmButton.OpenDialog(wrongPasswordDialog);
    }
}
