using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Common;
using System.IO;
using ZXing.QrCode;
using DevComponents.DotNetBar;
 
namespace QinWinForm
{
    public partial class  Form3 : Office2007Form
    {
        public Form3()
        {
            InitializeComponent(); 
        }
 
       private BarCodeClass bcc = new BarCodeClass();
       private DocementBase _docement;
 

        private void Form1_Load(object sender,EventArgs e)
        {
            txtMsg.Text = System.DateTime.Now.ToString("yyyyMMddhhmmss").Substring(0, 12);
        }


        private void buttonX2_Click(object sender, EventArgs e)
        {
            bcc.CreateQuickMark(pictureBox1, txtMsg.Text);
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("You Must Load an Image first!");
                return;
            }
            else
            {
                _docement = new imageDocument(pictureBox1.Image);
            }
            _docement.showPrintPreviewDialog();
        }

        private void buttonX4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("请录入图像后再进行解码!");
                return;
            }
            BarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode((Bitmap)pictureBox1.Image);
            MessageBox.Show(result.Text);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            txtMsg.Text = System.DateTime.Now.ToString("yyyyMMddhhmmss").Substring(0, 12);
        }

        /// <summary>
        /// 二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            bcc.CreateBarCode(pictureBox1, txtMsg.Text);
        }

        private void buttonX5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image files (*.jpg)|*.jpg";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "导出文件保存路径";
            saveFileDialog.FileName = null;
            saveFileDialog.ShowDialog();
            string strPath = saveFileDialog.FileName;

            Image img = pictureBox1.Image;
            img.Save(strPath);
        }
    }
 
}