using System;

using System.Data;           //for DataTable
using System.Data.OleDb;     //for ACCESS

namespace Edge.Tower2.UI.DB
{
    class DBLibrary
    {
        private string errorMsg {get;set;}
        private string ConnString {get;set;}

        OleDbConnection conn;
        OleDbCommand cmd;
  
        #region "Database Initialize"
        public bool InitDB()   // 0106-12
        {
            try
            {
                ConnString = DBCommend.getConnectString();
                conn = new OleDbConnection(ConnString);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Utility.Lib.SaveErrorLog("Unable to open database!"+ ex.ToString());
                return false;
            }
        }

        public bool InitDB(string DB)  // 0106-12
        {
            try
            {
                ConnString = DBCommend.getConnect(DB);
                conn = new OleDbConnection(ConnString);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Utility.Lib.SaveErrorLog("Unable to open database!" +ex.ToString());
                return false;
            }
        }
        #endregion

        //-- Insert New Data
        #region "Add new data"
        /// <summary>
        /// Add new data
        /// </summary>
        /// <param name="">string InsertCmd, ref string ErrMsg</param>
        /// <returns></returns>
        public bool InsertData_sw(string InsertCmd, ref string ErrMsg)
        {
            if (!InitDB()) // 0106-12
            {
                ErrMsg = "Open database failed!";
                return false;
            }

            try
            {
                cmd = new OleDbCommand(InsertCmd, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return false;
            }
        }

        public bool InsertData_sw(string InsertCmd, ref string ErrMsg, string DB)  
        {
            if (!InitDB(DB))  // 0106-12
            {
                ErrMsg = "Open database failed!";
                return false;
            }

            try
            {
                cmd = new OleDbCommand(InsertCmd, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return false;
            }
        }
        #endregion

        //-- Update one record
        #region "Update"
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="">string updateCmd, ref string ErrMsg</param>
        /// <returns></returns>
        public bool Update_sw(string updateCmd, ref string ErrMsg)
        {
            if (!InitDB()) // 0106-12
            {
                ErrMsg = "Open database failed!";
                return false;
            }

            int i = 0;
            try
            {
                cmd = new OleDbCommand(updateCmd, conn);
                i = cmd.ExecuteNonQuery();
                conn.Close();
                if (i == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return false;
            }
        }
        #endregion

        //-- Delete one record
        #region "Delete one record"
        /// <summary>
        /// Delete one record
        /// </summary>
        /// <param name="PKval">string delCmd, ref string ErrMsg</param>
        /// <returns></returns>
        public bool DeleteOneData_sw(string delCmd, ref string ErrMsg)
        {
            if (!InitDB()) // 0106-12
            {
                ErrMsg = "Open database failed!";
                return false;
            }

            int i = 0;
            try
            {
                cmd = new OleDbCommand(delCmd, conn);
                //--if fail return 0
                i = cmd.ExecuteNonQuery();
                conn.Close();
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return false;
            }
        }
        #endregion

        //-- Delete one record by TableName, IDfield ... specified
        #region "DeleteOneData"
        /// <summary>
        /// Delete one record in database
        /// </summary>
        /// <param name="">string TableName, string IDField, string ID, ref string ErrMsg</param>
        /// <returns></returns>
        public bool DeleteOneData_sw(string TableName, string IDField, string ID, ref string ErrMsg)
        {
            if (!InitDB()) // 0106-12
            {
                ErrMsg = "Open database failed!";
                return false;
            }

            int i = 0;
            try
            {
                string delCmd = "";
                delCmd = "Delete from " + TableName + " Where " + IDField + " = '" + ID + "'";
                cmd = new OleDbCommand(delCmd, conn);
                //--if failed return 0
                i = cmd.ExecuteNonQuery();
                conn.Close();
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return false;
            }
        }
        #endregion

        //-- Load database by single sql command string
        #region "LoadDataTable"
        /// <summary>
        /// LoadDataTable
        /// </summary>
        /// <param name="">string selectCmd, ref string ErrMsg</param>
        /// <returns></returns>
        public DataTable LoadDataTable_sw(string selectCmd, ref string ErrMsg)
        {
            if (!InitDB()) // 0106-12
            {
                ErrMsg = "Open database failed!";
                return null;
            }

            try
            {
                OleDbDataAdapter da = new OleDbDataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return null;
            }
        }
        #endregion

        //-- Load database by select command and tablename
        #region "LoadDataView_sw"
        /// <summary>
        /// LoadDataView
        /// </summary>
        /// <param name="">string selectCmd, string TableName, ref string ErrMsg</param>
        /// <returns></returns>
        public DataView LoadDataView_sw(string selectCmd, string TableName, ref string ErrMsg)
        {
            if (!InitDB()) // 0106-12
            {
                ErrMsg = "Open database failed!";
                return null;
            }

            try
            {
                DataSet ds = new DataSet();
                DataView DVbuf = new DataView();
                OleDbDataAdapter da = new OleDbDataAdapter(selectCmd, conn);
                da.Fill(ds, TableName);
                DVbuf = ds.Tables[TableName].DefaultView;
                conn.Close();
                da.Dispose();
                return DVbuf;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return null;
            }
        }

        public DataView LoadDataView_sw(string selectCmd, string TableName, ref string ErrMsg, string DB)
        {
            if (!InitDB(DB))
            {
                ErrMsg = "Open database failed!";
                return null;
            }

            try
            {
                DataSet ds = new DataSet();
                DataView DVbuf = new DataView();
                OleDbDataAdapter da = new OleDbDataAdapter(selectCmd, conn);
                da.Fill(ds, TableName);
                DVbuf = ds.Tables[TableName].DefaultView;
                conn.Close();
                da.Dispose();
                return DVbuf;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                ErrMsg = ex.Message;
                conn.Close();
                return null;
            }
        }
        #endregion
    }
}
