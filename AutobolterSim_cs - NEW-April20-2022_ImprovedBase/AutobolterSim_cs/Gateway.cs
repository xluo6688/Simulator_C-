/// Komatsu Mining Autobolter Simulator 
/// Xuanwen Luo
/// 10/2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;


namespace AutobolterSim_cs
{
    public static class Gateway
    {
       
        private static readonly Random _random = new Random();

        static List<SignalType.Signals> inputSignalFromController;
        static List<SignalType.Signals> outputSignalFromController;
        static List<SignalType.Signals> systemSignalFromController;

        static List<SignalType.Signals> inputWithForcedSignalList = new List<SignalType.Signals>();
        static List<SignalType.Signals> outputWithForcedSignalList = new List<SignalType.Signals>();

        static List<SignalType.Signals> inputWithForcedSignalList_temp = new List<SignalType.Signals>();
        static List<SignalType.Signals> outputWithForcedSignalList_temp = new List<SignalType.Signals>();

        static List<SignalType.Signals> forcedInputSignalList = new List<SignalType.Signals>();
        static List<SignalType.Signals> forcedOutputSignalList = new List<SignalType.Signals>();
        static int i = 0;
        static int samplingTime = 3000;
        static int minRandomNum = 600;
        static int maxRandomNum = 60000;
        static int minRandomEncoderDegreeNum = 0;
        static int maxRandomEncoderDegreeNum = 360;
        static int minRandomInclinometerDegreeNum = 0;
        static int maxRandomInclinometerDegreeNum = 90;
        static int minRandomFlowRateNum = 0;
        static int maxRandomFlowRateNum = 600;
        static int minSolenoidCurrent = 0;
        static int maxSolenoidCurrent = 150;
        static object _locker = new object();

        private static Mutex mut_a1 = new Mutex();
        private static Mutex mut_a2 = new Mutex();
        private static Mutex mut_a3 = new Mutex();
        private static Mutex mut_a4 = new Mutex();
        private static Mutex mut_a5 = new Mutex();
        static public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static List<SignalType.Signals> GetUpdatedInputFromGateway()
        {
            return inputWithForcedSignalList;
        }

        public static List<SignalType.Signals> GetUpdatedOutputFromGateway()
        {
            return outputWithForcedSignalList;
        }       



        private static List<SignalType.Signals> RemoveSignal(List<SignalType.Signals> signals, SignalType.Signals signal)
        //private List<SignalType.Signals> RemoveSignal(List<SignalType.Signals> signals, SignalType.Signals signal)
        {
            mut_a4.WaitOne();
                if (HasIt(signals, signal))
                {
                    signals.Remove(signals.Single(sg => sg.Name.Equals(signal.Name)));
                }
            mut_a4.ReleaseMutex();

            return signals;
        }

        public static bool HasIt(List<SignalType.Signals> SignalList, SignalType.Signals signal)
        {
            foreach (SignalType.Signals sg in SignalList.ToList())
            {
                if (sg.Name == signal.Name)
                {
                    return true;
                }

            }

            return false;
        }

       
        public static void GetForcedSignals(List<SignalType.Signals> inputSignals, List<SignalType.Signals> outputSignals)
        {
            mut_a1.WaitOne();
            //lock (forcedInputSignalList)
           // {
                forcedInputSignalList = inputSignals;
            //}

           // lock (forcedOutputSignalList)
           // {
                forcedOutputSignalList = outputSignals;
           // }

            mut_a1.WaitOne();
        }

        public static void ResetForcedSignals()
        {
            SignalType.Signals signal;
            mut_a5.WaitOne();
           // lock (forcedInputSignalList)
           // {
                foreach (SignalType.Signals sg in forcedInputSignalList.ToList())
                {
                    signal = sg;
                    signal.Forced = false;

                    forcedInputSignalList = RemoveSignal(forcedInputSignalList, sg);
                    forcedInputSignalList.Add(signal);
                }
           // }

           // lock (forcedOutputSignalList)
          //  {
                foreach (SignalType.Signals sg in forcedOutputSignalList.ToList())
                {
                    signal = sg;
                    signal.Forced = false;

                    forcedOutputSignalList = RemoveSignal(forcedOutputSignalList, sg);
                    forcedOutputSignalList.Add(signal);
                }
           // }
            mut_a5.ReleaseMutex();



        }


