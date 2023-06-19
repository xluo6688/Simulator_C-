/// Komatsu Mining Autobolter Simulator 
/// Xuanwen Luo
/// 10/2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutobolterSim_cs
{
    public class SignalType
    {
        public struct Signals
        {
            //public int No;
            public string Name;
            public double Value;
            public string Unit;
            public double Min;
            public double Max;
            public double RawData;
            public string SourceOrDestination;
            public bool Forced;
        }

        public enum ItemIndex
        {
            checkBox = 0,
            Name,
            Value,
            Unit,
            Min,
            Max,
            RawData,
            SourceOrDestination
        }

    }
}
