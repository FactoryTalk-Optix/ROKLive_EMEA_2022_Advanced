#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using UAManagedCore;
using System.Text.Json;
using System.Threading;
#endregion

public class eChartRadarLogic : BaseNetLogic
{
    private PeriodicTask updateTask;

    public override void Start()
    {
        // Initial setup - create both files
        InitEchart();
    }

    public override void Stop()
    {
        // Dispose the periodic task when stopping
        if (updateTask != null)
        {
            updateTask.Dispose();
            updateTask = null;
        }
    }

    [ExportMethod]
    public void RefreshRadarGraph()
    {
        try
        {
            // Path for the data.json file
            String jsonPath = ResourceUri.FromProjectRelativePath("eCharts/data.json").Uri;

            // Create a simplified version of the data just for updates
            var updateData = new eCharts.UpdateData
            {
                Series = new List<eCharts.SeriesOption>
                {
                    new eCharts.SeriesOption
                    {
                        Data = new List<eCharts.DataOption>
                        {
                            new eCharts.DataOption
                            {
                                Value = new List<double>()
                            }
                        }
                    }
                }
            };

            // Get the latest values
            for (int i = 1; i < 7; i++)
            {
                updateData.Series[0].Data[0].Value.Add(Project.Current.GetVariable("Model/Dashboard/eCharts/eChart" + i).Value * 1000);
            }

            // Serialize to JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };
            string jsonContent = JsonSerializer.Serialize(updateData, options);

            // Write to the data.json file
            File.WriteAllText(jsonPath, jsonContent);

            Log.Debug("eCharts", "Data JSON updated");
        }
        catch (Exception ex)
        {
            Log.Error("eCharts", $"Error updating data JSON: {ex.Message}");
        }
    }

    private void InitEchart()
    {
        Log.Debug("eCharts", "Starting full refresh");

        // 1. Generate the main JavaScript file
        String templatePath = ResourceUri.FromProjectRelativePath("eCharts/Template-data.js").Uri;
        var fileContent = File.ReadAllText(templatePath);

        var classContent = new eCharts.RadarChartOption();

        // Insert values
        for (int i = 1; i < 7; i++)
        {
            classContent.Series[0].Data[0].Value[i - 1] = Project.Current.GetVariable("Model/Dashboard/eCharts/eChart" + i).Value * 1000;
        }

        fileContent = fileContent.Replace("$DATA$", JsonSerializer.Serialize(classContent));

        // Write to file
        File.WriteAllText(templatePath.Replace("Template-", ""), fileContent);

        // 2. Also create a data.json file for the auto-update mechanism
        RefreshRadarGraph();

        Log.Debug("eCharts", "Full refresh finished");
    }
}

namespace eCharts
{
    // Add a simplified class for updates
    public class UpdateData
    {
        [JsonPropertyName("series")]
        public List<SeriesOption> Series { get; set; }
    }

    public class RadarChartOption
    {
        [JsonPropertyName("radar")]
        public RadarOption Radar { get; set; } = new RadarOption
        {
            Shape = "circle",
            Silent = true,
            Indicator = new List<IndicatorOption>
            {
                new IndicatorOption { Name = "Main Thrusters", Max = 20000, Color = "#ffffff" },
                new IndicatorOption { Name = "Lateral Thrusters", Max = 20000, Color = "#ffffff" },
                new IndicatorOption { Name = "Oxygen", Max = 20000, Color = "#ffffff" },
                new IndicatorOption { Name = "Water Treatment", Max = 20000, Color = "#ffffff" },
                new IndicatorOption { Name = "Drilling", Max = 20000, Color = "#ffffff" },
                new IndicatorOption { Name = "Stabilization", Max = 20000, Color = "#ffffff" }
            },
            AxisName = new AxisNameOption
            {
                FontSize = 15,
                FontFamily = "Share Tech Mono"
            },
            AxisLine = new AxisLineOption
            {
                LineStyle = new LineStyleOption
                {
                    Color = "#375454",
                    Width = 2,
                    Cap = "round"
                }
            }
        };

        [JsonPropertyName("series")]
        public List<SeriesOption> Series { get; set; } = new List<SeriesOption>
        {
            new SeriesOption
            {
                Name = "Energy consumption values",
                Type = "radar",
                Data = new List<DataOption>
                {
                    new DataOption
                    {
                        Value = new List<double> { 0, 0, 0, 0, 0, 0 }, // Default values will be replaced at runtime
                        Name = "Line 1"
                    }
                },
                ItemStyle = new ItemStyleOption
                {
                    Color = "#55ffdf"
                }
            }
        };
    }

    public class RadarOption
    {
        [JsonPropertyName("shape")]
        public string Shape { get; set; }

        [JsonPropertyName("silent")]
        public bool Silent { get; set; }

        [JsonPropertyName("indicator")]
        public List<IndicatorOption> Indicator { get; set; }

        [JsonPropertyName("axisName")]
        public AxisNameOption AxisName { get; set; }

        [JsonPropertyName("axisLine")]
        public AxisLineOption AxisLine { get; set; }
    }

    public class IndicatorOption
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("max")]
        public int Max { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }
    }

    public class AxisNameOption
    {
        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; }

        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; }
    }

    public class AxisLineOption
    {
        [JsonPropertyName("lineStyle")]
        public LineStyleOption LineStyle { get; set; }
    }

    public class LineStyleOption
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("cap")]
        public string Cap { get; set; }
    }

    public class SeriesOption
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("data")]
        public List<DataOption> Data { get; set; }

        [JsonPropertyName("itemStyle")]
        public ItemStyleOption ItemStyle { get; set; }
    }

    public class DataOption
    {
        [JsonPropertyName("value")]
        public List<double> Value { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ItemStyleOption
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }
    }
}