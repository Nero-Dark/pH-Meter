using System;
using System.Windows.Forms;

using System.IO.Ports;
using System.Threading;



namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        public double Xmin, Xmax, Ymin, Ymax;
        public int interval;
        public SerialPort COM_port;
        public byte[] commands = {0x02, 0x72};
        DateTime startTime = new DateTime();
        static string fileName; 

        public Form1()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();
            foreach (var item in ports)
            {
                COMComboBox.Items.Add(item);
                backgroundWorker1.WorkerSupportsCancellation = true;
            }

        }

        public void rescaleCharts(bool xAutoScale, bool yAutoScale, string chartStr)
        {
            if (chartStr == "chart1")
            {
                if (!xAutoScale)
                {
                    chart1.ChartAreas[0].AxisX.Minimum = Xmin;
                    chart1.ChartAreas[0].AxisX.Maximum = Xmax;
                }
                else
                {
                    chart1.ChartAreas[0].AxisX.Minimum = 0;
                    chart1.ChartAreas[0].AxisX.Maximum = Double.NaN;
                }
                if (!yAutoScale)
                {
                    chart1.ChartAreas[0].AxisY.Minimum = Ymin;
                    chart1.ChartAreas[0].AxisY.Maximum = Ymax;
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Minimum = Double.NaN;
                    chart1.ChartAreas[0].AxisY.Maximum = Double.NaN;
                }
            }
            if (chartStr == "chart2")
            {
                if (!xAutoScale)
                {
                    chart2.ChartAreas[0].AxisX.Minimum = Xmin;
                    chart2.ChartAreas[0].AxisX.Maximum = Xmax;
                }
                else
                {
                    chart2.ChartAreas[0].AxisX.Minimum = 0;
                    chart2.ChartAreas[0].AxisX.Maximum = Double.NaN;
                }
                if (!yAutoScale)
                {
                    chart2.ChartAreas[0].AxisY.Minimum = Ymin;
                    chart2.ChartAreas[0].AxisY.Maximum = Ymax;
                }
                else
                {
                    chart2.ChartAreas[0].AxisY.Minimum = Double.NaN;
                    chart2.ChartAreas[0].AxisY.Maximum = Double.NaN;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Open"){
                try
                {

                    COM_port = new SerialPort();
                    // настройки порта
                    COM_port.PortName = COMComboBox.SelectedItem.ToString();
                    COM_port.BaudRate = Int16.Parse(baudRateComboBox.SelectedItem.ToString());
                    COM_port.DataBits = 8;
                    COM_port.StopBits = System.IO.Ports.StopBits.One;
                    COM_port.Parity = System.IO.Ports.Parity.None;
                    COM_port.ReadTimeout = 500;
                    COM_port.WriteTimeout = 500;
                    COM_port.Open();

                    button2.Enabled = true;
                    button1.Text = "Close";
                }
                catch (Exception exep)
                {

                    MessageBox.Show(
                        "ERROR: невозможно открыть порт:" + exep.Message,
                        "Ошибка открытия COM-порта",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                backgroundWorker1.CancelAsync();
                COM_port.Close();
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
                button1.Text = "Open";
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            if (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length-4,4)!=".txt") {
                fileName = saveFileDialog1.FileName + ".txt";
            }
            else
            {
                fileName = saveFileDialog1.FileName;
            }
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            dataGridView1.RowCount = 0;
            interval  = Int16.Parse(textBox3.Text)*1000;

            backgroundWorker1.RunWorkerAsync();
            startTime = DateTime.Now;
            button2.Enabled = false;
            button3.Enabled = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            double frmHeight = this.ClientSize.Height;
            chart1.Height = (int) Math.Round(frmHeight / 2) - 7;
            chart2.Top = (int)Math.Round(frmHeight / 2) + 7;
            chart2.Height = (int)Math.Round(frmHeight / 2) - 7;
            textBox2.Top = chart2.Top;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {   
            char[] tempCharArr = { 'f', 'f', 'f', 'f', 'f', 'f' };
            char[] pHCharArr = { 'f', 'f', 'f', 'f', 'f' };
            double temp;
            double pH;
            char separator = '\u0009';
            DateTime tickTimeS = new DateTime();
            DateTime tickTimeE = new DateTime();

            while (!backgroundWorker1.CancellationPending) {
                tickTimeS = DateTime.Now;
                TimeSpan tickValue = tickTimeS - startTime;

                COM_port.Write(commands, 0, 1);
                Thread.Sleep(200);
                for (int index = 0; index <= 5; index++)
                {
                    try
                    {
                        tempCharArr[index] = (char)COM_port.ReadChar();
                    }
                    catch (Exception exep)
                    {
                        break;
                    }
                }
                string tempStr = new string(tempCharArr);
                
                tempStr = tempStr.Substring(1);
                tempStr = tempStr.Replace(".", ",");
                tempStr = tempStr.Replace("f", "");
                temp = Convert.ToDouble(tempStr);

                COM_port.Write(commands, 1, 1);
                //Thread.Sleep(200);
                for (int index = 0; index <= 4; index++)
                {
                    pHCharArr[index] = (char)COM_port.ReadChar();
                }
                string pHStr = new string(pHCharArr);
                
                pHStr = pHStr.Replace(".", ",");
                pHStr = pHStr.Replace("f", "");
                pH = Convert.ToDouble(pHStr);

                Action action = () => textBox1.Text = tempStr;
                Invoke(action);
                action = () => textBox2.Text = pHStr;
                Invoke(action);
                action = () => chart1.Series[0].Points.AddXY(Math.Round(tickValue.TotalSeconds), temp);
                Invoke(action);
                action = () => chart2.Series[0].Points.AddXY(Math.Round(tickValue.TotalSeconds), pH);
                Invoke(action);

                action = () => dataGridView1.Rows.Add(Math.Round(tickValue.TotalSeconds), tempStr, pHStr);
                Invoke(action);
                if (checkBox1.Checked)
                {
                    action = () => dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                    Invoke(action);
                }



                System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true);
                writer.WriteLine($"{Math.Round(tickValue.TotalSeconds)}{separator}{tempStr}{separator}{pHStr}");
                writer.Close();
                //Thread.Sleep(interval - 300);
                tickTimeE = DateTime.Now;
                tickValue = tickTimeE - tickTimeS;
                while (tickValue.TotalMilliseconds <= interval)
                {
                    Thread.Sleep(10);
                    tickTimeE = DateTime.Now;
                    tickValue = tickTimeE - tickTimeS;
                }
                
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            checkBox1.Checked = false;
        }

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.chartStr = "chart1";
            form2.TopMost = true;
            this.Enabled = false;
            form2.Show();
        }

        private void chart2_DoubleClick(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.chartStr = "chart2";
            form2.TopMost = true;
            this.Enabled = false;
            form2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            button2.Enabled = true;
            button3.Enabled = false;
        }

    }
}
   
