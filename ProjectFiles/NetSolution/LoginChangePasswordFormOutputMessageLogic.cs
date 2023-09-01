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

public class LoginChangePasswordFormOutputMessageLogic : BaseNetLogic
{
    public override void Start()
    {
        messageVariable = Owner.GetVariable("Message");
        task = new DelayedTask(() =>
        {
            if (messageVariable == null)
            {
                Log.Error("LoginFormOutputMessageLogic", "Unable to find variable Message in LoginFormOutputMessage label");
                return;
            }

            messageVariable.Value = "";
            taskStarted = false;
        }, 10000, LogicObject);
    }

    [ExportMethod]
    public void SetOutputMessage(string message)
    {
        if (messageVariable == null)
        {
            Log.Error("ChangePasswordFormOutputMessageLogic", "Unable to find variable Message in ChangePasswordFormOutputMessage label");
            return;
        }

        messageVariable.Value = message;

        if (taskStarted)
        {
            task?.Cancel();
            taskStarted = false;
        }

        task.Start();
        taskStarted = true;
    }

    private DelayedTask task;
    private bool taskStarted = false;
    private IUAVariable messageVariable;
}
