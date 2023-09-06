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
using System.Threading;
#endregion

public class AlarmsHistoryLogic : BaseNetLogic {
    // Initialization of DataStore items
    Store myStore;
    Table myTable;
    // Initialization of asynchronous tasks
    private LongRunningTask lrtSelectQuery;
    IUAObject myLastDaysObject;
    // Random variable
    Random r = new Random();
    // Automatic refresh
    private PeriodicTask ptRefresh;
    // Maximum number of alarms per day
    Int32 maxAlarmCount;
    // Maximum random number to generate
    Int32 maxRandomCount;

    public override void Start() {
        // Execute only if logic is enabled
        if ((bool)LogicObject.GetVariable("enableLogic").Value) {
            // Create switches and label
            CreateSwitches();
            // Getting references from local objects
            myStore = Project.Current.Get<Store>("DataStores/EmbeddedDatabase");
            myTable = myStore.Tables.Get<Table>("AlarmsEventLogger");
            // Getting reference of current custom object
            myLastDaysObject = Owner.GetObject("Last30Days");
            // Get maximum number of alarms per day
            maxAlarmCount = LogicObject.GetVariable("maxVal").Value;
            if (maxAlarmCount == 0) {
                maxAlarmCount = Int32.MaxValue;
            }
            // Get maximum number for Random function
            maxRandomCount = LogicObject.GetVariable("maxRandomCount").Value;
            // Delete children objects (if any)
            foreach (var childrenVar in myLastDaysObject.Children) {
                childrenVar.Delete();
            }
            // Populate with new children variables
            for (int i = 0; i < 30; i++) {
                var dailyVar = InformationModel.MakeVariable(DateCalculator(DateTime.Today, (i * -1)).ToString("yyyyMMdd"), OpcUa.DataTypes.Int32);
                dailyVar.Value = r.Next(0, maxRandomCount);
                Thread.Sleep(1);
                myLastDaysObject.Add(dailyVar);
            }
            // Populating current alarms count
            lrtSelectQuery = new LongRunningTask(SelectQueryMethod, LogicObject);
            try {
                lrtSelectQuery.Start();
            } catch {
                Log.Error("Drilling.AlarmsHistogram", "Error executing LongRunningTask");
            }
            // Starting automatic update of alarms
            ptRefresh = new PeriodicTask(RefreshGraph, 2000, LogicObject);
            ptRefresh.Start();
        }
    }

    [ExportMethod]
    public void RefreshGraph() {
        try {
            lrtSelectQuery.Start();
        } catch {
            Log.Verbose1("Drilling.AlarmsHistogram", "Task is already running, waiting for completition");
        }
    }

    public override void Stop() {
        // Insert code to be executed when the user-defined logic is stopped
        if (lrtSelectQuery != null) {
            lrtSelectQuery.Dispose();
        }
        if (ptRefresh != null) {
            ptRefresh.Dispose();
        }
    }

    private void SelectQueryMethod(LongRunningTask task) {
        // Initialize DB query
        Object[,] ResultSet;
        String[] Header;
        myStore.Query("SELECT LocalTime, SourceName FROM " + myTable.BrowseName.ToString() + " ORDER BY LocalTime DESC LIMIT 30", out Header, out ResultSet);
        // Populate objects with values
        if (ResultSet.GetLength(0) > 0) {
            // Zero the existing days from database
            for (int i = 0; i < ResultSet.GetLength(0); i++) {
                try {
                    var myRowDate = (DateTime)ResultSet[i, 0];
                    var myObjVar = myLastDaysObject.GetVariable(myRowDate.ToString("yyyyMMdd"));
                    myObjVar.Value = 0;
                } catch {
                    Log.Verbose1("Drilling.AlarmHistogram", "Alarms on " + ((DateTime)ResultSet[i, 0]).ToString() + " are out of range");
                }
            }
            // Populate with real data 
            for (int i = 0; i < ResultSet.GetLength(0); i++) {
                try {
                    var myRowDate = (DateTime)ResultSet[i, 0];
                    var myObjVar = myLastDaysObject.GetVariable(myRowDate.ToString("yyyyMMdd"));
                    if (myObjVar.Value < maxAlarmCount) {
                        myObjVar.Value = myObjVar.Value + 1;
                    }
                } catch {
                    Log.Verbose1("Drilling.AlarmHistogram", "Alarms on " + ((DateTime)ResultSet[i, 0]).ToString() + " are out of range");
                }
            }
        }
        Log.Verbose1("Drilling.AlarmHistogram", "Task completed");
    }
    private DateTime DateCalculator(DateTime startDate, int daysOffset) {
        return startDate.AddDays(daysOffset);
    }
    private void CreateSwitches() {
        // Get the page to work with
        var myPage = Owner.Owner.Get<Panel>("AlarmGrid");

        // Create Switch1
        var myNewSwitch1 = InformationModel.Make<Switch>("Switch1");
        // Set switch position
        myNewSwitch1.LeftMargin = 800;
        myNewSwitch1.TopMargin = 10;
        myNewSwitch1.Width = 60;
        // Set DynamicLink to Switch value
        myNewSwitch1.GetVariable("Checked").SetDynamicLink(Project.Current.GetVariable("Model/Drilling/CU79B"), DynamicLinkMode.ReadWrite);
        // Add switch to the page
        myPage.Add(myNewSwitch1);

        // Create Label1
        var myLabel1 = InformationModel.Make<Label>("Label1");
        // Set label position
        myLabel1.LeftMargin = myNewSwitch1.LeftMargin - 120;
        myLabel1.TopMargin = myNewSwitch1.TopMargin + 5;
        myLabel1.Width = -1;
        // Set label text
        myLabel1.Text = "CU79B Sensor";
        // Add label to the page
        myPage.Add(myLabel1);

        // Create Switch2
        var myNewSwitch2 = InformationModel.Make<Switch>("Switch2");
        // Set switch position
        myNewSwitch2.LeftMargin = myNewSwitch1.LeftMargin + 250;
        myNewSwitch2.TopMargin = 10;
        myNewSwitch2.Width = 60;
        // Set DynamicLink to Switch value
        myNewSwitch2.GetVariable("Checked").SetDynamicLink(Project.Current.GetVariable("Model/Drilling/CU88C"), DynamicLinkMode.ReadWrite);
        // Add switch to the page
        myPage.Add(myNewSwitch2);

        // Create Label2
        var myLabel2 = InformationModel.Make<Label>("Label2");
        // Set label position
        myLabel2.LeftMargin = myNewSwitch2.LeftMargin - 120;
        myLabel2.TopMargin = myNewSwitch2.TopMargin + 5;
        myLabel2.Width = -1;
        // Set label text
        myLabel2.Text = "CU88C Sensor";
        // Add label to the page
        myPage.Add(myLabel2);

    }
}
