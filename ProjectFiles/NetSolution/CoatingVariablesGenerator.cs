#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.CoreBase;
using FTOptix.Alarm;
using FTOptix.EventLogger;
using FTOptix.DataLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.OPCUAServer;
using FTOptix.OPCUAClient;
using FTOptix.Retentivity;
using FTOptix.AuditSigning;
using FTOptix.Core;
using System.Threading;
#endregion

public class CoatingVariablesGenerator : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        myPeriodicTask = new PeriodicTask(IncrementVariable, 2000, LogicObject);
        myPeriodicTask.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        myPeriodicTask.Dispose();
    }

    private void IncrementVariable() {
        Random r = new Random();
        Project.Current.GetVariable("Model/Coating/WinderSpeed").Value = (int)r.Next(0, 100);
        Thread.Sleep(1);
        Project.Current.GetVariable("Model/Coating/UnwinderSpeed").Value = (int)r.Next(0, 100);
        Thread.Sleep(1);
        Project.Current.GetVariable("Model/Coating/TemperatureZone1").Value = (int)r.Next(0, 100);
        Thread.Sleep(1);
        Project.Current.GetVariable("Model/Coating/TemperatureZone2").Value = (int)r.Next(0, 100);
        Thread.Sleep(1);
        Project.Current.GetVariable("Model/Coating/TemperatureZone3").Value = (int)r.Next(0, 100);
        Thread.Sleep(1);
        Project.Current.GetVariable("Model/Coating/TemperatureZone4").Value = (int)r.Next(0, 100);
        Thread.Sleep(1);
    }

    private PeriodicTask myPeriodicTask;
}
