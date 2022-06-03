using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public string chartStr;
        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool xAutoScale, yAutoScale;
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            if (form1 != null) // Если форма существует, то изменяем свойства её объектов
            {
                if (checkBox1.Checked)
                {
                    xAutoScale = true;
                }
                else
                {
                    xAutoScale = false;
                    form1.Xmin = double.Parse(textBox1.Text.Replace(".", ","));
                    form1.Xmax = double.Parse(textBox2.Text.Replace(".", ","));
                }

                if (checkBox2.Checked)
                {
                    yAutoScale = true;
                }
                else
                {
                    yAutoScale = false;
                    form1.Ymin = double.Parse(textBox3.Text.Replace(".", ","));
                    form1.Ymax = double.Parse(textBox4.Text.Replace(".", ","));
                }
                form1.rescaleCharts(xAutoScale, yAutoScale, chartStr);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            form1.Enabled = true;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
            else
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }
        }
    }
}
