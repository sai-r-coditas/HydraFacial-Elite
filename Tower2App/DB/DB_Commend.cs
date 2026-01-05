using System;
using System.Data.OleDb;     // for ACCESS

namespace Edge.Tower2.UI.DB
{
    //sww
    class DBCommend
    {
        public static string getConnectString()
        {
            if (Settings.DB_Driver == "ACE")
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}", "..\\db\\edge_v001.mdb");    //sww 0102-33
            else
                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}", "..\\db\\edge_v001.mdb");     //sww 0102-33 106-12
        }

        public static string getConnect(string DBName)
        {
            if (Settings.DB_Driver == "ACE")
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}", "..\\db\\"+ DBName);  //sww 0102-33
            else
                return string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}", "..\\db\\" + DBName);  //sww 0102-33   106-12
        }

        public static bool CheckDBAccessibility()   // 0106-12
        {
            try
            {
                OleDbConnection conn;
                string connstr;
                connstr = DBCommend.getConnectString();
                conn = new OleDbConnection(connstr);
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Utility.Lib.SaveErrorLog("Unable to open database!" + ex.ToString());
                return false;
            }
        }

        private void Delete_OneRecord()
        {
            //return true;
        }

        private void Insert_OneRecord()
        {
            //return true;
        }

        private bool Update_OneRecord()
        {
            return true;
        }

        //-- General for TextBox Value
        public static void GetSqlCommand(ref string SqlCmd, string DBFieldName, string UIFieldText, int Mode, int Index)
        {
           
            if (Mode == 1)
                SqlCmd = SqlCmd + DBFieldName + " = '" + UIFieldText + "'" + ",";
            else
                SqlCmd = SqlCmd + DBFieldName + " = " + UIFieldText + "" + ",";
        }

        //-- Generated sql command for textbox fields 
        public static void GetSqlCommandForFind(ref string SqlCmd, string DBFieldName, string UIFieldText, int Mode, int Index)
        {
            if (UIFieldText != "" && UIFieldText != "%" && UIFieldText != "%%") // not accept the Null string
            {
                if (Mode == 1)
                    SqlCmd = SqlCmd + DBFieldName + " Like '" + UIFieldText + "'" + " and ";
                else if (Mode == 2)
                    SqlCmd = SqlCmd + DBFieldName + " = " + UIFieldText + "" + " and ";
                else
                    SqlCmd = SqlCmd + DBFieldName + " = '" + UIFieldText + "'" + " and ";
            }
        }

        //-- Generated sql command for sorting, base on the field name
        public static void GetSqlCommandForOrder(ref string SqlOrderCmd, string DBFieldName, string UIFieldText, int Index)
        {
            if (UIFieldText != "")
            {
                SqlOrderCmd = SqlOrderCmd + " " + DBFieldName + " ASC,";
            }
        }

        //-- Generated insert sql command for text box
        public static void GetSqlCommandForInsert(ref string Str1, ref string Str2, string DBFieldName, string UIFieldText, int Mode, int Index)
        {
            if (UIFieldText != "")
            {
                if (Mode == 1)  //--For Text Field
                {
                    Str1 = Str1 + DBFieldName + " " + ",";
                    Str2 = Str2 + "'" + UIFieldText + "'" + ",";
                }
                else  //--For Interger Field
                {
                    Str1 = Str1 + " " + DBFieldName + " " + ",";
                    Str2 = Str2 + " " + UIFieldText + " " + ",";
                }
            }
        }

        //-- Cut off the last comma on string
        public static void CutOffStringComma(ref string Str)
        {
            if (Str != "" && Str.Substring(Str.Length - 1) == ",")
                Str = Str.Substring(0, Str.Length - 1);
        }

        //-- Cut off the last 'and' on string
        public static void CutOffStringAnd(ref string Str)
        {
            if (Str != "" && Str.Substring(Str.Length - 4) == "and ")
                Str = Str.Substring(0, Str.Length - 4);
        }

        //-- Generated sql command select strings 
        public static void AssembleSelectString(ref string Cmd, string FieldName, int Index)
        {
             Cmd = Cmd + FieldName + " ,";
        }
        
        //-- Reformat as sql command reqired
        public static string Quoting(string Inputstr)
        {
            string correctString = Inputstr.Replace("'", "''");
            return correctString;
        }
     }
}
