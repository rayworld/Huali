//--------------------------------------------------------------------
// FILENAME: Form1.cs
//
// Copyright ?2011 Motorola Solutions, Inc. All rights reserved.
//
// DESCRIPTION:
//
// NOTES:
//
// 
//--------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Symbol.Barcode2;

namespace MC32N0
{
    public partial class Form1 : Form
    {
        qq.WebService1 ss = new MC32N0.qq.WebService1();
        string QRCode = "";

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanDataCollection"></param>
        private void barcode21_OnScan(ScanDataCollection scanDataCollection)
        {
            ScanData scanData = scanDataCollection.GetFirst;
            if (scanData.Result == Results.SUCCESS)
            {
                if (scanData.Text.Length != 24)
                {
                    MessageBox.Show("��ά��δ����ȷʶ��!");
                    return;
                }

                if(QRCode.IndexOf(scanData.Text) > -1)
                {
                    MessageBox.Show("��ά�������ظ�!");
                    return;
                }


                if (progressBar1.Value < progressBar1.Maximum -1)
                {
                    //��¼�յ��Ķ�ά�룬
                    QRCode += scanData.Text + ";";
                    //������һ��
                    progressBar1.Value++;
                    statusBar1.Text = "��ά�룺 " + scanData.Text + " ɨ��ɹ���";
                    timer1.Enabled = true;          
                }
                else if (progressBar1.Value == progressBar1.Maximum -1) 
                {
                    QRCode += scanData.Text + ";";
                    //������һ��
                    progressBar1.Value++;
                    statusBar1.Text = "��ά�룺 " + scanData.Text + " ɨ��ɹ���";
                    timer1.Enabled = true;          
                    this.button1_Click(button1,EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("��ά������������Χ!");
                    return;
                }      
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && textBox1.Text != "")//�س�
            {
                QRCode = "";
                listView1.Items.Clear();
                DataTable dt = new DataTable();
                string billType= checkBox1.Checked == true? "XOUT":"QOUT";
                string billNo = billType + textBox1.Text;
                dt = ss.getBillInfoByBillNo(billNo);
                if (dt.Rows.Count > 0)
                {
                    statusBar1.Text = "��ѯ������Ϣ�ɹ���";
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
                    statusBar1.Text = "�����ݣ�����鵥�ݱ���Ƿ���ȷ���룡";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("��ȷ��Ҫ�ϴ���Щ������", "ϵͳ��Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                string billNo = billType + textBox1.Text;
                string entryID = listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text;
                string[] QRData = QRCode.Split(';');

                if (billNo == "" || entryID == "")
                {
                    MessageBox.Show("����������ⵥ��ţ�ѡ����ϸ��¼��");
                    return;
                }

                if (QRData.Length == 0)
                {
                    MessageBox.Show("���ϴ�����!");
                    return;
                }
                progressBar1.Value = 0;
                progressBar1.Maximum = QRData.Length - 1;
                int successCount = 0;
                for (int j = 0; j < QRData.Length - 1; j++)
                {
                    //д��T_QRCode/����icstock
                    if (ss.insertQRCode2T_QRCode(QRData[j], billNo, entryID) > 0)
                    {
                        progressBar1.Value++;
                        successCount++;
                        statusBar1.Text = "�ϴ�����(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";
                    }
                }
                ss.updateICStockByActQty(billNo, entryID, successCount);
                string process = ss.getActQtyByBillNoEntryID(billNo, entryID);
                string[] pro = process.Split(';');
                progressBar1.Maximum = int.Parse(pro[0]);
                progressBar1.Value = int.Parse(pro[1]);
                if (progressBar1.Value == progressBar1.Maximum && progressBar1.Maximum > 0)//�˷�¼�Ѿ����
                {
                    listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
                    progressBar1.Value = 0;
                    progressBar1.Maximum = 0;
                    statusBar1.Text = "�˷�¼�Ѿ�ȫ���ϴ����!";
                }
                else
                {
                    statusBar1.Text = "�ɹ��ϴ� " + progressBar1.Value.ToString() + " ����ά���ǩ!";
                    timer1.Enabled = true;
                }
                QRCode = "";
            }
            else
            {
                QRCode = "";
                if (listView1.SelectedIndices.Count > 0)
                {
                    //MessageBox.Show(listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text);
                    string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                    string billNo = billType + textBox1.Text;
                    string entryID = listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text;
                    string process = ss.getActQtyByBillNoEntryID(billNo, entryID);
                    string[] pro = process.Split(';');
                    progressBar1.Maximum = int.Parse(pro[0]);
                    progressBar1.Value = int.Parse(pro[1]);
                    statusBar1.Text = "����(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";

                }
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
                statusBar1.Text = "�˷�¼�Ѿ�ȫ��ɨ�����!";
            }
            else
            {
                statusBar1.Text = "ɨ�����:(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";
            }
            timer1.Enabled = false;
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
                //MessageBox.Show(listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text);
                string billType = checkBox1.Checked == true ? "XOUT" : "QOUT";
                string billNo = billType + textBox1.Text;
                string entryID = listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text;
                string process = ss.getActQtyByBillNoEntryID(billNo, entryID);
                string[] pro = process.Split(';');
                progressBar1.Maximum = int.Parse(pro[0]);
                progressBar1.Value = int.Parse(pro[1]);
                statusBar1.Text = "����(" + progressBar1.Value.ToString() + "/" + progressBar1.Maximum.ToString() + ")";

            }
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            // You must disable the scanner before exiting the application in 
            // order to release all the resources.
            barcode21.EnableScanner = false;
            this.Close();
        }
    }
}