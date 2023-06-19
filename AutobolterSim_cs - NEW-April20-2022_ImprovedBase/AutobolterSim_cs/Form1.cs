/// Komatsu Mining Autobolter Simulator 
/// Xuanwen Luo
/// 10/2020


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace AutobolterSim_cs
{
    public delegate void modifiedSignalCallback(SignalType.Signals signal);
    public delegate (List<ListViewItem>, List<ListViewItem>) GetCheckedListViewItemsCallback(ListView inputView, ListView outputView);
    public delegate void clearViewaCallback();
    public delegate ListView ResetCheckboxCallback(ListView listview);
    public delegate void form2_SelectedSignalViewClosedCallback(object sender, FormClosedEventArgs e);
    public delegate void oneTrendForm_SelectedSignalViewClosedCallback(object sender, FormClosedEventArgs e);

    //public delegate void OnTimedEventForOneTrendViewCallback(Object source, System.Timers.ElapsedEventArgs e);



    public partial class Form1 : Form
    {
        System.Timers.Timer oneTrendViewTimer = new System.Timers.Timer();

        int samplingTime = 1000;
        //int trendViewSamplingTime = 1000;
        //double x_time = 0;
        //double x_value = 0;
        //int maxNumOfCells = 50;
        //int maxNumOfTrendViewCells = 10;
        /* These two for SignalPanelCell */

        //List<SignalType.Signals> inputSignalFromGateway;
        //List<SignalType.Signals> outputSignalFromGateway;
        //List<SignalType.Signals> systemSignalFromGateway;

        List<ListViewItem> inputCheckedItemList;
        List<ListViewItem> outputCheckedItemList;    

        List<SignalType.Signals> inputForcedSignals = new List<SignalType.Signals>();
        List<SignalType.Signals> outputForcedSignals = new List<SignalType.Signals>();

        static int selectedSignalViewOrder = 1;
        static int selectedSignalTrendViewOrder = 1;
        static int selectedSignalOneTrendViewOrder = 1;

        public static SignalType.Signals modifiedInputs;
        public static SignalType.Signals modifiedOutputs;

        // to check if need them
        public static List<SignalType.Signals> totalInputCheckededSignals;
        public static List<SignalType.Signals> totalOutputCheckedSignals; 

        int maxNumberOfSelectedSignalView = 1;
        int maxNumberOfSelectedSignalTrendView = 10;
        int maxNumberOfOneBigTrendView = 10;
        int numberOfSelectedSignalView = 0;
        int numberOfSelectedSignalTrendView = 0;
        int numberOfSelectedSignalOneTrendView = 0;
        
        Form2 form2;
        TrendForm trendForm;
        OneTrendForm oneTrendForm;
        Panel signalPanel;
        Panel trendPanel;      

        public SignalPanelCell[] inputSignalCell;
        public SignalPanelCell[] outputSignalCell;
        public TrendViewCell[] inputSignalTrendViewCell; 
        public TrendViewCell[] outputSignalTrendViewCell;  

        Thread selectedSignalViewThread;
        Thread UpdataSignalCellValueThread;
        Thread selectedSignalTrendViewThread;      

        Thread[] trendPanelThread;
        Thread selectedSignalOneTrendViewThread;
        private static Mutex mut1 = new Mutex();
        //private const int numIterations = 1;
        //private const int numThreads = 3;

        //private static Mutex mut2 = new Mutex();
        //private static Mutex mut3 = new Mutex();
        //private static Mutex mut_oneTrendView = new Mutex();
        

        public Form1()
        {
            InitializeComponent();
            ViewInitialization();
        }

        private void ViewInitialization()
        {
            listView_inputSignal = CreateListView(listView_inputSignal);
            listView_inputSignal = AddSourceColumnHeadToListView(listView_inputSignal);
            listView_inputSignal = AddSignalToListView(listView_inputSignal, Gateway.ReadInputSignals());     
            listView_inputSignal.DoubleBuffering(true);

            listView_outputSignal = CreateListView(listView_outputSignal);
            listView_outputSignal = AddDesitinationColumnHeadToListView(listView_outputSignal);
            listView_outputSignal = AddSignalToListView(listView_outputSignal, Gateway.ReadOutputSignals());
            listView_outputSignal.DoubleBuffering(true);

            listView_SystemSignal = CreateListView(listView_SystemSignal);
            listView_SystemSignal = AddSystemColumnHeadToListView(listView_SystemSignal);
            listView_SystemSignal = AddSignalToListView(listView_SystemSignal, Gateway.ReadSystemSignalsFromController());
            listView_SystemSignal.DoubleBuffering(true);            
        }       
       
               
        public (List<ListViewItem>, List<ListViewItem>) GetCheckedListViewItems(ListView listView_inputSignal, ListView listView_outputSignal) // only consider return input and output currently 
        {
            List<ListViewItem> inputList = new List<ListViewItem>();
            List<ListViewItem> outputList = new List<ListViewItem>();
            List<ListViewItem> systemBusList = new List<ListViewItem>();// not used           

            try
            {
                if (listView_inputSignal.InvokeRequired)
                {
                    GetCheckedListViewItemsCallback w = new GetCheckedListViewItemsCallback(GetCheckedListViewItems);
                    listView_inputSignal.Invoke(w, new object[] { listView_inputSignal, listView_outputSignal});
                }
                else
                {
                    foreach (ListViewItem item in listView_inputSignal.Items)
                    {
                        if (item.Checked == true)
                        {
                            inputList.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }


            try
            {
                if (listView_outputSignal.InvokeRequired)
                {
                    GetCheckedListViewItemsCallback w = new GetCheckedListViewItemsCallback(GetCheckedListViewItems);
                    listView_outputSignal.Invoke(w, new object[] { listView_inputSignal, listView_outputSignal });
                }
                else
                {
                    foreach (ListViewItem item in listView_outputSignal.Items)
                    {
                        if (item.Checked == true)
                        {
                            outputList.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            try
            {
                if (listView_SystemSignal.InvokeRequired)
                {
                    GetCheckedListViewItemsCallback w = new GetCheckedListViewItemsCallback(GetCheckedListViewItems);
                    listView_SystemSignal.Invoke(w, new object[] { listView_inputSignal, listView_outputSignal });
                }
                else
                {
                    foreach (ListViewItem item in listView_SystemSignal.Items)
                    {
                        if (item.Checked == true)
                        {
                            systemBusList.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }       

            return (inputList, outputList);
        }

               
        private void selectIOsToolStripMenuItem_Click(object sender, EventArgs e)
        {             

            if (numberOfSelectedSignalView < maxNumberOfSelectedSignalView)
            {
                (inputCheckedItemList, outputCheckedItemList) = GetCheckedListViewItems(listView_inputSignal, listView_outputSignal);               

                if (inputCheckedItemList.Count == 0 & outputCheckedItemList.Count == 0)
                {
                    MessageBox.Show("Form1: Please select signals !, You selected " + (inputCheckedItemList.Count + outputCheckedItemList.Count).ToString() + " signals !");
                }
                else
                {
                    // add checked signals from a new selected singal view       
                    form2 = new Form2(selectedSignalViewOrder);
                    form2.Name = "SelectedSignalView" + selectedSignalViewOrder.ToString();
                    form2.Text = "Selected IO Signals Panel " + selectedSignalViewOrder.ToString();
                    form2.FormClosed += new FormClosedEventHandler(form2_SelectedSignalViewClosed);            
                    selectedSignalViewThread = new Thread(() => CreateSelectedSignalView(form2, inputCheckedItemList, outputCheckedItemList));
                    selectedSignalViewThread.Start();
                    numberOfSelectedSignalView++;
                    selectedSignalViewOrder++;                    
                }
            }
            else
            {
                MessageBox.Show("The max number of selected signal view is reached, close one or more selected signal views opened and try agian !");
            }

            ResetCheckbox(listView_inputSignal);
            ResetCheckbox(listView_outputSignal);
            ResetCheckbox(listView_SystemSignal);
        }       

        private List<SignalType.Signals> ItemListToSignalList(List<ListViewItem> ItemList)
        {
            List<SignalType.Signals> signalList = new List<SignalType.Signals>();
            SignalType.Signals sg;

            if (ItemList.Count > 0)
            {
                foreach (ListViewItem item in ItemList)
                {
                    sg.Name = item.SubItems[(int)SignalType.ItemIndex.Name].Text;               
                    sg.Value = Double.Parse(item.SubItems[(int)SignalType.ItemIndex.Value].Text);
                    sg.Unit = item.SubItems[(int)SignalType.ItemIndex.Unit].Text;
                    sg.Min = Double.Parse(item.SubItems[(int)SignalType.ItemIndex.Min].Text);
                    sg.Max = Double.Parse(item.SubItems[(int)SignalType.ItemIndex.Max].Text);
                    sg.RawData = Double.Parse(item.SubItems[(int)SignalType.ItemIndex.RawData].Text);
                    sg.SourceOrDestination = item.SubItems[(int)SignalType.ItemIndex.SourceOrDestination].Text;  
                    sg.Forced = false;
                    signalList.Add(sg);
                }
            }

            return signalList;
        }

        public ListView ResetCheckbox(ListView listview)
        {           
            try
            {
                if (listview.InvokeRequired)
                {
                    MessageBox.Show("ResetCheckbox,  InvokeRequired !");

                    ResetCheckboxCallback c = new ResetCheckboxCallback(ResetCheckbox);
                    listview.Invoke(c, new object[] { listview });
                }
                else
                {
                    foreach (ListViewItem lvi in listview.Items)
                    {
                        lvi.Checked = false;
                    }
                }
            }
            catch (Exception e)
            {

            }       
            
            return listview;
        }

        private List<SignalType.Signals> updateTotalCheckedSignals(List<SignalType.Signals> signals, List<SignalType.Signals> totalSignals)
        {
            if (totalSignals.Count <= 0)
            {
                totalSignals = signals;
            }
            else
            {
                if (signals.Count > 0)
                {
                    foreach (SignalType.Signals sg in signals)
                    {
                        if (!HasIt(totalSignals, sg))
                        {
                            totalSignals.Add(sg);
                        }
                    }
                }
            }

            return totalSignals;
        }

        public bool HasIt(List<SignalType.Signals> SignalList, SignalType.Signals signal)
        {
            foreach (SignalType.Signals sg in SignalList)
            {
                if (sg.Name.Equals(signal.Name))
                {
                    return true;
                }
            }

            return false;
        }       

        void form2_SelectedSignalViewClosed(object sender, FormClosedEventArgs e)
        {
            ResetSignalCellCheckBox();
            Gateway.ResetForcedSignals();
            numberOfSelectedSignalView--;
            inputCheckedItemList.Clear();
            outputCheckedItemList.Clear();
            inputForcedSignals.Clear();
            outputForcedSignals.Clear();
            selectedSignalViewOrder = 1; // reset it if only allow have one view 
            form2.Dispose();
            signalPanel.Dispose();
            UpdataSignalCellValueThread.Abort();         
        }

        public void CreateSelectedSignalView(Form2 form2, List<ListViewItem> i_inputCheckedItemList, List<ListViewItem>  i_outputCheckedItemList)
        {  
            //Currently only limited to create one selected signal view 

            int signalOrder = 1;
            int inputCellNumber = 0;
            int outputCellNumber = 0;

            inputSignalCell = new SignalPanelCell[i_inputCheckedItemList.Count];
            outputSignalCell = new SignalPanelCell[i_outputCheckedItemList.Count];

            if (i_inputCheckedItemList.Count > 0)
            {                
                foreach (ListViewItem lvi in i_inputCheckedItemList)
                {
                    inputSignalCell[inputCellNumber] = new SignalPanelCell(signalOrder, lvi);
                    signalPanel = inputSignalCell[inputCellNumber].CreateOneSignalPanel(lvi);    
                    int signaPanel_x = 0; int signaPanel_y = signalOrder * signalPanel.Height;
                    signalPanel.Location = new System.Drawing.Point(signaPanel_x, signaPanel_y);
                    form2.Controls.Add(signalPanel);
                    signalOrder++;
                    
                    inputForcedSignals.Add(inputSignalCell[inputCellNumber].UpdateSignalValue()); // get all signals from selected signal view that could be forced
                    inputCellNumber++;
                }
               
            }

            if (i_outputCheckedItemList.Count > 0)
            {                
                foreach (ListViewItem lvi in i_outputCheckedItemList)
                {
                    outputSignalCell[outputCellNumber] = new SignalPanelCell(signalOrder, lvi);
                    Panel signalPanel = outputSignalCell[outputCellNumber].CreateOneSignalPanel(lvi);             
                    int signaPanel_x = 0; int signaPanel_y = signalOrder * signalPanel.Height;
                    signalPanel.Location = new System.Drawing.Point(signaPanel_x, signaPanel_y);
                    form2.Controls.Add(signalPanel);

                    signalOrder++;  
                    outputForcedSignals.Add(outputSignalCell[outputCellNumber].UpdateSignalValue());  // get all signals from selected signal view that could be forced
                    outputCellNumber++;
                }

            }     

            UpdataSignalCellValueThread = new Thread(() => UpdataSignalCellValue(form2, inputSignalCell, outputSignalCell, inputForcedSignals, outputForcedSignals));
            UpdataSignalCellValueThread.Start(); 
            
            foreach (SignalType.Signals sg in inputForcedSignals)
            {
                Console.WriteLine("From Form1: inputForcedSignals name = " + sg.Name + "Forced = " + sg.Forced.ToString());
            }            

            Application.Run(form2);
        }


        private List<SignalType.Signals> RemoveSignal(List<SignalType.Signals> signals, SignalType.Signals signal)
        {
           if (HasIt(signals, signal)){    
                signals.Remove(signals.ToList().Single(sg => sg.Name == signal.Name));   
            }

            return signals;
        }

        public void UpdataSignalCellValue(Form2 form2, SignalPanelCell[] inputSignalCell, SignalPanelCell[] outputSignalCell, List<SignalType.Signals> i_inputForcedSignals, List<SignalType.Signals> i_outputForcedSignals)
        {
            while (true)
            {
                mut1.WaitOne();

                List<SignalType.Signals> inputSignalFromGateway = Gateway.ReadInputSignals();
                List<SignalType.Signals> outputSignalFromGateway = Gateway.ReadOutputSignals();                

                for (int i = 0; i < i_inputForcedSignals.ToList().Count; i++)           
                {
                    for (int j = 0; j < inputSignalFromGateway.Count; j++)
                    {
                        
                        if (inputSignalCell[i].GetSignalOfCell().Name.Equals(inputSignalFromGateway[j].Name))
                        {
                            i_inputForcedSignals = RemoveSignal(i_inputForcedSignals, inputSignalCell[i].GetSignalOfCell()); // using sg.Name
                            i_inputForcedSignals.Add(inputSignalCell[i].UpdateSignalValue()); // update data and signal check box status from signal cell 
                            inputSignalCell[i].UpdateCellValue(inputSignalFromGateway[j]); // update signal cell data value with sg          
                        }
                        else
                        {
                            i_inputForcedSignals = RemoveSignal(i_inputForcedSignals, inputSignalCell[i].GetSignalOfCell()); // using sg.Name
                            i_inputForcedSignals.Add(inputSignalCell[i].UpdateSignalValue()); // update data and signal check box status from signal cell 
                        }
                    }
                }
                mut1.ReleaseMutex();

                mut1.WaitOne();

                for (int i = 0; i < i_outputForcedSignals.ToList().Count; i++)                             
                {
                    for (int j = 0; j < outputSignalFromGateway.Count; j++)
                    {                        
                        if (outputSignalCell[i].GetSignalOfCell().Name.Equals(outputSignalFromGateway[j].Name))
                        {
                            if (!outputSignalCell[i].GetSignalOfCell().Forced)
                            {
                                i_outputForcedSignals = RemoveSignal(i_outputForcedSignals, outputSignalFromGateway[j]);
                                i_outputForcedSignals.Add(outputSignalCell[i].UpdateSignalValue()); // update data and signal check box status from signal cell 
                                outputSignalCell[i].UpdateCellValue(outputSignalFromGateway[j]); // update signal cell data with sg          
                            }
                            else
                            {
                                i_outputForcedSignals = RemoveSignal(i_outputForcedSignals, outputSignalFromGateway[j]);
                                i_outputForcedSignals.Add(outputSignalCell[i].UpdateSignalValue()); // update data and signal check box status from signal cell 
                            }
                        }

                    }

                }

                Gateway.GetForcedSignals(i_inputForcedSignals, i_outputForcedSignals);
                mut1.ReleaseMutex();

                Thread.Sleep(samplingTime);
               
            }   
            
        }
        
        public void ResetSignalCellCheckBox()
        {
            for (int i = 0; i < inputSignalCell.Length; i++)
            {
                if (inputSignalCell[i] != null)
                {
                    inputSignalCell[i].ResetCheckBox();
                }
                
            }

            for (int i = 0; i < outputSignalCell.Length; i++)
            {
                if (outputSignalCell[i] != null)
                {
                    outputSignalCell[i].ResetCheckBox();
                }                
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

        }

        private void trendViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }

        private void oneTrendViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numberOfSelectedSignalOneTrendView < maxNumberOfOneBigTrendView)
            {
                (inputCheckedItemList, outputCheckedItemList) = GetCheckedListViewItems(listView_inputSignal, listView_outputSignal);

                if (inputCheckedItemList.Count == 0 & outputCheckedItemList.Count == 0)
                {
                    MessageBox.Show("Form1: Please select signals !, You selected " + (inputCheckedItemList.Count + outputCheckedItemList.Count).ToString() + " signals !");
                }
                else
                {
                    // add checked signals from a new selected singal view                 

                    oneTrendForm = new OneTrendForm(selectedSignalOneTrendViewOrder, inputCheckedItemList, outputCheckedItemList);
                    
                    oneTrendForm.Name = "SelectedSignalView" + selectedSignalViewOrder.ToString();
                    oneTrendForm.AutoSize = true;
                    oneTrendForm.AutoScroll = true;
                    oneTrendForm.Text = "Selected Signals Trend View " + selectedSignalViewOrder.ToString();
                    oneTrendForm.FormClosed += new FormClosedEventHandler(oneTrendForm_SelectedSignalViewClosedCallback);
                    selectedSignalOneTrendViewThread = new Thread(() => CreateSelectedSignalOneTrendView(oneTrendForm, inputCheckedItemList, outputCheckedItemList));
                    selectedSignalOneTrendViewThread.Start();
                    numberOfSelectedSignalOneTrendView++;
                    selectedSignalOneTrendViewOrder++;
                }
            }
            else
            {
                MessageBox.Show("The max number of selected signal view is reached, close one or more selected signal views opened and try agian !");
            }

            ResetCheckbox(listView_inputSignal);
            ResetCheckbox(listView_outputSignal);
            ResetCheckbox(listView_SystemSignal);     
                  
        }

        public void CreateSelectedSignalOneTrendView(OneTrendForm oneTrendForm, List<ListViewItem> i_inputCheckedItemList, List<ListViewItem> i_outputCheckedItemList)
        { 
            int trendOrder = 1;
            //Panel p = new Panel();

            MultiSeries oneTrendView = new MultiSeries(trendOrder, i_inputCheckedItemList, i_outputCheckedItemList);
            oneTrendForm.Controls.Add(oneTrendView.CreateBigTrendViewPanel());

            Application.Run(oneTrendForm);
        }


        void oneTrendForm_SelectedSignalViewClosedCallback(object sender, FormClosedEventArgs e)
        {
            //ResetSignalCellCheckBox();
            Gateway.ResetForcedSignals();
            numberOfSelectedSignalOneTrendView--;
            inputCheckedItemList.Clear();
            outputCheckedItemList.Clear();
            inputForcedSignals.Clear();
            outputForcedSignals.Clear();
            selectedSignalOneTrendViewOrder = 1; // reset it if only allow have one view 
            oneTrendForm.Dispose();
            //signalPanel.Dispose();           
        }
       

        //public Series UpdataSeries(OneSeries s)
        //{
        //    Series series;  
            
        //    mut3.WaitOne();
        //    series = s.GetOneSeries();
        //    mut3.ReleaseMutex();
        //    return series;
        //}
               
        private Panel GetTrendViewPanel(TrendViewCell itemCell, ListViewItem item)      
        {
            Panel panel;     
            //mut3.WaitOne();
            panel = itemCell.CreateOneTrendViewPanel(item);   
            //mut3.ReleaseMutex();
            return panel;
        }

        public void CreateSelectedSignalTrendView(TrendForm trendForm, List<ListViewItem> i_inputCheckedItemList, List<ListViewItem> i_outputCheckedItemList)
        {
            int trendOrder = 1;
            int inputTrendCellNumber = 0;
            int outputTrendCellNumber = 0;

            inputSignalTrendViewCell = new TrendViewCell[i_inputCheckedItemList.Count];
            outputSignalTrendViewCell = new TrendViewCell[i_outputCheckedItemList.Count];
            trendPanelThread = new Thread[i_inputCheckedItemList.Count + i_outputCheckedItemList.Count];

            if (i_inputCheckedItemList.Count > 0)
            {
                foreach (ListViewItem lvi in i_inputCheckedItemList.ToList())
                {
                    inputSignalTrendViewCell[inputTrendCellNumber] = new TrendViewCell(trendOrder, lvi);    
                    trendPanelThread[trendOrder - 1] = new Thread(() => { trendPanel = GetTrendViewPanel(inputSignalTrendViewCell[inputTrendCellNumber], lvi); }) ;
                    trendPanelThread[trendOrder - 1].Start();
                    trendPanelThread[trendOrder - 1].Join();
                    int trendPanel_x = 50; int trendPanel_y = (trendOrder - 1) * trendPanel.Height;  
                    trendPanel.Location = new System.Drawing.Point(trendPanel_x, trendPanel_y);   
                    trendForm.Controls.Add(trendPanel);
                    trendOrder++;
                    inputTrendCellNumber++;                   
                }

            }
            
            if (i_outputCheckedItemList.Count > 0)
            {
                foreach (ListViewItem lvi in i_outputCheckedItemList.ToList())
                {
                    outputSignalTrendViewCell[outputTrendCellNumber] = new TrendViewCell(trendOrder, lvi); 
                    trendPanelThread[trendOrder-1] = new Thread(() => { trendPanel = GetTrendViewPanel(outputSignalTrendViewCell[outputTrendCellNumber], lvi); });
                    trendPanelThread[trendOrder-1].Start();
                    trendPanelThread[trendOrder - 1].Join();
                    int trendPanel_x = 50; int trendPanel_y = (trendOrder - 1) * trendPanel.Height;                
                    trendPanel.Location = new System.Drawing.Point(trendPanel_x, trendPanel_y);                   

                    trendForm.Controls.Add(trendPanel);
                    trendOrder++;                   

                    outputTrendCellNumber++;                    
                }
            }       

            Application.Run(trendForm);
        }       


        void trendForm_SelectedSignalViewClosed(object sender, FormClosedEventArgs e)
        {
            ResetSignalTrendFormCellCheckBox(); // closing trend view will not reset this
            // Gateway.ResetForcedSignals(); // closing trend view will not reset this
            numberOfSelectedSignalTrendView--;
            inputCheckedItemList.Clear(); 
            outputCheckedItemList.Clear(); 
            inputForcedSignals.Clear(); // closing trend view will not reset this
             outputForcedSignals.Clear(); // closing trend view will not reset this
            selectedSignalTrendViewOrder = 1; // reset it if only allow have one view 
            trendForm.Dispose();
            trendPanel.Dispose();  

            for (int i = 0; i < trendPanelThread.Length; i++)
            {
                trendPanelThread[i].Abort();
            }   
        }

        private void ResetSignalTrendFormCellCheckBox()
        {

            for (int i = 0; i < inputSignalTrendViewCell.Length; i++)
            {
                if (inputSignalTrendViewCell[i] != null)
                {
                   inputSignalTrendViewCell[i] = null;
                }
            }

            for (int i = 0; i < outputSignalTrendViewCell.Length; i++)
            {
                if (outputSignalTrendViewCell[i] != null)
                {
                    outputSignalTrendViewCell[i] = null;
                }
            }
        }

        private void multipleTrendViewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numberOfSelectedSignalTrendView < maxNumberOfSelectedSignalTrendView)
            {
                (inputCheckedItemList, outputCheckedItemList) = GetCheckedListViewItems(listView_inputSignal, listView_outputSignal);

                if (inputCheckedItemList.Count == 0 & outputCheckedItemList.Count == 0)
                {
                    MessageBox.Show("Form1: Please select signals !, You selected " + (inputCheckedItemList.Count + outputCheckedItemList.Count).ToString() + " signals !");
                }
                else
                {
                    // add checked signals from a new selected singal view     
                    trendForm = new TrendForm(selectedSignalTrendViewOrder);
                    trendForm.Name = "SelectedSignalTrendView" + selectedSignalTrendViewOrder.ToString();
                    trendForm.Text = "Selected Signals Trend View " + selectedSignalTrendViewOrder.ToString();
                    trendForm.AutoSize = true;
                    trendForm.AutoScroll = true;
                    trendForm.FormClosed += new FormClosedEventHandler(trendForm_SelectedSignalViewClosed);
                    selectedSignalTrendViewThread = new Thread(() => CreateSelectedSignalTrendView(trendForm, inputCheckedItemList, outputCheckedItemList));
              
                    selectedSignalTrendViewThread.Start();
                    numberOfSelectedSignalTrendView++;
                    selectedSignalTrendViewOrder++;                  

                }
            }
            else
            {
                MessageBox.Show("The max number of selected signal view is reached, close one or more selected signal views opened and try agian !");
            }
            ResetCheckbox(listView_inputSignal);
            ResetCheckbox(listView_outputSignal);
            ResetCheckbox(listView_SystemSignal);          

        }
       
    }
}