        static public List<SignalType.Signals> ReadInputSignals()
        {
            mut_a2.WaitOne();
            inputSignalFromController = ReadInputSignalsFromController();

            if (forcedInputSignalList.Count > 0) // signals from selected signal view
            {
                inputWithForcedSignalList = inputSignalFromController;

                //lock (forcedInputSignalList)
                //{
                    foreach (SignalType.Signals sg1 in forcedInputSignalList.ToList()) // signals from selected signal view
                    {
                        //lock (inputWithForcedSignalList)
                       // {
                            foreach (SignalType.Signals sg2 in inputWithForcedSignalList.ToList())
                            {
                                // if (sg1.Name.Equals(sg2.Name) & sg1.Forced)

                                if (sg1.Name == sg2.Name & sg1.Forced)
                                {
                                    lock (inputWithForcedSignalList)
                                    {
                                        
                                        Console.Write(" Before remove: inputWithForcedSignalList.Count = " + inputWithForcedSignalList.Count.ToString());
                                        inputWithForcedSignalList = RemoveSignal(inputWithForcedSignalList, sg2);
                                        Console.Write("; After remove: inputWithForcedSignalList.Count = " + inputWithForcedSignalList.Count.ToString());
                                        inputWithForcedSignalList.Add(sg1);
                                       
                                    }
                                    
                                    //break;
                                }
                            }
                        //}
                        
                    }
               // }
                  
            }
            else
            {
                //lock (inputWithForcedSignalList)
                //{
                    
                    inputWithForcedSignalList = inputSignalFromController;
                    
               // }               
            
            }
            mut_a2.ReleaseMutex();

            return inputWithForcedSignalList;

        }

        static public List<SignalType.Signals> ReadOutputSignals()
        {
            mut_a3.WaitOne();
            outputSignalFromController = ReadOutputSignalsFromController();
            List<SignalType.Signals> mySignal = new List<SignalType.Signals>();

           
            if (forcedOutputSignalList.Count > 0)
            {
                outputWithForcedSignalList = outputSignalFromController;

               // lock(forcedOutputSignalList)
               // {
                    foreach (SignalType.Signals sg1 in forcedOutputSignalList.ToList())
                    {
                        //lock (outputWithForcedSignalList)
                        //{
                            foreach (SignalType.Signals sg2 in outputWithForcedSignalList.ToList())
                            {
                                if (sg1.Name == sg2.Name & sg1.Forced == true)
                                {
                                    lock (outputWithForcedSignalList)
                                    {
                                       
                                        outputWithForcedSignalList = RemoveSignal(outputWithForcedSignalList, sg2);
                                        outputWithForcedSignalList.Add(sg1);
                                       
                                    }


                                    //break;
                                }
                            }
                        //}
                       
                    }
               // }
                      
            }
            else
            {
                //lock (outputWithForcedSignalList)
               // {
                    mut_a3.WaitOne();
                    outputWithForcedSignalList = outputSignalFromController;
                    mut_a3.ReleaseMutex();
               // }               
            }

            mut_a3.ReleaseMutex();
            return outputWithForcedSignalList;

        }

