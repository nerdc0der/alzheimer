/*
 * nerdcoder.com 2014
 * Aplicación de prueba para función de liberación de memoria alzheimer
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace AlzheimerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region nerdcoder.com: declaraciones
        class arg
        {
            public int serie;
            public Point punto;
            public arg(int serie,Point punto)
            {
                this.serie = serie;
                this.punto = punto;
            }
        }
        int iteraciones;
        int percent = 0; 
        int serieJ = 0;
        int x = 0;

        /// <summary>
        /// Consulta, calcula e informa el consumo de RAM de la actual aplicación
        /// </summary>
        public void RAM()
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            long ramuse = ((proc.WorkingSet64 / 1024) / 1024);
            lblRam.Text = ramuse.ToString() + "Mb";
            chart2.Series[0].Points.Add(ramuse);
            chart2.Update();
        }

        #endregion 

        #region nerdcoder.com: alzheimer xD
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        public static void alzheimer()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
        #endregion

        #region nerdcoder.com: BackGroundWorker
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!backgroundWorker1.CancellationPending)
            {
                try
                {
                    for (int j = 1; j <= iteraciones; j++)
                    {
                        int percentage = ((j * 100) / iteraciones);
                        for (int i = 1; i < 100; i++)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int x = 0; x < 1000000; x++)
                                sb.AppendLine("*********************");
                            /* reporte de progreso */
                            backgroundWorker1.ReportProgress(percentage, new arg(j, new Point(j, i)));
                        }
                    }
                }
                catch { }
            }
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!backgroundWorker1.CancellationPending)
            {
                arg p = (arg)e.UserState;

                if (p.serie != serieJ)
                {
                    serieJ = p.serie;
                    chart1.Series.Add(serieJ.ToString());
                    chart1.Series[serieJ.ToString()].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                }

                chart1.Series[serieJ.ToString()].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(p.punto.X + (x++), p.punto.Y));
                chart1.Update();
                this.Text = "nerdcoder.com - test (" + p.punto.X + 1 + "," + p.punto.Y + ")";
                if (percent < e.ProgressPercentage && e.ProgressPercentage < 101)
                {
                    progressBar1.Value = e.ProgressPercentage;
                    percent = e.ProgressPercentage;
                }
            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker1.CancellationPending)
            {
                progressBar1.Value = 100;
                this.Text = "Done!";
            }
        }
        #endregion

        #region nerdcoder.com: ControlHandlers
        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            RAM();
            progressBar1.Value = 0;
            iteraciones = trackBar1.Value;
            chart1.Series.Clear();
            backgroundWorker1.RunWorkerAsync();
        }
        private void btnFreeRAM_Click(object sender, EventArgs e)
        {
            alzheimer();
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lblIteraciones.Text = trackBar1.Value.ToString();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            RAM();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lblIteraciones.Text = trackBar1.Value.ToString();
        }
        #endregion
    }
}
