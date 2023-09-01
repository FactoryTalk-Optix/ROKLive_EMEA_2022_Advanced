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
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.Alarm;
#endregion

public class PrepareForDocker : BaseNetLogic
{
    [ExportMethod]
    public void DockerCleanup()
    {
        // Insert code to be executed by the method
        Project.Current.Get<NativeUIPresentationEngine>("UI/NativePresentationEngine").Delete();
        Project.Current.Get<WebUIPresentationEngine>("UI/WebPresentationEngine").Port = 80;
        Project.Current.Get("UI/Templates/RotatingCircle/Close").Delete();
    }
}
