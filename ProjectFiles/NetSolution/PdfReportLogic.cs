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
using System.IO;
#endregion

public class PdfReportLogic : BaseNetLogic
{
    private PeriodicTask myPeriodicTask;
    private LongRunningTask myLongRunningTask;
    Button myButton = null;
    IUANode myPathNode = null;
    string pdfPathStr = null;

    public override void Start()
    {
        // Try to assign a value to the button
        try {
            myButton = Owner.Get<Button>("Objects/Generate");
        } catch {
            // Button does not exist
            Log.Warning("PdfReportLogic", "Can't find Generate PDF button, maybe with a different name?");
            return;
        }
        // Execute search in asynchronous mode
        myLongRunningTask = new LongRunningTask(LrtRecursiveSearch, LogicObject);
        myLongRunningTask.Start();
    }

    private void LrtRecursiveSearch() {
        // Loop in button elements to find the MouseClickEvent
        foreach (var item1 in myButton.Children) {
            if (item1.BrowseName.Contains("MouseClickEventHandler")) {
                foreach (var item2 in item1.Children) {
                    RecursiveSearch(item2, "OutputPath");
                    if (myPathNode != null) {
                        break;
                    }
                }
            }
            if (myPathNode != null) {
                break;
            }
        }
        // Get value from found IUANode
        if (myPathNode==null) {
            Log.Warning("PdfReportLogic", "Can't find any OutputPath value");
            myLongRunningTask.Dispose();
            return;
        } else {
            // Get path from PDF
            pdfPathStr = new ResourceUri(((IUAVariable)myPathNode).Value).Uri;
            Log.Debug("PdfReportLogic", pdfPathStr);
            // Execute periodic check of PDF file
            myPeriodicTask = new PeriodicTask(CheckForPdf, 1000, LogicObject);
            myPeriodicTask.Start();
        }
    }

    private void RecursiveSearch(IUANode inputNode, string nodeName) {
        if (inputNode.BrowseName == nodeName) {
            // required name matches the search value
            myPathNode = inputNode;
            return;
        } else {
            if (inputNode.Children.Count > 0) {
                foreach (var item in inputNode.Children) {
                    // Loop again
                    RecursiveSearch(item, nodeName);
                }
            } else {
                return;
            }
            
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        if (myPeriodicTask!=null) {
            myPeriodicTask.Dispose();
        }
        if (myLongRunningTask!=null) {
            myLongRunningTask.Dispose();
        }
    }

    public void CheckForPdf() {
        if (File.Exists(pdfPathStr)) {
            Owner.Get<Label>("Objects/LbReport").TopMargin = 310;
            Owner.Get<Button>("Objects/Generate").TopMargin = 340;
            var myButton = (Button)Owner.Get("Objects/View");
            if (myButton==null) {
                // Create new button to view report
                var customType = Project.Current.Get("UI/Templates/ViewReportButton");
                var myNewButton = InformationModel.MakeObject<Button>("View", customType.NodeId );
                myNewButton.LeftMargin = Owner.Get<Button>("Objects/Generate").LeftMargin;
                myNewButton.Width = Owner.Get<Button>("Objects/Generate").Width;
                myNewButton.Height = Owner.Get<Button>("Objects/Generate").Height;
                myNewButton.TopMargin = 375;
                Owner.Get<Panel>("Objects").Add(myNewButton);
                // Set path to PDF report
                var myPdfViewer = Project.Current.Get<PdfViewer>("UI/Pages/DialogBox/ShowReport/PDFViewer");
                myPdfViewer.Path = pdfPathStr;
            }
        } else {
            // Delete button if PDF does not exist
            var myButton = (Button)Owner.Get("Objects/View");
            if (myButton != null) {
                myButton.Delete();
            }
            // Restore original positioning
            Owner.Get<Label>("Objects/LbReport").TopMargin = 350;
            Owner.Get<Button>("Objects/Generate").TopMargin = 377;
        }
    }
}
