#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
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
#endregion

public class ResetSlurryValues : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        string[,] myArray = (string[,])Project.Current.GetVariable("Model/Slurry/ColumnsValue").Value.Value;
        if (myArray[0,0]=="0" && myArray[0,1] == "0" && myArray[0,2] == "0") {
            for (int i = 0; i < myArray.Length; i++) {
                myArray[0,i] = "";
            }
            Project.Current.GetVariable("Model/Slurry/ColumnsValue").SetValue(myArray);
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}
