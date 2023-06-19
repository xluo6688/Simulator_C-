using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

namespace AutobolterSim_cs
{
    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enabled)
        {
            var method = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enabled });

            //var prop = control.GetType().GetProperty("DoubleBuffering", BindingFlags.Instance | BindingFlags.NonPublic);
            //prop.SetValue(control, enabled, null);
        }
    }
}
