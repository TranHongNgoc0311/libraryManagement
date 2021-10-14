using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0
{
    public partial class Waiting : Form
    {
        int value = 0;
        public Waiting()
        {
            InitializeComponent();
            panel1.BackColor = Color.FromArgb(100,Color.WhiteSmoke);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            value += 5;
            if(value > 100)
            {
                this.Hide();
                LayoutForm form = new LayoutForm();
                form.Show();
                timer1.Enabled = false;
            }
            else
            {
                PbWaitingForm.Value = value;
            }
        }
    }
}
