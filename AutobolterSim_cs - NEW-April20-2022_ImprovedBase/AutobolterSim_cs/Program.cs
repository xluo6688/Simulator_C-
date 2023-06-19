/// Komatsu Mining Autobolter Simulator 
/// Xuanwen Luo
/// 10/2020


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace AutobolterSim_cs
{
    static class Program
    {
        static Form1 SignalPoolView;
        static int viewRefreshTime = 100;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Gateway.ReadInputSignals();
            Gateway.ReadOutputSignals();
            Gateway.ReadSystemSignalsFromController();
            Thread.Sleep(100); // To read signals earlier

            SignalPoolView = new Form1();
            SignalPoolView.AutoScroll = true;

            Thread inputSignal_thread = new Thread(Simulation);      
            inputSignal_thread.Start();

            Application.Run(SignalPoolView);
        }

        static void Simulation()
        {
            while (true)
            {
                SignalPoolView.UpdateSignalPoolView();
                Thread.Sleep(viewRefreshTime);
            }
        }

    }
}
