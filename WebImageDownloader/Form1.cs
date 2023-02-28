using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebImageDownloader
{
    public partial class Form1 : Form
    {
        WebImageDownloader loader = new WebImageDownloader();

        public Form1()
        {
            InitializeComponent();

            loader.SetPath("D:\\develop\\images");
            loader.OnDownloadProgress(UpdateProgressBar);
        }

        void UpdateProgressBar (int progress)
        {
            Invoke(new Action(() => progressBar1.Value = progress));
        } 

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            await loader.Download("https://vudgu.ru");
            await Task.Delay(500);
            button1.Visible = true;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
