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

namespace ConsoleApp2
{
    public partial class Form2 : Form
    {
        string name, msg;
        int i = 0;
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string name, string msg)
        {
            InitializeComponent();
            this.name = name;
            this.msg = msg;
            label1.Text = name + msg;
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
       
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            i++;
            if(i>8)
            {
                timer1.Enabled = false;
                Close();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }
    }
}