        static public List<SignalType.Signals> ReadInputSignalsFromController()
        {
            // TO DO: replace this code with RS20s read();
            // MessageBox.Show("Start Read RS2os Input Signals");
            List<SignalType.Signals> bf = new List<SignalType.Signals>();

            while (true)
            {
                SignalType.Signals signal = new SignalType.Signals
                {
                    Name = "feedPressureTransducer_PT3 ",
                    Value = RandomNumber(minRandomNum, maxRandomNum),
                    Unit = "psi",
                    Min = 0,
                    Max = 80000,
                    RawData = RandomNumber(minRandomNum, maxRandomNum),
                    SourceOrDestination = "Port5"
                };
                //Console.WriteLine("value = " + i.ToString());
                bf.Add(signal);

                signal.Name = "rotationPressureTransducer_PT4 ";
                signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                signal.Unit = "psi";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                signal.SourceOrDestination = "Port5";
                bf.Add(signal);

                signal.Name = "lowPressureWaterPressureTransducer_PTW1 ";
                signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                signal.Unit = "psi";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                signal.SourceOrDestination = "Port5";
                bf.Add(signal);

                signal.Name = "highPressureWaterPressureTransducer_PTW2 ";
                signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                signal.Unit = "psi";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "timberJackPressureTransducer_PT2 ";
                signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                signal.Unit = "psi";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "secondaryFunctionPressureTransducer_PT6 ";
                signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                signal.Unit = "psi";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                signal.SourceOrDestination = "Port5";
                bf.Add(signal);

                signal.Name = "AUXFunctionPressureTransducer_PT1 ";
                signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                signal.Unit = "psi";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "feedHydraulicSystemOilFlow  ";
                signal.Value = RandomNumber(minRandomFlowRateNum, maxRandomFlowRateNum);
                signal.Unit = "pulse/L";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomFlowRateNum, maxRandomFlowRateNum);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "waterFlow  ";
                signal.Value = RandomNumber(minRandomFlowRateNum, maxRandomFlowRateNum);
                signal.Unit = "pulse/L";
                signal.Min = 0;
                signal.Max = 80000;
                signal.RawData = RandomNumber(minRandomFlowRateNum, maxRandomFlowRateNum);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "feedHomeSwitch ";
                signal.Value = RandomNumber(0, 2) % 2;
                signal.Unit = "On/Off";
                signal.Min = 0;
                signal.Max = 1;
                signal.RawData = RandomNumber(0, 2) % 2;
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "loaderArmPositionSwitch ";
                signal.Value = RandomNumber(0, 2) % 2;
                signal.Unit = "On/Off";
                signal.Min = 0;
                signal.Max = 1;
                signal.RawData = RandomNumber(0, 2) % 2;
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "gripperProximitySwitch ";
                signal.Value = RandomNumber(0, 2) % 2;
                signal.Unit = "On/Off";
                signal.Min = 0;
                signal.Max = 1;
                signal.RawData = RandomNumber(0, 2) % 2;
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "carouselPositionEncoder  ";
                signal.Value = RandomNumber(minRandomEncoderDegreeNum, maxRandomEncoderDegreeNum);
                signal.Unit = "degree";
                signal.Min = 0;
                signal.Max = 360;
                signal.RawData = RandomNumber(minRandomEncoderDegreeNum, maxRandomEncoderDegreeNum);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "bolterIndexingInclinometer_NS   ";
                signal.Value = RandomNumber(minRandomInclinometerDegreeNum, maxRandomInclinometerDegreeNum);
                signal.Unit = "degree";
                signal.Min = 0;
                signal.Max = 90;
                signal.RawData = RandomNumber(minRandomInclinometerDegreeNum, maxRandomInclinometerDegreeNum);
                signal.SourceOrDestination = "Port8";
                bf.Add(signal);

                signal.Name = "bolterIndexingInclinometer_EW   ";
                signal.Value = RandomNumber(minRandomInclinometerDegreeNum, maxRandomInclinometerDegreeNum);
                signal.Unit = "degree";
                signal.Min = 0;
                signal.Max = 90;
                signal.RawData = RandomNumber(minRandomInclinometerDegreeNum, maxRandomInclinometerDegreeNum);
                signal.SourceOrDestination = "Port8";
                bf.Add(signal);



                //for (int i = 1; i < 60; i++)
                //{
                //    signal.Name = "Rotation Pressure Sensor " + i.ToString();
                //    signal.Value = RandomNumber(minRandomNum, maxRandomNum);
                //    signal.Unit = "psi";
                //    signal.Min = 0;
                //    signal.Max = 80000;
                //    signal.RawData = RandomNumber(minRandomNum, maxRandomNum);
                //    signal.SourceOrDestination = "Port6";

                //    bf.Add(signal);
                //}

                // Console.WriteLine(" Reading inputs ... ");



                Thread.Sleep(RandomNumber(1000, 1000));
                return bf;
            }

           
            
        }

