/// Komatsu Mining Autobolter Simulator 
/// Xuanwen Luo
/// 10/2020

using System.Collections.Generic;
using System;
using System.Windows.Forms;

namespace AutobolterSim_cs
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        delegate void WriteSignalCallback(List<SignalType.Signals> signalList);
        delegate void ClearSignalCallback();       

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();                
            }
            base.Dispose(disposing);
        }       
             

        public void UpdateSignalPoolView()  
        {
            List<SignalType.Signals>  inputSignalFromGateway = Gateway.ReadInputSignals();
            List<SignalType.Signals>  outputSignalFromGateway = Gateway.ReadOutputSignals();
            List<SignalType.Signals>  systemSignalFromGateway = Gateway.ReadSystemSignalsFromController();

            WriteSignalToInputView(inputSignalFromGateway);
            WriteSignalToOutputView(outputSignalFromGateway);
            WriteSignalToSystemSignalView(systemSignalFromGateway);
        }

        public void WriteSignalToInputView(List<SignalType.Signals> inputSignalList)
        {            
            try
            {        
                if (listView_inputSignal.InvokeRequired)
                {
                    WriteSignalCallback w = new WriteSignalCallback(WriteSignalToInputView);
                    listView_inputSignal.Invoke(w, new object[] { inputSignalList });      
                }
                else
                {
                   listView_inputSignal.BeginUpdate(); 
                   listView_inputSignal = UpdateSignalToListView(listView_inputSignal, inputSignalList);              
                   listView_inputSignal.EndUpdate();       
                }        
            }
            catch (Exception e)
            {

            }          
            
        }


        public void WriteSignalToOutputView(List<SignalType.Signals> outputSignalList)
        {
            try
            {
                if (listView_outputSignal.InvokeRequired)
                {
                    WriteSignalCallback w = new WriteSignalCallback(WriteSignalToOutputView);
                    listView_outputSignal.Invoke(w, new object[] { outputSignalList });
                }
                else
                {
                    listView_outputSignal.BeginUpdate();
                    listView_outputSignal = UpdateSignalToListView(listView_outputSignal, outputSignalList);            
                    listView_outputSignal.EndUpdate();
                }
            }
            catch (Exception e)
            {

            }

        }

        public void WriteSignalToSystemSignalView(List<SignalType.Signals> systemSignalList)
        {
            try
            {
                if (listView_SystemSignal.InvokeRequired)  
                {
                    WriteSignalCallback w = new WriteSignalCallback(WriteSignalToSystemSignalView);
                    listView_SystemSignal.Invoke(w, new object[] { systemSignalList });
                }
                else
                {
                    listView_SystemSignal.BeginUpdate();
                    listView_SystemSignal = UpdateSignalToListView(listView_SystemSignal, systemSignalList);   
                    listView_SystemSignal.EndUpdate();
                }
            }
            catch (Exception e)
            {

            }

        }
        


        public ListView CreateListView(ListView listView)        
        {                      
            int fontSize = 9;            
            listView.Scrollable = true;
            listView.Font = new System.Drawing.Font("Serif", fontSize, System.Drawing.FontStyle.Regular);
            listView.AutoArrange = false;

            return listView;
        }

        public ListView AddSourceColumnHeadToListView(ListView lv)
        {
            lv.CheckBoxes = true;
            lv.Scrollable = true;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.HideSelection = false;
            // lv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lv.Sorting = SortOrder.Ascending;
            //lv.Sorting = SortOrder.None;
            lv.AllowColumnReorder = true;
            //lv.MultiSelect = true;

            lv.UseCompatibleStateImageBehavior = false;
            //lv.View = System.Windows.Forms.View.Details;
            lv.View = View.Details;
           
            //lv.Columns.Add(new ColumnHeader());
            //lv.Columns[0].Text = "No.";
            //lv.Columns[0].Width = 30;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[0].Text = "   ";
            lv.Columns[0].Width = 20;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[1].Text = "Name";
            lv.Columns[1].Width = 300;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[2].Text = "Value";
            lv.Columns[2].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[3].Text = "Unit";
            lv.Columns[3].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[4].Text = "Min";
            lv.Columns[4].Width = 50;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[5].Text = "Max";
            lv.Columns[5].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[6].Text = "Raw";
            lv.Columns[6].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[7].Text = "Source";
            lv.Columns[7].Width = 120;
            lv.SuspendLayout();

            return lv;
        }


        static public ListView AddDesitinationColumnHeadToListView(ListView lv)
        {
            lv.CheckBoxes = true;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.HideSelection = false;
            // lv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lv.Sorting = SortOrder.Ascending;
            lv.UseCompatibleStateImageBehavior = false;
            //lv.View = System.Windows.Forms.View.Details;
            lv.View = View.Details;
            //lv.Columns.Add(new ColumnHeader());
            //lv.Columns[0].Text = "No"; 
            //lv.Columns[0].Width = 30;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[0].Text = "   ";
            lv.Columns[0].Width = 20;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[1].Text = "Name";
            lv.Columns[1].Width = 300;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[2].Text = "Value";
            lv.Columns[2].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[3].Text = "Unit";
            lv.Columns[3].Width = 50;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[4].Text = "Min";
            lv.Columns[4].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[5].Text = "Max";
            lv.Columns[5].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[6].Text = "Raw";
            lv.Columns[6].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[7].Text = "Destination";
            lv.Columns[7].Width = 150;
            lv.SuspendLayout();

            return lv;
        }


        public ListView AddSystemColumnHeadToListView(ListView lv)
        {
            lv.CheckBoxes = false;
            lv.Scrollable = true;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.HideSelection = false;
            // lv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lv.Sorting = SortOrder.Ascending;
            //lv.Sorting = SortOrder.None;
            lv.AllowColumnReorder = true;
            //lv.MultiSelect = true;

            lv.UseCompatibleStateImageBehavior = false;
            //lv.View = System.Windows.Forms.View.Details;
            lv.View = View.Details;

            //lv.Columns.Add(new ColumnHeader());
            //lv.Columns[0].Text = "No.";
            //lv.Columns[0].Width = 30;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[0].Text = "";
            lv.Columns[0].Width = 20;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[1].Text = "Name";
            lv.Columns[1].Width = 300;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[2].Text = "Value";
            lv.Columns[2].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[3].Text = "Unit";
            lv.Columns[3].Width = 50;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[4].Text = "Min";
            lv.Columns[4].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[5].Text = "Max";
            lv.Columns[5].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[6].Text = "Raw";
            lv.Columns[6].Width = 100;
            lv.Columns.Add(new ColumnHeader());
            lv.Columns[7].Text = "Source";
            lv.Columns[7].Width = 150;
            lv.SuspendLayout();

            return lv;
        }
                              
        public ListView AddSignalToListView(ListView baseListView, List<SignalType.Signals> signalList)
        {         
            List<ListViewItem> listOfItems = new List<ListViewItem>();         
            for (int i = 0; i < signalList.Count; i++)
            {
                //ListViewItem item = new ListViewItem(i.ToString());
                ListViewItem item = new ListViewItem();               
                item.Name = signalList[i].Name;
                item.SubItems.Add(signalList[i].Name);
                item.SubItems.Add(signalList[i].Value.ToString());
                item.SubItems.Add(signalList[i].Unit);
                item.SubItems.Add(signalList[i].Min.ToString());
                item.SubItems.Add(signalList[i].Max.ToString());
                item.SubItems.Add(signalList[i].RawData.ToString());
                item.SubItems.Add(signalList[i].SourceOrDestination);
                //item.Checked = true;
                listOfItems.Add(item);
            }

            baseListView.BeginUpdate();
            baseListView.Items.Clear();
            baseListView.Items.AddRange(listOfItems.ToArray());            
            baseListView.EndUpdate();

            return baseListView;
        }

        public ListView UpdateSignalToListView(ListView baseListView, List<SignalType.Signals> signalList)
        {
            baseListView.BeginUpdate();
            if (baseListView.Items.Count > 0)
            {
                for (int i = 0; i < baseListView.Items.Count; i++)
                {
                    if (signalList.Count > 0)
                    {
                        foreach (SignalType.Signals sg in signalList)
                        {
                            if (baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.Name].Text == sg.Name)
                            {
                                if (sg.Forced)
                                {
                                    baseListView.Items[i].UseItemStyleForSubItems = false;
                                    baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.Value].ForeColor = System.Drawing.Color.Blue;                               
                                    baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.Value].Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Italic ^ System.Drawing.FontStyle.Bold);
                                    baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.RawData].ForeColor = System.Drawing.Color.Blue;
                                    baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.RawData].Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Italic ^ System.Drawing.FontStyle.Bold);
                               
                                }
                                else
                                {
                                    baseListView.Items[i].UseItemStyleForSubItems = true;
                                }                                
                                baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.Value].Text = sg.Value.ToString();
                                baseListView.Items[i].SubItems[(int)SignalType.ItemIndex.RawData].Text = sg.RawData.ToString();
                            }
                        }
                    }

                }
               
            }
            
            baseListView.EndUpdate();

            return baseListView;
        }        
             

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.selectIOsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trendViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lb_inputSignal = new System.Windows.Forms.Label();
            this.lb_outputSignal = new System.Windows.Forms.Label();
            this.listView_inputSignal = new System.Windows.Forms.ListView();
            this.listView_outputSignal = new System.Windows.Forms.ListView();
            this.lb_systemSignal = new System.Windows.Forms.Label();
            this.listView_SystemSignal = new System.Windows.Forms.ListView();
            this.oneTrendViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multipleTrendViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectIOsToolStripMenuItem,
            this.trendViewToolStripMenuItem,
            this.helpsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2543, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // selectIOsToolStripMenuItem
            // 
            this.selectIOsToolStripMenuItem.Name = "selectIOsToolStripMenuItem";
            this.selectIOsToolStripMenuItem.Size = new System.Drawing.Size(141, 24);
            this.selectIOsToolStripMenuItem.Text = "Selected IOs View";
            this.selectIOsToolStripMenuItem.Click += new System.EventHandler(this.selectIOsToolStripMenuItem_Click);
            // 
            // trendViewToolStripMenuItem
            // 
            this.trendViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oneTrendViewToolStripMenuItem,
            this.multipleTrendViewsToolStripMenuItem});
            this.trendViewToolStripMenuItem.Name = "trendViewToolStripMenuItem";
            this.trendViewToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.trendViewToolStripMenuItem.Text = "Trend View";
            this.trendViewToolStripMenuItem.Click += new System.EventHandler(this.trendViewToolStripMenuItem_Click);
            // 
            // helpsToolStripMenuItem
            // 
            this.helpsToolStripMenuItem.Name = "helpsToolStripMenuItem";
            this.helpsToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            this.helpsToolStripMenuItem.Text = "Helps";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // lb_inputSignal
            // 
            this.lb_inputSignal.AutoSize = true;
            this.lb_inputSignal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_inputSignal.Location = new System.Drawing.Point(12, 45);
            this.lb_inputSignal.Name = "lb_inputSignal";
            this.lb_inputSignal.Size = new System.Drawing.Size(138, 25);
            this.lb_inputSignal.TabIndex = 2;
            this.lb_inputSignal.Text = "Input Signals";
            // 
            // lb_outputSignal
            // 
            this.lb_outputSignal.AutoSize = true;
            this.lb_outputSignal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_outputSignal.Location = new System.Drawing.Point(1235, 45);
            this.lb_outputSignal.Name = "lb_outputSignal";
            this.lb_outputSignal.Size = new System.Drawing.Size(155, 25);
            this.lb_outputSignal.TabIndex = 3;
            this.lb_outputSignal.Text = "Output Signals";
            // 
            // listView_inputSignal
            // 
            this.listView_inputSignal.HideSelection = false;
            this.listView_inputSignal.Location = new System.Drawing.Point(17, 78);
            this.listView_inputSignal.Name = "listView_inputSignal";
            this.listView_inputSignal.Size = new System.Drawing.Size(1217, 868);
            this.listView_inputSignal.TabIndex = 4;
            this.listView_inputSignal.UseCompatibleStateImageBehavior = false;
            // 
            // listView_outputSignal
            // 
            this.listView_outputSignal.HideSelection = false;
            this.listView_outputSignal.Location = new System.Drawing.Point(1240, 78);
            this.listView_outputSignal.Name = "listView_outputSignal";
            this.listView_outputSignal.Size = new System.Drawing.Size(1217, 562);
            this.listView_outputSignal.TabIndex = 5;
            this.listView_outputSignal.UseCompatibleStateImageBehavior = false;
            // 
            // lb_systemSignal
            // 
            this.lb_systemSignal.AutoSize = true;
            this.lb_systemSignal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_systemSignal.Location = new System.Drawing.Point(1240, 654);
            this.lb_systemSignal.Name = "lb_systemSignal";
            this.lb_systemSignal.Size = new System.Drawing.Size(206, 25);
            this.lb_systemSignal.TabIndex = 6;
            this.lb_systemSignal.Text = "System/Bus Signals";
            // 
            // listView_SystemSignal
            // 
            this.listView_SystemSignal.HideSelection = false;
            this.listView_SystemSignal.Location = new System.Drawing.Point(1240, 689);
            this.listView_SystemSignal.Name = "listView_SystemSignal";
            this.listView_SystemSignal.Size = new System.Drawing.Size(1217, 257);
            this.listView_SystemSignal.TabIndex = 7;
            this.listView_SystemSignal.UseCompatibleStateImageBehavior = false;
            // 
            // oneTrendViewToolStripMenuItem
            // 
            this.oneTrendViewToolStripMenuItem.Name = "oneTrendViewToolStripMenuItem";
            this.oneTrendViewToolStripMenuItem.Size = new System.Drawing.Size(226, 26);
            this.oneTrendViewToolStripMenuItem.Text = "One Trend View";
            this.oneTrendViewToolStripMenuItem.Click += new System.EventHandler(this.oneTrendViewToolStripMenuItem_Click);
            // 
            // multipleTrendViewsToolStripMenuItem
            // 
            this.multipleTrendViewsToolStripMenuItem.Name = "multipleTrendViewsToolStripMenuItem";
            this.multipleTrendViewsToolStripMenuItem.Size = new System.Drawing.Size(226, 26);
            this.multipleTrendViewsToolStripMenuItem.Text = "MultipleTrend Views";
            this.multipleTrendViewsToolStripMenuItem.Click += new System.EventHandler(this.multipleTrendViewsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(2543, 1006);
            this.Controls.Add(this.listView_SystemSignal);
            this.Controls.Add(this.lb_systemSignal);
            this.Controls.Add(this.listView_outputSignal);
            this.Controls.Add(this.listView_inputSignal);
            this.Controls.Add(this.lb_outputSignal);
            this.Controls.Add(this.lb_inputSignal);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Auto Bolter System Signals";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectIOsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trendViewToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label lb_inputSignal;
        private System.Windows.Forms.Label lb_outputSignal;
        private System.Windows.Forms.ListView listView_inputSignal;
        private System.Windows.Forms.ListView listView_outputSignal;
        private System.Windows.Forms.Label lb_systemSignal;
        private System.Windows.Forms.ListView listView_SystemSignal;
        private ToolStripMenuItem helpsToolStripMenuItem;
        private ToolStripMenuItem oneTrendViewToolStripMenuItem;
        private ToolStripMenuItem multipleTrendViewsToolStripMenuItem;
    }
}

