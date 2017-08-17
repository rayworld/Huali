using DevComponents.DotNetBar;
using Ray.Framework.Config;
using Ray.Framework.DBUtility;
using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

using DevComponents.DotNetBar.Controls;

namespace DS9208
{
    public partial class Form2 : Office2007Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        string fName = "";
        DataTable dt = new DataTable();

        #region 事件

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_Load(object sender, EventArgs e)
        {
            this.styleManager1.ManagerStyle = (eStyle)Enum.Parse(typeof(eStyle), ConfigHelper.ReadValueByKey(ConfigHelper.ConfigurationFile.AppConfig, "FormStyle"));
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            dialog.Filter = "Excel97-2003文本文件|*.xls|Excel 2007文件|*.xlsx|所有文件|*.*";
            dialog.RestoreDirectory = true;
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fName = dialog.FileName;
            }

            if (!string.IsNullOrEmpty(fName))
            {
                dt = ReadExcelFile(fName, "Sheet1");
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                dataGridViewX1.DataSource = dt;
                dataGridViewX1.Columns["fdate"].HeaderText = "日期";
                dataGridViewX1.Columns["fbillNo"].HeaderText = "单据编号";
                dataGridViewX1.Columns["fEntryID"].HeaderText = "分录号";
                dataGridViewX1.Columns["fSupplyID"].HeaderText = "购货单位";
                dataGridViewX1.Columns["FDCSPID"].HeaderText = "发货仓库";
                dataGridViewX1.Columns["fitemID"].HeaderText = "产品名称";
                dataGridViewX1.Columns["fQty"].HeaderText = "实发数量";
                dataGridViewX1.Columns["fBatchNo"].HeaderText = "批号";
                dataGridViewX1.Columns["fNote"].HeaderText = "摘要";
                dataGridViewX1.Columns["fSupplyID"].Width = 240;
                dataGridViewX1.Columns["fitemID"].Width = 300;
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                int recCount = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ///对应关系修改
                    string 日期 = dt.Rows[i]["fdate"].ToString();
                    string 单据编号 = dt.Rows[i]["fbillNo"].ToString();
                    string EntryID = dt.Rows[i]["fEntryID"].ToString();
                    string 购货单位 = dt.Rows[i]["fSupplyID"].ToString();
                    string 发货仓库 = dt.Rows[i]["FDCSPID"].ToString();
                    string 产品名称 = dt.Rows[i]["fitemID"].ToString();
                    string 规格型号 = "";
                    float fQty = float.Parse(dt.Rows[i]["fQty"].ToString());
                    float 实发数量 = getFQty(产品名称,fQty);
                    string 批号 = dt.Rows[i]["fBatchNo"].ToString();
                    string 摘要 = dt.Rows[i]["fNote"].ToString();

                    //去重复
                    if (int.Parse(SqlHelper.GetSingle("Select Count(*) From [icstock] WHERE [单据编号] = '" + 单据编号 + "' AND fEntryID = " + EntryID, null).ToString()) < 1)
                    {
                        if (SqlHelper.ExecuteSql("INSERT INTO [icstock] ([日期],[单据编号],[FEntryID],[购货单位],[发货仓库] ,[产品名称] ,[规格型号] ,[实发数量] ,[批号] ,[摘要] ,[FActQty]) VALUES ('" + 日期 + "','" + 单据编号 + "','" + EntryID + "','" + 购货单位 + "','" + 发货仓库 + "','" + 产品名称 + "','" + 规格型号 + "'," + 实发数量 + ",'" + 批号 + "','" + 摘要 + "'," + 0 + ")", null) > 0)
                        {
                            recCount++;
                        }
                    }
                }
                DesktopAlert.Show("<h2>" + "共成功导入 " + recCount.ToString() + " 条记录！" + "</h2>");
            }

        }
        #endregion

        #region 私有过程

        /// <summary>
        /// 将Excel文件转成DataTable
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <param name="strSheetName">工作簿名</param>
        /// <returns></returns>
        private DataTable ReadExcelFile(string strFileName, string strSheetName)
        {
            if (strFileName != "")
            {
                ////office 2003 
                ////string conn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFileName + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                ////office 2007
                ////"Provider=Microsoft.ACE.OLEDB.12.0; Persist Security Info=False;Data Source=" + 文件选择的路径 + "; Extended Properties=Excel 8.0";
                //string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strFileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";  
                //string sql = "select * from [" + strSheetName + "$]";
                ////string sql = "SELECT * FROM OpenDataSource('Microsoft.Jet.OLEDB.4.0','Data Source=" + strFileName + ";Extended Properties='Excel 8.0;HDR=Yes;';Persist Security Info=False')...Sheet1$";
                //OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                //DataSet ds = new DataSet();
                //try
                //{
                //    da.Fill(ds, "table1");
                //}
                //catch
                //{
                //}
                //return ds.Tables[0];

                string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + strFileName + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                string strExcel = "";
                OleDbDataAdapter myCommand = null;
                DataTable dt = null;
                strExcel = "select * from [Sheet1$] order by fentryID";
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                dt = new DataTable();
                try
                {
                    myCommand.Fill(dt);
                }
                catch
                { 
                }
                return dt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查数据库，得到该产品的最小包装单位数量。
        /// </summary>
        /// <param name="productName">产品名称</param>
        /// <param name="qty">总数量</param>
        /// <returns>扫描数量</returns>
        public static float getFQty(string productName , float qty)
        {
            //!!!k3 DB Connections
            string kingdeeConn = "Data Source=192.168.1.223;Initial Catalog=AIS20160702131449;User ID=sa;Password=qaz123";
            float packageQty = int.Parse(SqlHelper.ExecuteScalar(kingdeeConn,CommandType.Text,"",null).ToString());
            return qty / packageQty;

        }
        
        #endregion

        #region UnUsed

        /// <summary>
        /// 
        /// </summary>
        public void LoadBillData()
        {
            //string loadBillSQL = "SELECT * FROM [View_QRCode] where fdate= '" + billDate + "'and fUnitID <> 'BTL' and fUnitID <> 'UNIT'";
            //!!!k3 DB Connections
            //string kingdeeConn = "Data Source=192.168.1.223;Initial Catalog=AIS20160702131449;User ID=sa;Password=qaz123";
            //DataTable dtBill21 = SqlHelper.ExecuteDataSet(kingdeeConn, CommandType.Text, loadBillSQL, null).Tables[0];
            //dataGridViewX1.DataSource = null;

            //dataGridViewX1.DataSource = dtBill21;

        }

        //string billDate= "'2016-11-15'";
        //string loadbillSQL = "SELECT * FROM [icstock] WHERE 日期 = " + billDate;
        //DataTable dtchukudan = SqlHelper.ExcuteDataTable(loadbillSQL);
        //if (dtchukudan.Rows.Count > 0)
        //{
        //    string billNo = "";
        //    int entryID = 1;
        //    int recCount = 0;

        //    for(int i = 0 ; i< dtchukudan.Rows.Count i++)
        //    {
        //        string 日期 = dataGridViewX1.Rows[i].Cells["日期"].Value.ToString();
        //        string 单据编号 = dataGridViewX1.Rows[i].Cells["单据编号"].Value.ToString();
        //        string 购货单位 = dataGridViewX1.Rows[i].Cells["购货单位"].Value.ToString();
        //        string 发货仓库 = dataGridViewX1.Rows[i].Cells["发货仓库"].Value.ToString();
        //        string 产品名称 = dataGridViewX1.Rows[i].Cells["产品名称"].Value.ToString();
        //        string 规格型号 = dataGridViewX1.Rows[i].Cells["规格型号"].Value.ToString();
        //        float 实发数量 = float.Parse(dataGridViewX1.Rows[i].Cells["实发数量"].Value.ToString());
        //        string 批号 = dataGridViewX1.Rows[i].Cells["批号"].Value.ToString();
        //        string 摘要 = dataGridViewX1.Rows[i].Cells["摘要"].Value.ToString();
        //        string ALC_门店代码 = dataGridViewX1.Rows[i].Cells["ALC 门店代码"].Value.ToString();
        //        string Item_Price_Group = dataGridViewX1.Rows[i].Cells["Item Price Group"].Value.ToString();
        //        string 城市 = dataGridViewX1.Rows[i].Cells["城市"].Value.ToString();
        //        string 省份 = dataGridViewX1.Rows[i].Cells["省份"].Value.ToString();
        //        string 渠道 = dataGridViewX1.Rows[i].Cells["日期"].Value.ToString();
        //        string ALC_城市 = dataGridViewX1.Rows[i].Cells["渠道"].Value.ToString();
        //        string ALC大区 = dataGridViewX1.Rows[i].Cells["ALC 城市"].Value.ToString();
        //        string ALC集团代码 = dataGridViewX1.Rows[i].Cells["ALC集团代码"].Value.ToString();
        //        string 促销类别 = dataGridViewX1.Rows[i].Cells["促销类别"].Value.ToString();

        //        if (billNo != 单据编号)
        //        {
        //            entryID = 1;
        //            billNo = 单据编号;
        //        }
        //        else
        //        {
        //            entryID++;
        //        }
        //        //string FEntryId = 单据编号.Substring(4) + entryID.ToString();
        //        if (SqlHelper.ExecuteSql("INSERT INTO [dbo].[icstock] ([日期],[单据编号],[FEntryID],[购货单位],[发货仓库] ,[产品名称] ,[规格型号] ,[实发数量] ,[批号] ,[摘要] ,[ALC 门店代码] ,[Item Price Group] ,[城市] ,[省份] ,[渠道] ,[ALC 城市] ,[ALC大区] ,[ALC集团代码] ,[促销类别] ,[FActQty]) VALUES ('" + 日期 + "','" + 单据编号 + "','" + entryID + "','" + 购货单位 + "','" + 发货仓库 + "','" + 产品名称 + "','" + 规格型号 + "'," + 实发数量 + ",'" + 批号 + "','" + 摘要 + "','" + ALC_门店代码 + "','" + Item_Price_Group + "','" + 城市 + "','" + 省份 + "','" + 渠道 + "','" + ALC_城市 + "','" + ALC大区 + "','" + ALC集团代码 + "','" + 促销类别 + "'," + 0 + ")", null) > 0)
        //        {
        //            recCount++;
        //        }
        //    }

        //    DesktopAlert.Show("共成功导入 " + recCount.ToString() + " 条记录！");
        //}



        //if (dataGridViewX1.RowCount > 0) 
        //{
        //    string billNo = "";
        //    int entryID = 1;
        //    int recCount = 0;
        //    for (int i = 0; i < dataGridViewX1.RowCount; i++) 
        //    {
        //        string 日期 = dataGridViewX1.Rows[i].Cells["日期"].Value.ToString();
        //        string 单据编号 = dataGridViewX1.Rows[i].Cells["单据编号"].Value.ToString();
        //        string 购货单位 = dataGridViewX1.Rows[i].Cells["购货单位"].Value.ToString();
        //        string 发货仓库 = dataGridViewX1.Rows[i].Cells["发货仓库"].Value.ToString();
        //        string 产品名称 = dataGridViewX1.Rows[i].Cells["产品名称"].Value.ToString();
        //        string 规格型号 = dataGridViewX1.Rows[i].Cells["规格型号"].Value.ToString();
        //        float 实发数量 = float.Parse(dataGridViewX1.Rows[i].Cells["实发数量"].Value.ToString());
        //        string 批号 = dataGridViewX1.Rows[i].Cells["批号"].Value.ToString();
        //        string 摘要 = dataGridViewX1.Rows[i].Cells["摘要"].Value.ToString();
        //        string ALC_门店代码 = dataGridViewX1.Rows[i].Cells["ALC 门店代码"].Value.ToString();
        //        string Item_Price_Group = dataGridViewX1.Rows[i].Cells["Item Price Group"].Value.ToString();
        //        string 城市 = dataGridViewX1.Rows[i].Cells["城市"].Value.ToString();
        //        string 省份 = dataGridViewX1.Rows[i].Cells["省份"].Value.ToString();
        //        string 渠道 = dataGridViewX1.Rows[i].Cells["日期"].Value.ToString();
        //        string ALC_城市 = dataGridViewX1.Rows[i].Cells["渠道"].Value.ToString();
        //        string ALC大区 = dataGridViewX1.Rows[i].Cells["ALC 城市"].Value.ToString();
        //        string ALC集团代码 = dataGridViewX1.Rows[i].Cells["ALC集团代码"].Value.ToString();
        //        string 促销类别 = dataGridViewX1.Rows[i].Cells["促销类别"].Value.ToString();
        //        if (billNo != 单据编号)
        //        {
        //            entryID = 1;
        //            billNo = 单据编号;
        //        }
        //        else
        //        {
        //            entryID++;
        //        }
        //        //string FEntryId = 单据编号.Substring(4) + entryID.ToString();
        //        if (SqlHelper.ExecuteSql("INSERT INTO [dbo].[icstock] ([日期],[单据编号],[FEntryID],[购货单位],[发货仓库] ,[产品名称] ,[规格型号] ,[实发数量] ,[批号] ,[摘要] ,[ALC 门店代码] ,[Item Price Group] ,[城市] ,[省份] ,[渠道] ,[ALC 城市] ,[ALC大区] ,[ALC集团代码] ,[促销类别] ,[FActQty]) VALUES ('" + 日期 + "','" + 单据编号 + "','" + entryID + "','" + 购货单位 + "','" + 发货仓库 + "','" + 产品名称 + "','" + 规格型号 + "'," + 实发数量 + ",'" + 批号 + "','" + 摘要 + "','" + ALC_门店代码 + "','" + Item_Price_Group + "','" + 城市 + "','" + 省份 + "','" + 渠道 + "','" + ALC_城市 + "','" + ALC大区 + "','" + ALC集团代码 + "','" + 促销类别 + "'," + 0 + ")", null) > 0)
        //        {
        //            recCount++;
        //        }
        //    }
        //    DevComponents.DotNetBar.Controls.DesktopAlert.Show("共成功导入 " + recCount.ToString() + " 条记录！");
        //}
        #endregion
    }
}
