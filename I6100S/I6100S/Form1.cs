using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Urovo;
using System.Runtime.InteropServices;

namespace I6100S
{   
    public partial class Form1 : Form
    {
        Scanner scanner = new Scanner();
        string QRCode = "";
        qq.WebService1 ss = new I6100S.qq.WebService1();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
            scanner.DecodeEvent += new EventHandler<DecodeEventArgs>(scanner_DecodeEvent);
            scanner.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void scanner_DecodeEvent(object sender, DecodeEventArgs e)
        {
            Win32.sndPlaySound(Properties.Resources.Scan, Win32.SND_ASYNC | Win32.SND_MEMORY);
            this.BeginInvoke((Action<string>)delegate(string barcode)
            {
                barcode = barcode.Substring(0, 24);
                //2016-11-02
                if (barcode.Length != 24)
                {
                    MessageBox.Show("二维码未能正确识别!");
                    return;
                }

                if (QRCode.IndexOf(barcode) > -1)
                {
                    MessageBox.Show("二维码数量重复!");
                    return;
                }

                if (progressBar1.Value < progressBar1.Maximum - 1)
                {

                    //记录收到的二维码，
                    QRCode += barcode + ";";
                    //计数加一，
                    progressBar1.Value++;
                    statusBar1.Text = "二维码： " + barcode + " 扫描成功！";
                    timer1.Enabled = true;  
                }
                else if (progressBar1.Value == progressBar1.Maximum - 1)
                {
                    barcode = barcode.Substring(0, 24);
                    QRCode += barcode + ";";
                    //计数加一，
                    progressBar1.Value++;
                    statusBar1.Text = "二维码： " + barcode + " 扫描成功！";
                    timer1.Enabled = true; 
                    this.button1_Click(button1, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("二维码数量超过范围!");
                    return;
                }

            }, e.Barcode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            scanner.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && textBox1.Text != "")//回车
            {
                QRCode = "";
                listView1.Items.Clear();
                DataTable dt = new DataTable();
                string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                string billNo = billType + textBox1.Text;
                dt = ss.getBillInfoByBillNo(billNo);
                if (dt.Rows.Count > 0)
                {
                    statusBar1.Text = "查询单据信息成功！";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.SubItems[0].Text = dt.Rows[i].ItemArray[3].ToString();
                        item.SubItems.Add(dt.Rows[i].ItemArray[6].ToString());
                        item.SubItems.Add(dt.Rows[i].ItemArray[4].ToString());
                        listView1.Items.Add(item);
                    }
                }
                else
                {
                    statusBar1.Text = "无数据，经检查单据编号是否正确输入！";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            QRCode = "";
            if (listView1.SelectedIndices.Count > 0)
            {
                string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                string billNo = billType + textBox1.Text;
                string entryID = listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text;
                string process = ss.getActQtyByBillNoEntryID(billNo, entryID);
                string[] pro = process.Split(';');
                progressBar1.Maximum = int.Parse(pro[0]);
                progressBar1.Value = int.Parse(pro[1]);
                statusBar1.Text = "进度(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value == progressBar1.Maximum)
            {
                statusBar1.Text = "此分录已经全部扫描完成!";
            }
            else
            {
                statusBar1.Text = "扫描进度:(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";
            }
            timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确信要上传这些数据吗？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                string billNo = billType + textBox1.Text;
                string entryID = listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text;
                string[] QRData = QRCode.Split(';');

                if (billNo == "" || entryID == "")
                {
                    MessageBox.Show("请先输入出库单编号，选择明细分录！");
                    return;
                }

                if (QRData.Length == 0)
                {
                    MessageBox.Show("无上传数据!");
                    return;
                }
                progressBar1.Value = 0;
                progressBar1.Maximum = QRData.Length - 1;
                int successCount = 0;
                for (int j = 0; j < QRData.Length - 1; j++)
                {
                    //写入T_QRCode/更新icstock
                    if (ss.insertQRCode2T_QRCode(QRData[j], billNo, entryID) > 0)
                    {
                        progressBar1.Value++;
                        successCount++;
                        statusBar1.Text = "上传进度(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";
                    }
                }
                ss.updateICStockByActQty(billNo, entryID, successCount);
                string process = ss.getActQtyByBillNoEntryID(billNo, entryID);
                string[] pro = process.Split(';');
                progressBar1.Maximum = int.Parse(pro[0]);
                progressBar1.Value = int.Parse(pro[1]);
                if (progressBar1.Value == progressBar1.Maximum && progressBar1.Maximum > 0)//此分录已经完成
                {
                    listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
                    progressBar1.Value = 0;
                    progressBar1.Maximum = 0;
                    statusBar1.Text = "此分录已经全部上传完成!";
                }
                else
                {
                    statusBar1.Text = "成功上传 " + progressBar1.Value.ToString() + " 条二维码标签!";
                    timer1.Enabled = true;
                }
                QRCode = "";
            }
            else
            {
                QRCode = "";
                if (listView1.SelectedIndices.Count > 0)
                {
                    string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                    string billNo = billType + textBox1.Text;
                    string entryID = listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text;
                    string process = ss.getActQtyByBillNoEntryID(billNo, entryID);
                    string[] pro = process.Split(';');
                    progressBar1.Maximum = int.Parse(pro[0]);
                    progressBar1.Value = int.Parse(pro[1]);
                    statusBar1.Text = "进度(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";

                }
            }
        }
    }
}