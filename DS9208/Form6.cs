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

namespace DS9208
{
    public partial class Form6 : Office2007Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        DataTable dt = new DataTable();
        string mingQRCode = "";

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            mingQRCode = EncryptHelper.Decrypt("77052300", textBoxX2.Text);
            //mingQRCode = textBoxX2.Text;
            //if (!string.IsNullOrEmpty(mingQRCode) && mingQRCode.Length == 9 && mingQRCode.StartsWith(DateTime.Now.Year.ToString().Substring(2)))
            if (!string.IsNullOrEmpty(mingQRCode) && mingQRCode.Length == 9)
            {
                string tableName = "t_QRCode" + mingQRCode.Substring(0, 4);
                ///二维码是否存在
                if (SqlHelper.GetSingle("SELECT FEntryID as interID FROM [dbo].[" + tableName + "] where [FQRCode] = '" + mingQRCode + "' ", null) != null)
                {
                    string interID = SqlHelper.GetSingle("SELECT FEntryID as interID FROM [dbo].[" + tableName + "] where [FQRCode] = '" + mingQRCode + "' Order by FCodeID DESC", null).ToString();
                    string billNo = interID.Substring(0, 10);
                    int entryID = int.Parse(interID.Substring(10));
                    dt = SqlHelper.Query("SELECT * FROM [icstock]  WHERE [单据编号] = '" + billNo + "'  and [FEntryID] = " + entryID.ToString(), null).Tables[0];
                    dataGridViewX1.DataSource = dt;
                }
                else
                {
                    DesktopAlert.Show("<h2>" + mingQRCode + " 二维码不存在！" + "</h2>");
                }
            }
            else
            {
                DesktopAlert.Show("<h2>" + "无效的二维码！" + "</h2>");
            }
        }
    }
}
