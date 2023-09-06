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
using System.Linq;
#endregion

public class PrepareEmptyVersion : BaseNetLogic
{
    [ExportMethod]
    public void DeleteDashboardStuff() {
        Log.Info("Deleting Dashboard stuff");
        // Delete WebBrowser from UI Page
        Project.Current.Get<WebBrowser>("UI/Pages/MainPanelLoader/Dashboard/WebBrowser/WebBrowser").Delete();
    }

    [ExportMethod]
    public void DeleteDrillingStuff() {
        Log.Info("Deleting Drilling stuff");
        // Delete MoreInfo button argument
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Drilling/AlarmGrid/MoreInfo").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        // Delete MoreInfo visibility DynamicLink
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Drilling/AlarmGrid/MoreInfo").Get("Visible").Children) {
            item.Delete();
        }
    }

    [ExportMethod]
    public void DeleteSlurryStuff() {
        Log.Info("Deleting Slurry stuff");
        // Delete methods from Insert button
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Slurry/SparePartsStorage/InsertNewItem/Insert").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        // Delete methods from Update button
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Slurry/SparePartsStorage/UpdateSelectedItem/Update").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        // Delete methods from Delete button
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Slurry/SparePartsStorage/Delete").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        // Delete methods from Refresh button
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Slurry/SparePartsStorage/Refresh").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
    }

    [ExportMethod]
    public void DeleteCoatingStuff() {
        Log.Info("Deleting Caoating stuff");
        // Delete Generate PDF button
        foreach (var item in Project.Current.Get<Button>("UI/Pages/MainPanelLoader/Coating/Objects/Generate").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        // Delete Stylesheet Checkboxes options
        foreach (var item in Project.Current.Get<CheckBox>("UI/Pages/MainPanelLoader/Coating/Objects/CheckBox1").Children) {
            if (item.BrowseName.Contains("ModifiedValue")) {
                item.Delete();
            }
        }
        foreach (var item in Project.Current.Get<CheckBox>("UI/Pages/MainPanelLoader/Coating/Objects/CheckBox2").Children) {
            if (item.BrowseName.Contains("ModifiedValue")) {
                item.Delete();
            }
        }
    }

    [ExportMethod]
    public void DeleteDryingStuff() {
        Log.Info("Deleting Drying stuff");
        // Delete stuff for ExpressionEvaluator
        foreach (var item in Project.Current.Get<Label>("UI/Pages/MainPanelLoader/Drying/ExpressionEvaluatorArea/OutputValue").Get("Text").Children) {
            item.Delete();
        }
        // Restore hashtags on Label
        Project.Current.Get<Label>("UI/Pages/MainPanelLoader/Drying/ExpressionEvaluatorArea/OutputValue").Text = "#####";
        // Delete all converters
        foreach (var item in Project.Current.Get<TextBox>("UI/Pages/MainPanelLoader/Drying/StringFormatterArea/OutputDateTime").Get("Text").Children) {
            item.Delete();
        }
        foreach (var item in Project.Current.Get<Label>("UI/Pages/MainPanelLoader/Drying/ConditionalConverterArea/OutputValue").Get("Text").Children) {
            item.Delete();
        }
        foreach (var item in Project.Current.Get<Image>("UI/Pages/MainPanelLoader/Drying/KeyValueConverterArea/OutputImage").Get("Path").Children) {
            item.Delete();
        }
    }

    [ExportMethod]
    public void DeleteCalendaringStuff() {
        Log.Info("Deleting Calendaring stuff");
        // Delete roller instances
        Project.Current.Get("UI/Pages/MainPanelLoader/Calendaring/AuditActions/Roller1").Delete();
        Project.Current.Get("UI/Pages/MainPanelLoader/Calendaring/AuditActions/Roller2").Delete();
        Project.Current.Get("UI/Pages/MainPanelLoader/Calendaring/AuditActions/Roller3").Delete();
        // Delete users DataGrid
        Project.Current.Get("UI/Pages/MainPanelLoader/Calendaring/AuditUsers/UsersAuditDataGrid").Delete();
        // Delete audit DataGrid
        Project.Current.Get("UI/Pages/MainPanelLoader/Calendaring/FileHash/HashOutput").Delete();
        // Delete emulator DataGrid
        Project.Current.Get("Loggers/EmulatorLogDataGrid").Delete();
        // Delete table from DB settings
        foreach (var item in Project.Current.Get<SQLiteStore>("DataStores/EmbeddedDatabase").Tables) {
            if (item.BrowseName == "EmulatorLogDataGrid") {
                item.Delete();
            }
        }
        // Delete variables in Model folder
        //foreach (var item in Project.Current.Get<Folder>("Model/Calendaring/AuditWorkflow").Children) {
        //    item.Delete();
        //}
    }

    [ExportMethod]
    public void DeleteSlittingStuff() {
        Log.Info("Deleting Slitting stuff");
        // Delete OPC custom behavior stuff
        foreach (var item in Project.Current.Get<Folder>("Model/Slitting/OPC_Items").Children) {
            item.Delete();
        }
        // Delete Roller alias from type and instance
        Project.Current.Get("UI/Templates/MotorWithBehavior/RollerAlias").Delete();
        Project.Current.Get("UI/Pages/MainPanelLoader/Slitting/OPCUACallMethods/MotorWithBehavior1/RollerAlias").Delete();
        // Delete arguments in widget type
        foreach (var item in Project.Current.Get<Button>("UI/Templates/MotorWithBehavior/Start").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        foreach (var item in Project.Current.Get<Button>("UI/Templates/MotorWithBehavior/Stop").Children) {
            if (item.BrowseName.Contains("MouseClickEventHandler")) {
                item.Delete();
            }
        }
        foreach (var item in Project.Current.Get<SpinBox>("UI/Templates/MotorWithBehavior/SetSpeedValue").Get("Value").Children) {
            item.Delete();
        }
        foreach (var item in Project.Current.Get<SpinBox>("UI/Templates/MotorWithBehavior/CurrentSpeedValue").Get("Value").Children) {
            item.Delete();
        }
        foreach (var item in Project.Current.Get<ScaleLayout>("UI/Templates/MotorWithBehavior/Motor").Get("FillColor").Children) {
            item.Delete();
        }
        foreach (var item in Project.Current.Get<ComboBox>("UI/Templates/MotorWithBehavior/RollerList").Get("Model").Children) {
            item.Delete();
        }
        foreach (var item in Project.Current.Get<ComboBox>("UI/Templates/MotorWithBehavior/RollerList").Get("DisplayValuePath").Children) {
            item.Delete();
        }

        // Creating backup of OPC/UA client settings
        var myBkClient = InformationModel.Make<OPCUAClient>("OPCUAClient");
        // Copy OPC/UA parameters
        var originalClient = Project.Current.Get<OPCUAClient>("OPC-UA/OPCUAClient");
        foreach (var childVariable in originalClient.Children.OfType<IUAVariable>())
            myBkClient.GetOrCreateVariable(childVariable.BrowseName).SetValueNoPermissions(childVariable.Value.Value);
        // Deleting OPC/UA client (and all its children)
        Project.Current.Get<OPCUAClient>("OPC-UA/OPCUAClient").Delete();
        // Set OPC-UA endpoint to Localhost
        myBkClient.ServerEndpointURL = "opc.tcp://127.0.0.1:48020";
        // Creating new empty OPC/UA client with backed-up settings
        Project.Current.Get("OPC-UA").Add(myBkClient);
        // Delete MQTT NetLogic
        Project.Current.Get("UI/Pages/MainPanelLoader/Slitting/MQTT/MqttLogic").Delete();
    }

    [ExportMethod]
    public void DeleteAll() {
        // Delete everything and create startup project
        DeleteCalendaringStuff();
        DeleteCoatingStuff();
        DeleteDashboardStuff();
        DeleteDrillingStuff();
        DeleteDryingStuff();
        DeleteSlurryStuff();
        DeleteSlittingStuff();
        Log.Info("Completed!");
    }
}
