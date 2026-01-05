using System;
using System.Collections.Generic;

using System.Data;           //for DataTable

namespace Edge.Tower2.UI
{
    class DB_Product
    {
        // ======================================
        // 2014 11/03
        // DB Control
        // get product name and size
        // ======================================

        public static bool CheckVerifyCode(string v)
        {
            return true;
        }

        public static bool FindProductUID(string uid)
        {
            string selectCmd = "";
            string selectCmd1 = "";
            selectCmd = "Select * ";

            string TableName = "ProductDetail";

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "UID ", "" + uid + "", 1, 1);
            //DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitType ", "" + VisitType + "", 1, 2);

            if (selectCmd == "")
            {
                return false;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;

            //- Check for order field
            string OrderCmd = "";

            //- Cut off comma
            DB.DBCommend.CutOffStringComma(ref  OrderCmd);

            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg);

            if (DV == null || DV.Count == 0)
            {
                return false;
            }

            else
            {
                if (DV.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool FindMonthUsage(string uid)
        {
            return false;
        }
        // 2014 11/03
        public static void InsertProductDetail(string uid)
        {
            string TableName = "ProductDetail";
            //- Insert Data
            string selectCmd = "";
            string Command1 = "";
            string Command2 = "";

            selectCmd = "Insert Into " + TableName + " (";

            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "UID", uid, 1, 1);  //1= text ,2 =integer 
          
            string dt;
            string dt2;
            DateTime date = DateTime.Now;
            DateTime date2 = DateTime.Now;
            dt = date.ToLongTimeString();        // display format:  11:45:44 AM
            dt2 = date2.ToShortDateString();
            string strDateTime = string.Concat(dt2, " ", dt);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Date_Created", strDateTime, 1, 4);

            selectCmd = selectCmd + Command1;

            //- No date in all fields
            if (Command2 == "")
            {
                System.Windows.MessageBox.Show("Data Create Failed!");
                return;
            }

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + ") Values (" + Command2;
            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + ")";

            string ErrMsg = "";
            bool R;
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            R = dbLibrary.InsertData_sw(selectCmd, ref ErrMsg);

            if (R == false)
            {
                System.Windows.MessageBox.Show("Product Detail Insert Failed!");
                return;
            }
            else
            {
                // do nothing
            }
        }

        // Using Dictionary to Save product name 
        public static bool FindProductName(string product_id, out string product_name, out string product_size)
        {
            if (product_id == "" || product_id == null)
            {
                product_name = "";
                product_size = "";
                return false;
            }

            if (dic_Product !=null && (dic_Product.ContainsKey(product_id)))
            {
                product_name = dic_Product[product_id].name;
                product_size = dic_Product[product_id].size;

                return true;
            }
            
            product_name = "";
            product_size = "";
            
            return false;
        }
        
        public static bool FindProductName_DB(string product_id, out string product_name, out string product_size)
        {
            if (product_id == "" || product_id == null)
            {
                product_name = "";
                product_size = "";
                return false;
            }

            product_name = "";
            product_size = "";
            string selectCmd = "";
            string selectCmd1 = "";
            selectCmd = "Select * ";

            string TableName = "ProductInfo";

            DB.DBCommend.CutOffStringComma(ref  selectCmd);

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ProductID ", "" + DB.DBCommend.Quoting(product_id) + "", 1, 1);

            if (selectCmd == "")
            {
                return false;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg, "productinfo.mdb");

            if (DV == null || DV.Count == 0)
            {
                product_name = "";
                product_size = "";
                return false;
            }
            else
            {
                if (DV.Count == 0)
                {
                    return false;
                }
            }

            product_name = (string)DV[0]["ProductName"].ToString();
            product_size = (string)DV[0]["ProductSize"].ToString();
            return true;
        }

        // 2014 11/03
        public static void InsertProductInfo(string product_id, string product_name, string product_size)
        {
            string TableName = "ProductInfo";

            //- Insert Data
            string selectCmd = "";
            string Command1 = "";
            string Command2 = "";

            selectCmd = "Insert Into " + TableName + " (";

            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "ProductID", product_id, 1, 1);  //1= text ,2 =integer 
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "ProductName", DB.DBCommend.Quoting(product_name), 1, 1);  //1= text ,2 =integer   // 0020-11
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "ProductSize", DB.DBCommend.Quoting(product_size), 1, 1);  //1= text ,2 =integer   // 0020-11

            selectCmd = selectCmd + Command1;

            //- No date in all fields
            if (Command2 == "")
            {
                System.Windows.MessageBox.Show("Data Create Failed!");
                return;
            }

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + ") Values (" + Command2;
            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + ")";

            string ErrMsg = "";
            bool R;
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            R = dbLibrary.InsertData_sw(selectCmd, ref ErrMsg, "productinfo.mdb");

            if (R == false)
            {
                System.Windows.MessageBox.Show("Product Insert Failed!");
                return;
            }
            else
            {
                // do nothing
            }

            // 2014 11/30
            //Load_productInfo();
            dic_Product.Add(product_id, new clsContent { name = product_name, size = product_size });
        }

        // 2014 11/30  Load when system bootup
        public static bool Load_ProductInfo()
        {
            string selectCmd = "";
            selectCmd = "Select  * ";

            string TableName = "ProductInfo";

            selectCmd = selectCmd + " from " + TableName + "  " + "Order by ProductID ASC";
         
            if (selectCmd == "")
            {
                // No messages need show 
                return false;
            }

            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg, "productinfo.mdb");

            dic_Product.Clear();
            if (DV == null || DV.Count == 0)
            {
                return false;
            }
            else
            {
                if (DV.Count == 0)
                {
                    return false;
                }
            }
            
            // Add to dic_Product
            for (int i =0 ;i < DV.Count; i++)
            {
                dic_Product.Add(DV[i]["ProductID"].ToString(), new clsContent { name = DV[i]["ProductName"].ToString(), size = DV[i]["ProductSize"].ToString() });
            }

            return true;
        }

        // 2014 11/30
        static Dictionary<string, clsContent> dic_Product = new Dictionary<string, clsContent>();

        // 2014 11/30
        class clsContent
        {
            public string name { get; set; }
            public string size { get; set; }
        }
    }
}
