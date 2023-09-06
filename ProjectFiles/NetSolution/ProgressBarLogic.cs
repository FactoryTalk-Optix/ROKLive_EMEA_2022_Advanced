#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NativeUI;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.Core;
using FTOptix.OPCUAServer;
using FTOptix.OPCUAClient;
using FTOptix.EventLogger;
using FTOptix.AuditSigning;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Alarm;
using FTOptix.Report;
using FTOptix.DataLogger;
#endregion

public class ProgressBarLogic : BaseNetLogic
{
    int targetVar1;
    int targetVar2;
    int targetVar3;
    int refreshTime = 50;
    int increment = 2;

    Random r = new Random();

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        myPeriodicTask1 = new PeriodicTask(IncrementVariable1, refreshTime, LogicObject);
        myPeriodicTask1.Start();
        myPeriodicTask2 = new PeriodicTask(IncrementVariable2, refreshTime, LogicObject);
        myPeriodicTask2.Start();
        myPeriodicTask3 = new PeriodicTask(IncrementVariable3, refreshTime, LogicObject);
        myPeriodicTask3.Start();
    }

    private void IncrementVariable1() {
        IncrementVariable("Variable1", ref targetVar1);
    }

    private void IncrementVariable2() {
        IncrementVariable("Variable2", ref targetVar2);
    }

    private void IncrementVariable3() {
        IncrementVariable("Variable3", ref targetVar3);
    }

    private void IncrementVariable(string targetVariable, ref int targetVal) {
        int curVal = LogicObject.GetVariable(targetVariable).Value;
        if ((curVal == targetVal) || (curVal <= 1) || (curVal >= 159)) {
            targetVal = r.Next(1, 160);
        }
        if (curVal < (targetVal - increment)) {
            curVal = curVal + 2;
        } else if (curVal >= (targetVal + increment)) {
            curVal = curVal - 2;
        } else {
            ++curVal;
        }
        LogicObject.GetVariable(targetVariable).Value = curVal;
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        myPeriodicTask1.Dispose();
        myPeriodicTask2.Dispose();
        myPeriodicTask3.Dispose();
    }

    private PeriodicTask myPeriodicTask1;
    private PeriodicTask myPeriodicTask2;
    private PeriodicTask myPeriodicTask3;

}
