using System;
using System.Data;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System.Configuration;

using Ray.Framework.DBUtility;
using Ray.Framework.Encrypt;

namespace DS9208
{
    public partial class Form4 : Office2007Form
    {
        string mingQRCodes = "";
        DataTable dt = (DataTable)null;

        public Form4()
        {
            InitializeComponent();
        }

        #region 事件
        /// <summary>                                                                           
        /// 用户输入新的出库单号并确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxX1_KeyDown(object sender, KeyEventArgs e)
        {
            ////用户按下回车键
            if (e.KeyCode == Keys.Enter)
            {
                //清空选项，
                dataGridViewX1.DataSource = (DataTable)null;
                dataGridViewX1.Rows.Clear();
                dataGridViewX1.Columns.Clear();
                //单据编号为数字
                if (!string.IsNullOrEmpty(textBoxX1.Text) && IsNumber(textBoxX1.Text))
                {
                    //清空二维码列表，
                    mingQRCodes = "";
                    //得到单据编号
                    string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                    string billNo = billType + textBoxX1.Text;
                    //收到单据分录信息
                    int recCount = int.Parse(SqlHelper.GetSingle("select count(*) from icstock where [单据编号] ='" + billNo + "' and [FActQty] < [实发数量]",null).ToString());
                    if (recCount > 0)
                    {
                        DataTable dtmaster = SqlHelper.ExcuteDataTable("select top 1 [日期],[购货单位],[发货仓库],[摘要] from icstock where [单据编号] ='" + billNo + "' and [FActQty] < [实发数量]");
                        textBoxX2.Text = dtmaster.Rows[0][0].ToString();
                        textBoxX3.Text = dtmaster.Rows[0][1].ToString();
                        textBoxX4.Text = dtmaster.Rows[0][2].ToString();

                        dt = SqlHelper.ExcuteDataTable("select [fEntryID] as 分录号,[产品名称],[批号],[实发数量] as 应发,[FActQty] as 实发  from icstock where [单据编号] ='" + billNo + "' and [FActQty] < [实发数量] order by fEntryID");
                        dataGridViewX1.DataSource = dt;;
                        DataGridViewCheckBoxColumn newColumn = new DataGridViewCheckBoxColumn();
                        newColumn.HeaderText = "选择";
                        dataGridViewX1.Columns.Insert(0, newColumn);
                        dataGridViewX1.Columns["产品名称"].Width = 400;
                        dataGridViewX1.Rows[0].Selected = true;
                        //
                        textBoxItem1.Focus();
                    }
                    else
                    {
                        DesktopAlert.Show("<h2>" + "无数据，请检查单据编号的输入!" + "</h2>");
                    }
                }
                else
                {
                    DesktopAlert.Show("<h2>" + "请检查单据编号的输入!" + "</h2>");
                }
            }
        }


        /// <summary>
        /// 程序启动时运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form4_Load(object sender, EventArgs e)
        {
            comboBoxEx2.SelectedIndex = 0;
            textBoxItem1.TextBoxWidth = 200;
            expandableSplitter1.Left = dataGridViewX1.Width;
            expandableSplitter1.Expanded = true;
            //dataGridViewX1.Width = this.Width - expandableSplitter1.Width;
            //panelEx2.Width = 0;
            //Only for test
            //textBoxX1.Text = "020804";

        }

        /// <summary>
        /// 拆单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            for (int i = dataGridViewX1.RowCount - 1; i > -1; i--)
            {
                if (dataGridViewX1.Rows[i].Cells[0].EditedFormattedValue.ToString() != "True")
                {
                    dataGridViewX1.Rows.Remove(dataGridViewX1.Rows[i]);
                }
            }
            dataGridViewX1.Rows[0].Selected = true;
            textBoxItem1.Focus();
        }

        #endregion

        #region 私有过程

