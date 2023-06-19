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
    public partial class TrendForm : Form
    {
        double x = 0;
        int counter = 0;
        int offset = 15;
        int gap = 20;
        int order;
        int selectedSignalTrendViewOrder;

        List<ListViewItem> inputCheckedItemList;
        List<ListViewItem> outputCheckedItemList;
        Panel trendViewPanel;
        Chart trendChart;
        ChartArea chartArea;
        Series series;

        
        ListViewItem selectedTrendSignalItem;
        public SignalPanelCell[] inputSignalCell;
        public SignalPanelCell[] outputSignalCell;
      //  private TrendViewCell[] inputSignalTrendViewCell;
      //  private TrendViewCell[] outputSignalTrendViewCell;

        int maxNumberOfSelectedSignalTrendView = 10;
        int numberOfSelectedSignalTrendView = 0;
        int maxNumOfTrendViewCells = 10;

        public TrendForm()
        {
            InitializeComponent();
            TrendTimer.Enabled = true;    
            StartPosition = FormStartPosition.CenterScreen;
           
        }

        public TrendForm(int order)
        {
            InitializeComponent();
            selectedSignalTrendViewOrder = order;
            TrendTimer.Enabled = true;
            StartPosition = FormStartPosition.CenterScreen;

           
            
        }       

        private void TrendForm_Load(object sender, EventArgs e)
        {
            TrendTimer.Tick += TrendTimer_Tick;
            TrendTimer.Interval = 100;      
        }       
        

        private void TrendTimer_Tick(object sender, EventArgs e)
        {
           // drawTrendLine(CreateValues());
            x += 1;

            //trendViewCell.CreateBigTrendView();
            // Console.WriteLine("TrendTimer is working .... ....... {0} ", counter++);
            testFun(x);
        }

       
        public double[] CreateValues()
        {
            double[] value = new double[3];

            value[0] = 3 * Math.Sin(5 * x) + 5 * Math.Cos(3 * x);
            value[1] = 10 * Math.Sin(x);
            value[2] = 20;

            return value;
        }


        public void testFun(double a)
        {
            double b = a;

         //   Console.WriteLine("testFun is working .... ....... {0} ", counter++);

        }

        public void drawTrendLine(double[] data)
        {
            //int maxNumPoints = 100;

            //for (int i = 0; i < data.Length; i++)
            //{
            //     chart1.Series[i].Points.AddXY(x, data[i]);
            //    if (chart1.Series[i].Points.Count > maxNumPoints)
            //        chart1.Series[i].Points.RemoveAt(0);

            //    chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[i].Points[0].XValue;
            //    chart1.ChartAreas[0].AxisX.Maximum = maxNumPoints + chart1.Series[i].Points[0].XValue;  //x
            //    chart1.Series[i].Color = DefinedColors.GetColor(i);
            //    chart1.Series[i].BorderWidth = 2;
            //    chart1.Series[i].BorderDashStyle = ChartDashStyle.Solid;
            //    chart1.ChartAreas[0].AxisX.Title = "Time";
            //    chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            //    chart1.ChartAreas[0].AxisX.Interval = 1;
            //    chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            //    chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            //    chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            //    chart1.ChartAreas[0].AxisX.MajorTickMark.Interval = 10;
            //    chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

            //    chart1.ChartAreas[0].AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray;
            //    chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            //    chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
            //    chart1.ChartAreas[0].AxisX.IsLabelAutoFit = true;

            //    chart1.ChartAreas[0].AxisY.Title = "Signal Value";
            //    chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            //    chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            //    chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            //    chart1.ChartAreas[0].AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray;
            //    chart1.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;

            //    chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
            //    chart1.ChartAreas[0].AxisY.MinorGrid.Interval = 1;
            //    chart1.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            //    //chart1.ChartAreas[0].AxisY.MinorTickMark.Interval = 0.1;  
            //}

            //trendForm.Controls.Add(chart1);
        }

    }
}
