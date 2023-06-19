using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;
using System.Drawing;
using System.Timers;


namespace AutobolterSim_cs
{
    class MultiSeries
    {
        public delegate void drawMultiLineCallback();

        double x = 0;

        
        Panel m_panel;
        Chart m_trendChart;
        ChartArea m_chartArea;
        Legend m_legend;
        SignalType.Signals[] m_inSignal;
        SignalType.Signals[] m_outSignal;
        Series[] m_inSeries;
        Series[] m_outSeries;

        int order;
        List<ListViewItem> inputListViewItem;
        List<ListViewItem> outputListViewItem;
        int maxNumPoints = 300; 
        private static Mutex mut_b1 = new Mutex();
        private static Mutex mut_b2 = new Mutex();
        System.Timers.Timer bigTrendTimer = new System.Timers.Timer();

        ~MultiSeries()
        {

        }

        public MultiSeries(int order, List<ListViewItem> i_inputListViewItem, List<ListViewItem> i_outputListViewItem)
        {
            this.inputListViewItem = i_inputListViewItem;
            this.outputListViewItem = i_outputListViewItem;
            int m_colorNumber = order - 1;

            m_inSeries = new Series[this.inputListViewItem.Count];
            m_outSeries = new Series[this.outputListViewItem.Count];        
            m_inSignal = new SignalType.Signals[this.inputListViewItem.Count];
            m_outSignal = new SignalType.Signals[this.outputListViewItem.Count];      

            for (int i = 0; i < this.inputListViewItem.Count; i++)
            {
                m_inSeries[i] = new Series();
                m_inSignal[i] = new SignalType.Signals();
            }

            for (int i = 0; i < this.outputListViewItem.Count; i++)
            {
                m_outSeries[i] = new Series();
                m_outSignal[i] = new SignalType.Signals();
            }

            m_panel = new Panel();
            m_trendChart = new Chart();
            m_chartArea = new ChartArea();
            m_legend = new Legend();

            m_trendChart.Width = 1800;
            m_trendChart.Height = 800;
            m_panel.Width = m_trendChart.Width;
            m_panel.Height = m_trendChart.Height;
            m_chartArea.Name = "One Trend View";
            m_trendChart.ChartAreas.Add(m_chartArea);

            int signalNum = 0;
            foreach (ListViewItem lvi in this.inputListViewItem)
            {
                m_inSignal[signalNum].Name = lvi.SubItems[(int)SignalType.ItemIndex.Name].Text;
                m_inSignal[signalNum].Value = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.Value].Text);
                m_inSignal[signalNum].Unit = lvi.SubItems[(int)SignalType.ItemIndex.Unit].Text;
                m_inSignal[signalNum].Min = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.Min].Text);
                m_inSignal[signalNum].Max = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.Max].Text);
                m_inSignal[signalNum].RawData = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.RawData].Text);
                m_inSignal[signalNum].SourceOrDestination = lvi.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text;

                m_inSeries[signalNum].ChartType = SeriesChartType.Line;
                m_inSeries[signalNum].Color = DefinedColors.GetColor(m_colorNumber++);
                m_inSeries[signalNum].BorderWidth = 2;
                m_inSeries[signalNum].BorderDashStyle = ChartDashStyle.Solid;
                m_inSeries[signalNum].Name = m_inSignal[signalNum].Name;         
                m_trendChart.Series.Add(m_inSeries[signalNum]);
                signalNum++;
            }

            signalNum = 0;
            foreach (ListViewItem lvi in this.outputListViewItem)
            {
                m_outSignal[signalNum].Name = lvi.SubItems[(int)SignalType.ItemIndex.Name].Text;
                m_outSignal[signalNum].Value = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.Value].Text);
                m_outSignal[signalNum].Unit = lvi.SubItems[(int)SignalType.ItemIndex.Unit].Text;
                m_outSignal[signalNum].Min = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.Min].Text);
                m_outSignal[signalNum].Max = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.Max].Text);
                m_outSignal[signalNum].RawData = Double.Parse(lvi.SubItems[(int)SignalType.ItemIndex.RawData].Text);
                m_outSignal[signalNum].SourceOrDestination = lvi.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text;

                m_outSeries[signalNum].ChartType = SeriesChartType.Line;
                m_outSeries[signalNum].Color = DefinedColors.GetColor(m_colorNumber++);
                m_outSeries[signalNum].BorderWidth = 2;
                m_outSeries[signalNum].BorderDashStyle = ChartDashStyle.Solid;
                m_outSeries[signalNum].Name = m_outSignal[signalNum].Name;
                m_trendChart.Series.Add(m_outSeries[signalNum]);
                signalNum++;
            }


            bigTrendTimer.Interval = 100;
            bigTrendTimer.Enabled = true;
            bigTrendTimer.Elapsed += BigViewOnTimedEvent;
            m_trendChart.Click += m_trendChart_click;

            m_chartArea.BackColor = System.Drawing.Color.Black;
            m_chartArea.AxisX.Title = "Time";
            m_chartArea.AxisX.TitleAlignment = StringAlignment.Center;
            m_chartArea.AxisX.Interval = 10;

            m_chartArea.AxisY.Title = "Signal Value";
           // m_chartArea.AxisY.TitleAlignment = StringAlignment.Near;
            m_chartArea.AxisY.TitleAlignment = StringAlignment.Center;
            m_chartArea.AxisY.IsLabelAutoFit = true;

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

            //m_chartArea.AxisY.Title = signal.Name.ToString();

            //chartArea.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            //chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            //chartArea.AxisY.MajorGrid.Enabled = true;
            //chartArea.AxisY.MinorGrid.LineColor = System.Drawing.Color.LightGray;


            //chartArea.AxisY.MinorGrid.Enabled = true;
            //chartArea.AxisY.MinorGrid.Interval = 1;


            //chartArea.AlignWithChartArea = chartArea.Name;
            //chartArea.AlignmentStyle = AreaAlignmentStyles.Position;

            m_chartArea.AlignmentOrientation = AreaAlignmentOrientations.Vertical;

            // chartArea.AlignmentOrientation = AreaAlignmentOrientations.Vertical;
            //chartArea.AlignWithChartArea="ChartAreas[0]";                 

            m_legend.Enabled = true;
            //m_legend.Position.X = 70;
            //m_legend.Position.Y = 10;
            m_trendChart.Legends.Add(m_legend);

            m_legend.DockedToChartArea = default;     
        }

        private void BigViewOnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
  
            MultipleUpdateTrendSignalValue();
            //Console.WriteLine("The Timer is counting {0}", x);   
            MultipleDrawLine();
            x += 1;
        }


        private void m_trendChart_click(object sender, EventArgs e)
        {
            if (bigTrendTimer.Enabled == true)
            {
                bigTrendTimer.Enabled = false;
            }
            else
            {
                bigTrendTimer.Enabled = true;
            }
        }

        public Panel CreateBigTrendViewPanel()
        {
            //Panel p = new Panel();

            m_panel.Controls.Add(m_trendChart);

            return m_panel;
        }



        public (SignalType.Signals[], SignalType.Signals[]) MultipleUpdateTrendSignalValue()
        {
           
            mut_b2.WaitOne();
            lock (Gateway.GetUpdatedInputFromGateway())
            {               
                foreach (SignalType.Signals sg in Gateway.GetUpdatedInputFromGateway().ToList())
                {
                    for (int i = 0; i < this.inputListViewItem.Count; i++)
                    {
                        if (sg.Name == m_inSignal[i].Name)
                        {
                            m_inSignal[i].Value = sg.Value;  
                        }
                    }
                }
            }

            //mut_b1.ReleaseMutex();

            // mut_b1.WaitOne();            
           

            lock (Gateway.GetUpdatedOutputFromGateway())
            {
                foreach (SignalType.Signals sg in Gateway.GetUpdatedOutputFromGateway().ToList())
                {
                    for (int i = 0; i < this.outputListViewItem.Count; i++)
                    {
                        if (sg.Name == m_outSignal[i].Name)
                        {
                            m_outSignal[i].Value = sg.Value;
                        }
                    }
                }
            }

            mut_b2.ReleaseMutex();
            return (m_inSignal, m_outSignal);
        }


        void MultipleDrawLine()
        {
            try
            {
                if (m_trendChart.InvokeRequired)
                {
                    drawMultiLineCallback w = new drawMultiLineCallback(MultipleDrawLine);
                    this.m_trendChart.Invoke(w, new object[] { });
                }
                else
                {
                    for (int i = 0; i < inputListViewItem.Count; i++)
                    {
                        m_inSeries[i].Points.AddXY(x, m_inSignal[i].Value);
                        if (m_inSeries[i].Points.Count > maxNumPoints)
                            m_inSeries[i].Points.RemoveAt(0);

                        m_chartArea.AxisX.Minimum = m_inSeries[i].Points[0].XValue;
                        m_chartArea.AxisX.Maximum = maxNumPoints + m_inSeries[i].Points[0].XValue;

                    }

                    for (int i = 0; i < outputListViewItem.Count; i++)
                    {
                        m_outSeries[i].Points.AddXY(x, m_outSignal[i].Value);
                        if (m_outSeries[i].Points.Count > maxNumPoints)
                            m_outSeries[i].Points.RemoveAt(0);

                        m_chartArea.AxisX.Minimum = m_outSeries[i].Points[0].XValue;
                        m_chartArea.AxisX.Maximum = maxNumPoints + m_outSeries[i].Points[0].XValue;
                    }
                }
            }
            catch (Exception e)
            {

            }

        }
    }
}
