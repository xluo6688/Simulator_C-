using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutobolterSim_cs
{
    public partial class Form2 : Form
    {
        public int selectedSignalViewOrder;
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(int order)
        {
            InitializeComponent();
            selectedSignalViewOrder = order;
            //MessageBox.Show("From Form2: total form2 opened = " + Application.OpenForms.Count.ToString());
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
