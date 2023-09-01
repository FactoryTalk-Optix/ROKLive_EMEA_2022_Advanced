#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.CoreBase;
using FTOptix.Core;
using System.IO;
using FTOptix.OPCUAServer;
using System.Threading;
#endregion

public class eChartRadarLogic : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        RefreshRadarGraph();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void RefreshRadarGraph()
    {
        Owner.Get<WebBrowser>("WebBrowser").Visible = false;
        Log.Debug("eCharts", "Starting");
        String projectPath = (ResourceUri.FromProjectRelativePath("").Uri);
        String folderSeparator = Path.DirectorySeparatorChar.ToString();

        // Get template name and create destination path
        string templatePath = projectPath + folderSeparator + "eCharts" + folderSeparator + "Template-data.js";
        string filePath = projectPath + folderSeparator + "eCharts" + folderSeparator + "data.js";

        // Read template page content
        string text = File.ReadAllText(templatePath);

        // Insert values
        for (int i = 1; i < 7; i++)
        {
            text = text.Replace(i < 10 ? "$0" + i: "$" +i, (Project.Current.GetVariable("Model/Dashboard/eCharts/eChart" + i).Value * 1000).ToString());
        }
        
        // Write to file
        File.WriteAllText(filePath, text);

        // Refresh WebBrowser page
        Owner.Get<WebBrowser>("WebBrowser").Refresh();
        Log.Debug("eCharts", "Finished");
        Thread.Sleep(500);
        Owner.Get<WebBrowser>("WebBrowser").Visible = true;
    }
}
