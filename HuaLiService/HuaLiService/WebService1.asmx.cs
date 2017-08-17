using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

namespace HuaLiService
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://192.168.2.200/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        //用户输入单据号，更新明细分录
        [WebMethod]
        public DataTable getBillInfoByBillNo(string BillNo)
        {
            return SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from icstock where [单据编号] ='" + BillNo + "' and [FActQty] < [实发数量]").Tables[0];
        }

        //用户选中明细分录
        [WebMethod]
        public string getActQtyByBillNoEntryID(string BillNo, string EntryID)
        {
            return SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), CommandType.Text, "select CAST([实发数量] as varchar) + ';' +  CAST([FActQty] as varchar) as jindu from dbo.icstock where [单据编号]='"+BillNo+"' and FEntryID =" + EntryID).ToString();
        }

       
        //写入T_QRCode
        [WebMethod]        
        public int insertQRCode2T_QRCode(string QRCode, string billNo,string EntryID)
        {
            string mingQRCode = EncryptHelper.Decrypt("77052300", QRCode);
            string tableName = "t_QRCode" + mingQRCode.Substring(0, 4);
            string EntryNo = billNo + EntryID.PadLeft(4,'0');
            return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "INSERT INTO [" + tableName + "] ([FQRCode],[FEntryID]) VALUES('" + mingQRCode + "','" + EntryNo + "')");
        }

        ///更新icstock
        [WebMethod]
        public int updateICStockByActQty(string billNo, string EntryID,int SuccessCount)
        {
            return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "UPDATE [icstock] SET [FActQty] = [FActQty] + " + SuccessCount + " WHERE  [单据编号] = '" + billNo + "' and  [FEntryID] =" + EntryID);
        }

    }
}
