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
using Ray.Framework.Encrypt;
using System.Data.SqlClient;

namespace DS9208
{
    public partial class Form7 : Office2007Form
    {
        DataTable dt = new DataTable();
        public Form7()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form7_Load(object sender, EventArgs e)
        {
            comboBoxEx2.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxX1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //dataGridViewX1.Rows.Clear();
                //得到单据编号
                string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                string billNo = billType + textBoxX1.Text;
                dt = SqlHelper.Query("SELECT [产品名称] as Disp , [FEntryID] as Val FROM [dbo].[icstock] where [单据编号] = '" + billNo + "'", null).Tables[0];
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = 0;
                dt.Rows.InsertAt(dr, 0);
                comboBoxEx1.DataSource = dt;
                comboBoxEx1.DisplayMember = "Disp";
                comboBoxEx1.ValueMember = "Val";
                comboBoxEx1.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxX2.Focus();
        }

        private void textBoxX2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                string billNo = billType + textBoxX1.Text;
                string QRCode = textBoxX2.Text;
                string mingQRCode = EncryptHelper.Decrypt("77052300", QRCode);
                string EntryID = comboBoxEx1.SelectedValue.ToString();
                string interID = billNo + comboBoxEx1.SelectedValue.ToString().PadLeft(4, '0');
                string tableName = "t_QRCode" + mingQRCode.Substring(0, 4);
                int retValDetail = 0;
                int retValTotal = 0;
                retValDetail = SqlHelper.ExecuteSql("DELETE FROM " + tableName + "  WHERE [FQRCode] = '" + mingQRCode + "' and [FEntryID] = '" + interID + "'", null);
                retValTotal = SqlHelper.ExecuteSql("UPDATE [icstock] SET [FActQty] = [FActQty] - 1 WHERE  [单据编号] = '" + billNo + "' and [FActQty] > 0 and  [FEntryID] =" + EntryID, null);
                if (retValTotal > 0 && retValDetail > 0)
                {
                    DesktopAlert.Show("<h2>" + "二维码删除成功！" + "</h2>");
                }
                else if (retValDetail < 1)
                {
                    DesktopAlert.Show("<h2>" + "二维码不存在！" + "</h2>");
                }
                else
                {
                    DesktopAlert.Show("<h2>" + "二维码删除失败！" + "</h2>");
                }
            }
        }
    }
}
