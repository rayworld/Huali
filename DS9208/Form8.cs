using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using Ray.Framework.DBUtility;


namespace DS9208
{
    public partial class Form8 : Office2007Form
    {
        string sql = "";

        public Form8()
        {
            InitializeComponent();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            //加载所有数据
            //dataGridViewX1.DataSource = SqlHelper.ExcuteDataTable("SELECT * FROM [icstock]");
            //DesktopAlert.Show("<h2>Level One Heading</h2>");
            comboBoxEx2.SelectedIndex = 0;
        }

        private void textBoxX2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //过滤只显示要删除的的数据
                string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                string billNo = billType + textBoxX2.Text;
                sql = "SELECT [FActQty] as 已扫数量, [日期], [单据编号],[FEntryID]  as 分录号,[购货单位],[产品名称], [发货仓库],[实发数量], [批号], [摘要]  FROM [icstock] where [单据编号] = '" + billNo + "' Order By FEntryID";
                dataGridViewX1.DataSource = SqlHelper.ExcuteDataTable(sql);
                dataGridViewX1.Columns["购货单位"].Width = 240;
                dataGridViewX1.Columns["产品名称"].Width = 300; 
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            //确认后删除
            if (MessageBox.Show("你真的要删除这些数据吗？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int res = 0;
                int resTotal = 0;
                string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                string billNo = billType  + textBoxX2.Text;
                if (dataGridViewX1.SelectedRows.Count > 0)
                {
                    for(int i= 0;i< dataGridViewX1.Rows.Count;i++)
                    {
                        if(dataGridViewX1.Rows[i].Selected == true)
                        {
                            string entryID = dataGridViewX1.Rows[i].Cells["分录号"].Value.ToString();
                            sql = "Delete [icstock] where [单据编号] = '" + billNo + "' and FEntryID = " + entryID;
                            resTotal += SqlHelper.ExecuteSql(sql , null);
 
                            string fID = entryID.PadLeft(4, '0');
                            res += DeleteDetailTable(billNo + fID);
                        }
                    }
                    if (resTotal > 0)
                    {
                        DesktopAlert.Show("<h2>" + resTotal + "条分录," + res + "条二维码被删除！" + "</h2>");
                        //刷新
                        sql = "SELECT [FActQty] as 已扫数量, [日期], [单据编号],[FEntryID]  as 分录号,[购货单位],[产品名称], [发货仓库],[实发数量], [批号], [摘要]  FROM [icstock] where [单据编号] = '" + billNo + "' Order By FEntryID";
                        dataGridViewX1.DataSource = (DataTable)null;
                        dataGridViewX1.DataSource = SqlHelper.ExcuteDataTable(sql);
                        dataGridViewX1.Columns["购货单位"].Width = 240;
                        dataGridViewX1.Columns["产品名称"].Width = 300;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EntryID"></param>
        /// <returns></returns>
        private int DeleteDetailTable(string EntryID)
        {
            int retVal = 0;
            string tableName = "dbo.t_QRCode16";

            for (int i = 0; i < 100; i++)
            {
                string fID = i < 10 ? "0" + i.ToString() : i.ToString();
                sql = "delete " + tableName + fID + " where FEntryID = '" + EntryID + "'";
                retVal += SqlHelper.ExecuteSql(sql, null);
            }
            return retVal;
        }
    }
}