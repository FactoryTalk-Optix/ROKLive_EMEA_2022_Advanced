#region Using directives
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.OPCUAServer;
using FTOptix.Report;
using FTOptix.DataLogger;
#endregion

public class LoginFormOutputMessageLogic : BaseNetLogic
{
    public override void Start()
    {
        messageVariable = Owner.GetVariable("Message");
        messageVariable.Value = "";

        task = new DelayedTask(() =>
        {
            if (messageVariable == null)
            {
                Log.Error("LoginFormOutputMessageLogic", "Unable to find Message variable in LoginFormOutputMessage label");
                return;
            }

            messageVariable.Value = "";
            taskStarted = false;
        }, 10000, LogicObject);
    }

    public override void Stop()
    {
        task?.Dispose();
    }

    [ExportMethod]
    public void SetOutputMessage(string message)
    {
        if (messageVariable == null)
        {
            Log.Error("LoginFormOutputMessageLogic", "Unable to find Message variable in LoginFormOutputMessage label");
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
