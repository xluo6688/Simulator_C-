using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;
using System.Drawing;
using System.Timers;


namespace AutobolterSim_cs
{
    public class TrendViewCell
    {
        public delegate void drawLineCallback();
        public delegate void drawMultiLineCallback();
        int x = 0;
        int offset = 15;
        int gap = 20;
        Panel trendViewPanel;     
        Chart trendChart;
        ChartArea chartArea;
        Series series;   
        Legend legend;   
        int order;
        SignalType.Signals signal;
  
        ListViewItem selectedTrendSignalItem;
 
        int colorNumber;
        int maxNumPoints = 300;
 
        private static Mutex mut_b1 = new Mutex();
        private static Mutex mut_b2 = new Mutex();
        private static System.Timers.Timer trendTimer = new System.Timers.Timer();
        ~TrendViewCell()
        {
            
        }

        public SignalType.Signals GetSignalOfTrendCell()
        {
            return signal;
        }

        public TrendViewCell(int order, ListViewItem listViewItem)
        {
            this.order = order;
            this.colorNumber = order - 1;
            selectedTrendSignalItem = listViewItem;
  
            trendViewPanel = new Panel();
            trendChart = new Chart();
            chartArea = new ChartArea();
            series = new Series();
            legend = new Legend();
            signal.Name = selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text;
            signal.Value = Double.Parse(selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.Value].Text);
            signal.Unit = selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.Unit].Text;
            signal.Min = Double.Parse(selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.Min].Text);
            signal.Max = Double.Parse(selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.Max].Text);
            signal.RawData = Double.Parse(selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.RawData].Text);
            signal.SourceOrDestination = selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text;
            //signal.Forced = checkBox.Checked;
            trendTimer.Interval = 100;
            trendTimer.Enabled = true;
            trendTimer.Elapsed += OnTimedEvent;         
            trendChart.Click += trendViewChart_click;

            series.ChartType = SeriesChartType.Line;
            series.Color = DefinedColors.GetColor(colorNumber);
            series.BorderWidth = 2;
            series.BorderDashStyle = ChartDashStyle.Solid;            
            chartArea.BackColor = System.Drawing.Color.Black;
            chartArea.AxisX.Title = "Time";
            chartArea.AxisX.TitleAlignment = StringAlignment.Center;
            chartArea.AxisX.Interval = 10;

            // chartArea.AlignmentStyle = AreaAlignmentStyles.All;
            //chartArea.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            //chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            //chartArea.AxisX.MajorGrid.Enabled = true;
            //chartArea.AxisX.MajorTickMark.Interval = 10;
            //chartArea.AxisX.MajorGrid.Interval = 10;

            //chartArea.AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray;
            //chartArea.AxisX.MinorGrid.Enabled = true;
            //chartArea.AxisX.MinorGrid.Interval = 1;
            //chartArea.AxisX.IsLabelAutoFit = true;

            chartArea.AxisY.Title = signal.Name.ToString();       
            //chartArea.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            //chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            //chartArea.AxisY.MajorGrid.Enabled = true;
            //chartArea.AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray;
            //chartArea.AxisY.TitleAlignment = StringAlignment.Near;
            chartArea.AxisY.TitleAlignment = StringAlignment.Center;
            
            //chartArea.AxisY.MinorGrid.Enabled = true;
            //chartArea.AxisY.MinorGrid.Interval = 1;
            chartArea.AxisY.IsLabelAutoFit = true;

            //chartArea.AlignWithChartArea = chartArea.Name;
            //chartArea.AlignmentStyle = AreaAlignmentStyles.Position;
      
            chartArea.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
           

            // chartArea.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            //chartArea.AlignWithChartArea="ChartAreas[0]";                 

            legend.Enabled = true;
            legend.Position.X = 50;
            legend.Position.Y = 50;
            trendChart.Legends.Add(legend);

            
        }

        private void trendViewChart_click(object sender, EventArgs e)
        {
            if (trendTimer.Enabled == true)
            {
                trendTimer.Enabled = false;
            }
            else
            {
                trendTimer.Enabled = true;
            }
        }



        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {  
            UpdateTrendSignalValue(); 
            //Console.WriteLine("The Timer is counting {0}", x);
            drawLine();
            x += 1;
        }

        public SignalType.Signals UpdateTrendSignalValue()
        {
            mut_b1.WaitOne();
            lock (Gateway.GetUpdatedInputFromGateway())
            {
                foreach (SignalType.Signals sg in Gateway.GetUpdatedInputFromGateway().ToList())
                {
                    if (sg.Name == signal.Name)
                    {
                        signal.Value = sg.Value;
                    }
                }
            }

            lock (Gateway.GetUpdatedOutputFromGateway())
            {
                foreach (SignalType.Signals sg in Gateway.GetUpdatedOutputFromGateway().ToList())
                {
                    if (sg.Name == signal.Name)
                    {
                        signal.Value = sg.Value;
                    }
                }
            }

            mut_b1.ReleaseMutex();

            return signal;
        }       


        private Label CreateTrendLabel(string labelText, string labelName, int pos_x, int pos_y, int width, int heigth)
        {
            Label label = new Label();
            int fontSize = 11;
            label.Name = labelName;
            label.Text = labelText;
            label.Font = new System.Drawing.Font("Serif", fontSize, System.Drawing.FontStyle.Bold);
            label.Width = width; label.Height = heigth;
            label.Location = new System.Drawing.Point(pos_x, pos_y - fontSize);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Margin = new Padding(0);

            return label;
        }

        public Panel CreateOneTrendViewPanel(ListViewItem selectedTrendSignalItem) 
        {
            trendViewPanel.Width = 1800; trendViewPanel.Height = 200;            
            trendViewPanel.Padding = new Padding(0);

            int orderLabelPos_x = 10; int orderLabelPos_y = 5 + offset; int orderLabel_width = 30; int orderLabel_heigth = 25;
            Label lb_busSignal = CreateTrendLabel(order.ToString(), "orderLabel" + order.ToString(), orderLabelPos_x, orderLabelPos_y, orderLabel_width, orderLabel_heigth);
            lb_busSignal.Padding = new Padding(0);
            trendViewPanel.Controls.Add(lb_busSignal);

            string trendChartName = "tc" + order.ToString(); int tc_x = orderLabelPos_x + orderLabel_width + gap; int tc_y = -10 + offset; int tc_width = 1500; int tc_height = 200;
            string signalName = selectedTrendSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text;

            trendChart = CreateTrendChart(trendChart, trendChartName, selectedTrendSignalItem, tc_x, tc_y, tc_width, tc_height);
            trendViewPanel.Controls.Add(trendChart);

            return trendViewPanel;

        }      

        private Chart CreateTrendChart(Chart trendChart, string chartName, ListViewItem listViewItem, int pos_x, int pos_y, int width, int height)
        {
            trendChart.Name = chartName;
            trendChart.Width = width;
            trendChart.Height = height;
            trendChart.Location = new System.Drawing.Point(pos_x, pos_y);         
            chartArea.Name = chartName;           
            //series.Name = listViewItem.SubItems[(int)SignalType.ItemIndex.Name].Text;
            series.Name = signal.Name;
            trendChart.ChartAreas.Add(chartArea);
            trendChart.Series.Add(series);           

            return trendChart;
        }       

        void drawLine()
        {
            try
            {
                if (trendChart.InvokeRequired)
                {
                    drawLineCallback w = new drawLineCallback(drawLine);
                    this.trendChart.Invoke(w, new object[] { });
                }
                else
                {
                    series.Points.AddXY(x, signal.Value);
                    if (series.Points.Count > maxNumPoints)
                        series.Points.RemoveAt(0);

                    chartArea.AxisX.Minimum = series.Points[0].XValue;
                    chartArea.AxisX.Maximum = maxNumPoints + series.Points[0].XValue;                 
                }

            }
            catch (Exception e)
            {

            }
        }
    }
}