        static public List<SignalType.Signals> ReadOutputSignalsFromController()
        {
            // TO DO: replace this code with RS20s read();
            // MessageBox.Show("Start Read RS2os Input Signals");
            List<SignalType.Signals> bf = new List<SignalType.Signals>();

            while (true)
            {
                SignalType.Signals signal = new SignalType.Signals
                {
                    Name = "feedExtendSolenoidCurrent ",
                    Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent),
                    Unit = "mA",
                    Min = minSolenoidCurrent,
                    Max = maxSolenoidCurrent,
                    RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent),
                    SourceOrDestination = "Port5"
                };
                bf.Add(signal);

                signal.Name = "feedRetractSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port5";
                bf.Add(signal);

                signal.Name = "feedPressureRegulatorCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port5";
                bf.Add(signal);

                signal.Name = "rotationCWSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "rotationCCWSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port6";
                bf.Add(signal);

                signal.Name = "WaterOnOffSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "timberJackExtendSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "timberJackRetrsctSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "bolterIndexWestSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "bolterIndexEastSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "bolterIndexNorthSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "bolterIndexSouthSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);


                signal.Name = "topJawLoaderArmOpenSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "topJawLoaderArmCloseSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "midJawLoaderArmOpenSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "midJawLoaderArmCloseSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "kickDownFeedHighSpeedSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);

                signal.Name = "topJawRelaxHighPressureWaterSolenoidCurrent ";
                signal.Value = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.Unit = "mA";
                signal.Min = minSolenoidCurrent;
                signal.Max = maxSolenoidCurrent;
                signal.RawData = RandomNumber(minSolenoidCurrent, maxSolenoidCurrent);
                signal.SourceOrDestination = "Port7";
                bf.Add(signal);
                
                Thread.Sleep(RandomNumber(1000, 1000));
                return bf;
            }
        }

            

        static public List<SignalType.Signals> ReadSystemSignalsFromController()
        {
            // TO DO: replace this code with RS20s read();
            // MessageBox.Show("Start Read RS2os Input Signals");
            List<SignalType.Signals> bf = new List<SignalType.Signals>();

            while (true)
            {
                SignalType.Signals signal = new SignalType.Signals
                {
                    Name = "systemBusLoad ",
                    Value = RandomNumber(0, 100),
                    Unit = "%",
                    Min = 0,
                    Max = 100,
                    RawData = RandomNumber(1, 100),
                    SourceOrDestination = "Port1"
                };
                bf.Add(signal);

                                                                         
                signal.Name = "hostMachineMaintenanceMode ";
                signal.Value = RandomNumber(0, 2) % 2;
                signal.Unit = "T/F";
                signal.Min = 0;
                signal.Max = 1;
                signal.RawData = RandomNumber(0, 1);
                signal.SourceOrDestination = "CCU";
                bf.Add(signal);

                signal.Name = "hostMachineBolterMode ";
                signal.Value = RandomNumber(0, 2) % 2;
                signal.Unit = "T/F";
                signal.Min = 0;
                signal.Max = 1;
                signal.RawData = RandomNumber(0, 1);
                signal.SourceOrDestination = "CCU";
                bf.Add(signal);

                signal.Name = "BolterPumpState ";
                signal.Value = RandomNumber(0, 2) % 2;
                signal.Unit = "T/F";
                signal.Min = 0;
                signal.Max = 1;
                signal.RawData = RandomNumber(0, 1);
                signal.SourceOrDestination = "CCU";
                bf.Add(signal);

                Thread.Sleep(RandomNumber(1000, 1000));
                return bf;
            }            
        }
    }
}
