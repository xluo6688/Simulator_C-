/// Komatsu Mining Autobolter Simulator 
/// Xuanwen Luo
/// 10/2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace AutobolterSim_cs
{
    public class SignalPanelCell
    {
        public delegate void updateSignalCellValueCallback(SignalType.Signals signal);
        public delegate void resetCheckBoxCallback();

        int gap = 15;
        int offset = 15;
        double newValue;
        Panel signalPanel;
        TextBox textBox;
        HScrollBar hScrollBar;
        CheckBox checkBox;
        double minValue = 0;
        //double maxValue = 6553500; // add two more zeros
       // double maxValue = 65535; // temp
      // Int64 maxValue = 2147483647; // add two more zeros
      //ListViewItem changedItem;
        double maxValue = 80000; // temp
        ListViewItem selectedSignalItem;  
        int order;
        SignalType.Signals signal;
        //SignalType.Signals modifiedSignal;      
        private static Mutex mut_c1 = new Mutex();
        private static Mutex mut_c2 = new Mutex();

        ~SignalPanelCell()
        {
            signal.Forced = false;
        } 

        public SignalType.Signals GetSignalOfCell()
        {
            SignalType.Signals sg;
            mut_c2.WaitOne();
            sg = signal;
            mut_c2.ReleaseMutex();
            return sg;
        }

      
        public SignalPanelCell(int order, ListViewItem listViewItem)
        {
            this.order = order;
            selectedSignalItem = listViewItem;

            signalPanel = new Panel();
            hScrollBar = new HScrollBar();
            textBox = new TextBox();
            checkBox = new CheckBox();

            textBox.Text = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Value].Text;
            hScrollBar.Minimum = Convert.ToInt32(minValue);
            hScrollBar.Maximum = Convert.ToInt32(maxValue);
            hScrollBar.Value = Convert.ToInt32(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Value].Text);

            signal.Name = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text;
            signal.Value = Double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Value].Text);
            signal.Unit = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Unit].Text;
            signal.Min = Double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Min].Text);
            signal.Max = Double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Max].Text);
            signal.RawData = Double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.RawData].Text);
            signal.SourceOrDestination = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text;
            signal.Forced = checkBox.Checked;

            //initiallize value
            //Console.WriteLine("SignalPanelCell.cs Constructor: (" + signal.Name + ", " + signal.Value.ToString() + ")");
        }       

        public Panel CreateOneSignalPanel(ListViewItem selectedSignalItem)
        {
            signalPanel.Width = 1000; signalPanel.Height = 35;
            signalPanel.Padding = new Padding(0);         

            int orderLabelPos_x = 10; int orderLabelPos_y = 5 + offset; int orderLabel_width = 30; int orderLabel_heigth = 25;
            Label lb_busSignal = CreateLabel(order.ToString(), "orderLabel" + order.ToString(), orderLabelPos_x, orderLabelPos_y, orderLabel_width, orderLabel_heigth);
            lb_busSignal.Padding = new Padding(0);
            signalPanel.Controls.Add(lb_busSignal);

            string scrollBarName = "sb" + order.ToString(); int sb_x = orderLabelPos_x + orderLabel_width + gap; int sb_y = -10 + offset; int sb_width = 400; int sb_height = 30;

            hScrollBar = CreateHorizontalScrollBar(hScrollBar, scrollBarName, sb_x, sb_y, sb_width, sb_height);
            hScrollBar.Padding = new Padding(0);
            hScrollBar.Anchor = new AnchorStyles();

            signalPanel.Controls.Add(hScrollBar);

            string textBoxName = "tb" + order.ToString(); int tb_x = sb_x + sb_width + gap; int tb_y = -8 + offset; int tb_width = 180; int tb_height = 30;
            textBox = CreateTextBox(textBox, textBoxName, tb_x, tb_y, tb_width, tb_height);
            textBox.Text = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Value].Text;
            signalPanel.Controls.Add(textBox);

            string checkBoxName = "cb" + order.ToString(); int cb_x = tb_x + tb_width + gap; int cb_y = -10 + offset; int cb_width = 15; int cb_height = 30;
            checkBox = CreateCheckBox(checkBox, checkBoxName, cb_x, cb_y, cb_width, cb_height);
            signalPanel.Controls.Add(checkBox);

            int signalNameLabelPos_x = cb_x + cb_width + gap - 5; int signalNameLabelPos_y = 3 + offset; int signalNameLabel_width = 300; int signalNameLabel_heigth = 25;
            signal.Name = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text;
            //Label lb_signalName = CreateLabel(signal.Name + ", " + selectedSignalItem.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text, selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text + order.ToString(), signalNameLabelPos_x, signalNameLabelPos_y, signalNameLabel_width, signalNameLabel_heigth);
            Label lb_signalName = CreateLabel(selectedSignalItem.Name + ", " + selectedSignalItem.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text, selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text + order.ToString(), signalNameLabelPos_x, signalNameLabelPos_y, signalNameLabel_width, signalNameLabel_heigth);
            
            lb_signalName.Padding = new Padding(0);
            signalPanel.Controls.Add(lb_signalName);

            hScrollBar.Scroll += new ScrollEventHandler(hScrollBar_Scroll);
            hScrollBar.ValueChanged += new EventHandler(hScrollBar_ValueChanged);
            textBox.TextChanged += new EventHandler(textBox_TextChanged);                

            if (textBox.Text == "")
            {
                newValue = 0;
                textBox.Text = newValue.ToString();
            }

            return signalPanel;
        }
               

        public void UpdateCellValue(SignalType.Signals signal)
        {            
            if (!this.checkBox.Checked & (this.signal.Name == signal.Name))
            {
                try
                {
                    if (textBox.InvokeRequired)
                    {
                        updateSignalCellValueCallback w = new updateSignalCellValueCallback(UpdateCellValue);
                        this.textBox.Invoke(w, new object[] { signal });
                    }
                    else
                    {
                        this.textBox.Text = signal.Value.ToString();
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    if (this.hScrollBar.InvokeRequired)
                    {
                        updateSignalCellValueCallback w = new updateSignalCellValueCallback(UpdateCellValue);
                        this.hScrollBar.Invoke(w, new object[] { signal });
                    }
                    else
                    {
                        this.hScrollBar.Value = Convert.ToInt32(signal.Value);
                    }
                }
                catch (Exception e)
                {

                }
            }
           
        }

        
        public SignalType.Signals UpdateSignalValue()
        {
            mut_c1.WaitOne();
            signal.Name = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Name].Text;
            signal.Value = hScrollBar.Value;
            signal.Unit = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Unit].Text;
            signal.Min = double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Min].Text);
            signal.Max = double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.Max].Text);
            signal.RawData = double.Parse(selectedSignalItem.SubItems[(int)SignalType.ItemIndex.RawData].Text);
            signal.SourceOrDestination = selectedSignalItem.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text;
            signal.Forced = checkBox.Checked;
            mut_c1.ReleaseMutex();

            return signal;
        }      

        private void hScrollBar_Scroll(Object sender, ScrollEventArgs e)
        {        
            checkBox.Checked = true;
            signal.Forced = true;
            UpdateSignalValue();
        }

        private void hScrollBar_ValueChanged(Object sender, EventArgs e)
        {
            textBox.Text = hScrollBar.Value.ToString();
            UpdateSignalValue();
        }


        private void textBox_TextChanged(Object sender, EventArgs e)
        {
            if (textBox.Text == "")
            {
                newValue = 0;
            }  
            else if (!Int32.TryParse(textBox.Text, out Int32 vale))
            {
                newValue = 0;
                MessageBox.Show("Only accept positive number !");               
            }
            else
            {
                newValue = Int32.Parse(textBox.Text);
                if (newValue > maxValue)
                {
                    newValue = maxValue;
                }

                textBox.Text = newValue.ToString(); // make sure the limitation is the max value
                hScrollBar.Value = Convert.ToInt32(newValue);

                UpdateSignalValue();
            }
           
        }


        private Label CreateLabel(string labelText, string labelName, int pos_x, int pos_y, int width, int heigth)
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

        private HScrollBar CreateHorizontalScrollBar(HScrollBar hScrollBar, string name, int pos_x, int pos_y, int width, int height)
        {        
            hScrollBar.Name = name;
            hScrollBar.Width = width; hScrollBar.Height = height;
            hScrollBar.Location = new System.Drawing.Point(pos_x, pos_y);

            return hScrollBar;
        }

        private TextBox CreateTextBox(TextBox textBox, string name, int pos_x, int pos_y, int width, int height)
        {           
            textBox.Enabled = true;
            textBox.Name = name;
            textBox.Location = new System.Drawing.Point(pos_x, pos_y);
            textBox.Width = width; textBox.Height = height;
            int fontSize = 12;
            textBox.Font = new System.Drawing.Font("Serif", fontSize, System.Drawing.FontStyle.Regular);

            return textBox;
        }

        private CheckBox CreateCheckBox(CheckBox checkBox, string name, int pos_x, int pos_y, int width, int height)
        {
            checkBox.Name = name;
            checkBox.Text = "";
            checkBox.AutoSize = false;
            checkBox.Size = new System.Drawing.Size(100, 100);

            checkBox.Location = new System.Drawing.Point(pos_x, pos_y);
            checkBox.Width = width; checkBox.Height = height;
            checkBox.Click += new EventHandler(CheckBox_CheckedChanged);            

            return checkBox;
        }

        private void CheckBox_CheckedChanged(Object sender, EventArgs e)
        {
            if (checkBox.Checked)
            {
                signal.Forced = true;                
            }
            else
            {
                signal.Forced = false;                
            }

            //UpdateSignalValue();
        }

        public void ResetCheckBox()
        {
            try
            {
                if (checkBox.InvokeRequired)
                {
                    resetCheckBoxCallback w = new resetCheckBoxCallback(ResetCheckBox);
                    checkBox.Invoke(w, new object[] { });
                }
                else
                {
                    this.checkBox.Checked = false;
                }
            }
            catch (Exception e)
            {

            }

            UpdateSignalValue();
        }
    }
}
