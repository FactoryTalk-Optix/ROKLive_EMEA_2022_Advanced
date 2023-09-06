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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;
#endregion

public class MqttLogic : BaseNetLogic
{
    public override void Start()
    {
        brokerIpAddress = LogicObject.GetVariable("BrokerIpAddress");
        checkMqttClientConnection = new PeriodicTask(MqttClientCheckConnection, 1000, LogicObject);
        checkMqttClientConnection.Start();
    }

    public override void Stop()
    {
        Disconnect();
        checkMqttClientConnection.Cancel();
        checkMqttClientConnection.Dispose();
    }

    [ExportMethod]
    public void Connect()
    {
        // Create a client connecting to the broker (default port is 1883)
        if (mqttClient == null)
            mqttClient = new MqttClient(brokerIpAddress.Value);
        try
        {
            if (mqttClient.IsConnected)
                return;
            // Connect to the broker
            mqttClient.Connect("OptixPublishClient");
            // Assign a callback to be executed when a message is published to the broker
            mqttClient.MqttMsgPublishReceived += PublishClient_MqttMsgPublished;
            // Subscribe to the "my_topic" topic with QoS 2
            mqttClient.Subscribe(new string[] { "/my_topic" }, // topic
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); // QoS level
            LogicObject.GetVariable("ReceivedMessage").Value = "Connected To Broker";
        }
        catch (Exception ex)
        {
            Log.Error("MQTT.Connect", "Can't connect to MQTT Broker, error: " + ex.Message);
            LogicObject.GetVariable("ReceivedMessage").Value = ex.Message;
            return;
        }
    }

    [ExportMethod]
    public void Disconnect()
    {
        try
        {
            if (!mqttClient.IsConnected)
                return;
            mqttClient.Unsubscribe(new string[] { "/my_topic" });
            Thread.Sleep(5000);
            mqttClient.Disconnect();
            LogicObject.GetVariable("ReceivedMessage").Value = "Disconnected From the Broker";
        }
        catch (NullReferenceException)
        {
            //Log.Warning("MQTT.Disconnect", "Broker is already disconnected");
            LogicObject.GetVariable("ReceivedMessage").Value = "Broker is already disconnected";
        }
        catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttCommunicationException)
        {
            //Log.Warning("MQTT.Disconnect", "Broker is not connected");
            LogicObject.GetVariable("ReceivedMessage").Value = "Broker is not connected";
        }
        catch (Exception ex)
        {
            Log.Error("MQTT.Disconnect", "Error: " + ex.ToString());
        }
    }

    [ExportMethod]
    public void PublishMessage(string message)
    {
        try
        {
            if (!mqttClient.IsConnected)
            {
                LogicObject.GetVariable("ReceivedMessage").Value = "Connect to Broker before publish data";
                return;
            }
            mqttClient.Publish("/my_topic", // topic
                System.Text.Encoding.UTF8.GetBytes(message), // message body
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                false); // retained
        }
        catch (Exception ex)
        {
            LogicObject.GetVariable("ReceivedMessage").Value = "Data send failed (Connect to Broker before publish data)";
            Log.Info("MQTT.Send", "Error sending package: " + ex.Message);
        }
    }

    private void PublishClient_MqttMsgPublished(object sender, MqttMsgPublishEventArgs e)
    {
        LogicObject.GetVariable("ReceivedMessage").Value = System.Text.Encoding.UTF8.GetString(e.Message);
    }

    private void MqttClientCheckConnection()
    {
        if (mqttClient != null)
            LogicObject.GetVariable("Connected").Value = mqttClient.IsConnected;
        else
            LogicObject.GetVariable("Connected").Value = false;
    }

    private MqttClient mqttClient;
    private IUAVariable brokerIpAddress;
    private PeriodicTask checkMqttClientConnection;
}
