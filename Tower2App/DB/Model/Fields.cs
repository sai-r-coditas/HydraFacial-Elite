
namespace Edge.Tower2.UI.DB.Model
{
    public class Fields
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Date_Created { get; set; }
        public string Birthday { get; set; }
        public string TelNo { get; set; }
        public string PhotoL { get; set; }
        public string PhotoC { get; set; }
        public string PhotoR { get; set; }
        public string PhotoL_Desc { get; set; }
        public string PhotoC_Desc { get; set; }
        public string PhotoR_Desc { get; set; }
        public string Operator { get; set; }
        public string PhoneType { get; set; }
    }

    public class FieldName
    {
        public static string ID { get { return "ID"; } }
        public static string Name { get { return "Name"; } }
        public static string Date_Created { get { return "Date_Create"; } }
        public static string Birthday { get { return "Birthday"; } }
        public static string TelNo { get { return "TelNo"; } }
        public static string PhotoL { get { return "PhotoL"; } }
        public static string PhotoC { get { return "PhotoC"; } }
        public static string PhotoR { get { return "PhotoR"; } }
        public static string PhotoL_Desc { get { return "PhotoL_Desc"; } }
        public static string PhotoC_Desc { get { return "PhotoC_Desc"; } }
        public static string PhotoR_Desc { get { return "PhotoR_Desc"; } }
        public static string Operator { get { return "Operator"; } }
        public static string PhoneType { get { return "PhoneType"; } }
    }
}