        /// <summary>  
        /// 判读字符串是否为数值型
        /// </summary>  
        /// <param name="strNumber">字符串</param>  
        /// <returns>是否</returns>  
        public static bool IsNumber(string strNumber)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"^-?\d+\.?\d*$");
            return r.IsMatch(strNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mingQRCode"></param>
        /// <param name="billNo"></param>
        /// <param name="EntryID"></param>
        /// <returns></returns>
        public int insertQRCode2T_QRCode(string mingQRCode, string billNo, string EntryID)
        {
            string tableName = "t_QRCode" + mingQRCode.Substring(0, 4);
            string EntryNo = billNo + EntryID.PadLeft(4, '0');
            return SqlHelper.ExecuteSql("INSERT INTO [" + tableName + "] ([FQRCode],[FEntryID]) VALUES('" + mingQRCode + "','" + EntryNo + "')");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billNo"></param>
        /// <param name="EntryID"></param>
        /// <returns></returnsT
        public int updateICStockByActQty(string billNo, string EntryID)
        {
            return SqlHelper.ExecuteSql("UPDATE [icstock] SET [FActQty] = [FActQty] + 1 WHERE  [单据编号] = '" + billNo + "' and  [FEntryID] =" + EntryID);
        }

        #endregion

        private void textBoxItem1_KeyDown(object sender, KeyEventArgs e)
        {
            //用户按下回车键
            if (e.KeyCode == Keys.Enter)
            {
                //如果已经扫描二维码个数小于该分录总数，则继续扫描，
                int maxVal = int.Parse(dataGridViewX1.SelectedRows[0].Cells[4].Value.ToString());
                int currVal = int.Parse(dataGridViewX1.SelectedRows[0].Cells[5].Value.ToString());

                if (currVal < maxVal)
                {
                    //去掉回车换行符
                    string QRCode = textBoxItem1.Text.Trim().Replace(" ", "").Replace("\n", "").Replace("\r\n", "");
                    //揭秘成明码
                    string mingQRCode = EncryptHelper.Decrypt("77052300", QRCode);
                    //显示明码
                    labelItem2.Text = mingQRCode;

                    //扫描窗口重新获得焦点
                    textBoxItem1.Text = "";
                    labelItem2.Text = "";
                    textBoxItem1.Focus();

                    //显示状态信息
                    string billType = comboBoxEx2.SelectedIndex == 0 ? "XOUT" : "QOUT";
                    string billNo = billType + textBoxX1.Text;
                    string entryID = dataGridViewX1.SelectedRows[0].Cells[1].Value.ToString();

                    //限定二维码信息
                    if (string.IsNullOrEmpty(mingQRCode) || mingQRCode.Length != 9 || IsNumber(mingQRCode) == false)
                    {
                        ////DesktopAlert.Show("<h2>" + "二维码未能正确识别！" + "</h2>");
                        DesktopAlert.Show("<h2>" + "二维码未能正确识别！" + "</h2>");
                        if (string.IsNullOrEmpty(mingQRCode))
                        {
                            DesktopAlert.Show("<h2>" + "111111111！" + "</h2>");
                        }
                        else if (mingQRCode.Length != 9)
                        {
                            DesktopAlert.Show("<h2>" + "222222222！" + "</h2>");
                        }
                        else
                        {
                            DesktopAlert.Show("<h2>" + "333333333！" + "</h2>");
                        }
                        return;
                    }

                    //单据编号和分录编号不为空
                    if (billNo == "" || entryID == "")
                    {
                        DesktopAlert.Show("<h2>" + "请先输入出库单编号，选择明细分录！" + "</h2>");
                        return;
                    }

                    //查重
                    int index = mingQRCodes.IndexOf(mingQRCode);
                    if (index > -1)
                    {
                        DesktopAlert.Show("<h2>" + "此二维码录入重复！" + "</h2>");
                        return;
                    }
                    mingQRCodes += mingQRCode + ";";

                    //写入T_QRCode
                    //billNo = billNo.Substring(0, 1) + billNo.Substring(4);
                    insertQRCode2T_QRCode(mingQRCode, billNo, entryID);
                    //更新icstock
                    updateICStockByActQty(billNo, entryID);



                    //更新状态栏
                    currVal++;
                    dataGridViewX1.SelectedRows[0].Cells[5].Value = currVal;

                    if (currVal == maxVal)//此分录已经完成
                    {
                        dataGridViewX1.Rows.Remove(dataGridViewX1.SelectedRows[0]);
                        //此出库单已经全部录入完成
                        if (dataGridViewX1.Rows.Count == 0)
                        {
                            DesktopAlert.Show("<h2>" + "此出库单已经全部录入完成！" + "</h2>");
                        }
                        else//此分录已经全部录入完成
                        {
                            dataGridViewX1.Rows[0].Selected = true;
                            DesktopAlert.Show("<h2>" + "此分录已经全部录入完成！" + "</h2>");
                        }
                        //清空二维码录入记录
                        mingQRCodes = "";
                    }
                }
                else
                {
                    DesktopAlert.Show("<h2>" + "二维码数量超过范围！" + "</h2>");
                    return;
                }
            }
        }

        /// <summary>
        /// 用户重新选择了分录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            mingQRCodes = "";
            textBoxItem1.Focus();

        }

        private void expandableSplitter1_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            panelEx2.Width = expandableSplitter1.Expanded  == true ? 360 : 0;
            dataGridViewX1.Width = this.Width - panelEx2.Width;
        }       
    }
}
