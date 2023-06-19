using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AutobolterSim_cs
{
    public partial class OneTrendForm : Form
    {
        List<ListViewItem> inputCheckedItemList;
        List<ListViewItem> outputCheckedItemList;

        double x = 0;
        int minRandomNum = 10;
        int maxRandomNum = 60000;
        private static readonly Random _random = new Random();
        int totalNumOfLines;

        public OneTrendForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            timer1.Start();
            timer1.Enabled = true;
        }

        public OneTrendForm(int order, List<ListViewItem> inputCheckedItemList, List<ListViewItem> outputCheckedItemList)
        {
            InitializeComponent();
            this.inputCheckedItemList = inputCheckedItemList;
            this.outputCheckedItemList = outputCheckedItemList;
            StartPosition = FormStartPosition.CenterScreen;
            timer1.Enabled = true;
            timer1.Start();
        }
       

        private void OneTrendForm_Load(object sender, EventArgs e)
        {
            //Chart mychart = new Chart();
            timer1.Tick += timer1_Tick;
            timer1.Interval = 120;

            //chart1.Text = "Autobolter Simulator";
            //string[] seriesName = { "Feed Pressure", "Feed Speed", "Rotation Speed" };
           
            //Console.WriteLine("OneTrendForm_Load .........., inputCheckedItemList = {0},  outputCheckedItemList = {1}", inputCheckedItemList.Count, outputCheckedItemList.Count);

            //chart1.Width = 1500;
            //chart1.Height = 750;


            //for (int i = 0; i < 3; i++)
            //{
            //    chart1.Series.Add(seriesName[i].ToString());
            //    chart1.Series[i].ChartType = SeriesChartType.Line;
            //}
            //chart1.Legends[0].Enabled = true;
            //chart1.ChartAreas[0].AxisY.Minimum = -30;
            //chart1.ChartAreas[0].AxisY.Maximum = 60;
        }


        static public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public double[] CreateValues()
        {
            double[] value = new double[3];

            value[0] = 3 * Math.Sin(5 * x) + 5 * Math.Cos(3 * x);
            value[1] = 10 * Math.Sin(x);
            value[2] = 20;

            return value;
        }

        double getValue(bool sourceType, ListViewItem checkedItemList)
        {
            double value = 0;
            List<SignalType.Signals> signalFromGateway;            

            if (!sourceType)
            {
                signalFromGateway = Gateway.ReadInputSignals();
            }
            else
            {
                signalFromGateway = Gateway.ReadOutputSignals();
            }

            foreach (SignalType.Signals sg in signalFromGateway)
            {
                if (sg.Name == checkedItemList.SubItems[(int)SignalType.ItemIndex.Name].Text)
                {
                    value = sg.Value;
                }
            }

            return value;
        }


        //public void drawTrendLine(List<ListViewItem> inputCheckedItemList, List<ListViewItem> outputCheckedItemList)
        //{
        //    int maxNumPoints = 100;
        //    int lineNumber = 0;

        //    chart1.Legends[0].Enabled = true;
        //    chart1.ChartAreas[0].AxisY.Minimum = 0;
        //    chart1.ChartAreas[0].AxisY.Maximum = 2000;

        //    Console.WriteLine("inputCheckedItemList = {0}; outputCheckedItemList = {1}", inputCheckedItemList.Count, outputCheckedItemList.Count);

        //    if (inputCheckedItemList.ToList().Count > 0)
        //    {
        //        for (int i = 0; i < inputCheckedItemList.ToList().Count; i++)
        //        {
        //            chart1.Series[lineNumber].Points.AddXY(x, getValue(false, inputCheckedItemList[i]));
        //            if (chart1.Series[lineNumber].Points.Count > maxNumPoints)
        //                chart1.Series[lineNumber].Points.RemoveAt(0);


        //            chart1.Series.Add(inputCheckedItemList[i].SubItems[(int)SignalType.ItemIndex.Name].Text);
        //            chart1.Series[lineNumber].ChartType = SeriesChartType.Line;
        //            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[lineNumber].Points[0].XValue;
        //            chart1.ChartAreas[0].AxisX.Maximum = maxNumPoints + chart1.Series[lineNumber].Points[0].XValue;  //x
        //            chart1.Series[lineNumber].Color = DefinedColors.GetColor(lineNumber);
        //            chart1.Series[lineNumber].BorderWidth = 2;
        //            chart1.Series[lineNumber].BorderDashStyle = ChartDashStyle.Solid;
        //            chart1.ChartAreas[0].AxisX.Title = "Time";
        //            chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
        //            chart1.ChartAreas[0].AxisX.Interval = 1;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisX.MajorTickMark.Interval = 10;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

        //            chart1.ChartAreas[0].AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
        //            chart1.ChartAreas[0].AxisX.IsLabelAutoFit = true;

        //            chart1.ChartAreas[0].AxisY.Title = "Signal Value";
        //            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
        //            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;

        //            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisY.MinorGrid.Interval = 1;
        //            chart1.ChartAreas[0].AxisY.IsLabelAutoFit = true;
        //            //chart1.ChartAreas[0].AxisY.MinorTickMark.Interval = 0.1; 
        //            lineNumber++;
        //        }
        //    }

        //    if (outputCheckedItemList.ToList().Count > 0)
        //    {
        //        for (int i = 0; i < outputCheckedItemList.ToList().Count; i++)
        //        {
        //            chart1.Series[lineNumber].Points.AddXY(x, getValue(true, outputCheckedItemList[i]));
        //            if (chart1.Series[lineNumber].Points.Count > maxNumPoints)
        //                chart1.Series[lineNumber].Points.RemoveAt(0);

        //            chart1.Series.Add(outputCheckedItemList[i].SubItems[(int)SignalType.ItemIndex.Name].Text);
        //            chart1.Series[lineNumber].ChartType = SeriesChartType.Line;
        //            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[lineNumber].Points[0].XValue;
        //            chart1.ChartAreas[0].AxisX.Maximum = maxNumPoints + chart1.Series[lineNumber].Points[0].XValue;  //x
        //            chart1.Series[lineNumber].Color = DefinedColors.GetColor(lineNumber);
        //            chart1.Series[lineNumber].BorderWidth = 2;
        //            chart1.Series[lineNumber].BorderDashStyle = ChartDashStyle.Solid;
        //            chart1.ChartAreas[0].AxisX.Title = "Time";
        //            chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
        //            chart1.ChartAreas[0].AxisX.Interval = 1;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisX.MajorTickMark.Interval = 10;
        //            chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

        //            chart1.ChartAreas[0].AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
        //            chart1.ChartAreas[0].AxisX.IsLabelAutoFit = true;

        //            chart1.ChartAreas[0].AxisY.Title = "Signal Value";
        //            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
        //            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray;
        //            chart1.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;

        //            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
        //            chart1.ChartAreas[0].AxisY.MinorGrid.Interval = 1;
        //            chart1.ChartAreas[0].AxisY.IsLabelAutoFit = true;
        //            //chart1.ChartAreas[0].AxisY.MinorTickMark.Interval = 0.1; 
        //            lineNumber++;
        //        }
        //    }           
            
        //}


        //public void drawTrendLine(double[] data)
        //{
        //    int maxNumPoints = 100;

        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        chart1.Series[i].Points.AddXY(x, data[i]);
        //        if (chart1.Series[i].Points.Count > maxNumPoints)
        //            chart1.Series[i].Points.RemoveAt(0);

        //        chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[i].Points[0].XValue;
        //        chart1.ChartAreas[0].AxisX.Maximum = maxNumPoints + chart1.Series[i].Points[0].XValue;  //x
        //        chart1.Series[i].Color = DefinedColors.GetColor(i);
        //        chart1.Series[i].BorderWidth = 2;
        //        chart1.Series[i].BorderDashStyle = ChartDashStyle.Solid;
        //        chart1.ChartAreas[0].AxisX.Title = "Time";
        //        chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
        //        chart1.ChartAreas[0].AxisX.Interval = 1;
        //        chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
        //        chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
        //        chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
        //        chart1.ChartAreas[0].AxisX.MajorTickMark.Interval = 10;
        //        chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

        //        chart1.ChartAreas[0].AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray;
        //        chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
        //        chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
        //        chart1.ChartAreas[0].AxisX.IsLabelAutoFit = true;

        //        chart1.ChartAreas[0].AxisY.Title = "Signal Value";
        //        chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
        //        chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
        //        chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
        //        chart1.ChartAreas[0].AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray;
        //        chart1.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;

        //        chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
        //        chart1.ChartAreas[0].AxisY.MinorGrid.Interval = 1;
        //        chart1.ChartAreas[0].AxisY.IsLabelAutoFit = true;
        //        //chart1.ChartAreas[0].AxisY.MinorTickMark.Interval = 0.1;  
        //    }
        //}

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    drawTrendLine(CreateValues());
        //    x += 1;
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            //drawTrendLine(inputCheckedItemList, outputCheckedItemList);
            Console.WriteLine("timer1_Tick, drawTrendLine is called !");
            //drawTrendLine(CreateValues());
            x += 1;


            
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
            }
            else
            {
                timer1.Enabled = true;
            }

        }

       
    }
}
