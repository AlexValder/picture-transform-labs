using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CSharpLab {
    public class GistoForm : Form {
        public GistoForm(string name, IDictionary<int, int> data) {
            this.InitializeComponent();
            Text = name;

            var series = new Series {
                Color = Color.Black,
            };
            foreach (var pair in data) {
                series.Points.AddXY(pair.Key, pair.Value);
            }

            series["PointWidth"] = "1";

            var min = series.Points.FindMinByValue();
            var max = series.Points.FindMaxByValue();

            this.gistogramChart.Series.Add(series);
            DisableGrid(min, max);
        }

        private void DisableGrid(DataPoint min, DataPoint max) {
            foreach (var chart in gistogramChart.ChartAreas) {
                chart.AxisX.MajorGrid.Enabled = false;
                chart.AxisX.MinorGrid.Enabled = false;
                chart.AxisY.MajorGrid.Enabled = false;
                chart.AxisY.MinorGrid.Enabled = false;
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.gistogramChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.gistogramChart)).BeginInit();
            this.SuspendLayout();
            //
            // gistogramChart
            //
            chartArea1.Name = "ChartArea1";
            this.gistogramChart.ChartAreas.Add(chartArea1);
            this.gistogramChart.Location = new System.Drawing.Point(-1, 0);
            this.gistogramChart.Name     = "gistogramChart";
            this.gistogramChart.Palette  = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.gistogramChart.Size     = new System.Drawing.Size(500, 500);
            this.gistogramChart.TabIndex = 0;
            this.gistogramChart.Text     = "gistogramChart";
            //
            // GistoForm
            //
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.gistogramChart);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name        = "GistoForm";
            ((System.ComponentModel.ISupportInitialize)(this.gistogramChart)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataVisualization.Charting.Chart gistogramChart;
    }
}
