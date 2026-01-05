
namespace Edge.Tower2.UI
{
    static class Product
    {
        public static string Name1(string partNumber)
        {
            if (partNumber == null)
                return "";

            switch (partNumber)
            {
                // sww added
                case "70137": return "Activ-4 (8oz/237ml)";
                case "70229": return "Activ-4 (16oz/473ml)";
                case "70138": return "Beta-HD (8oz/237ml)";
                case "70230": return "Beta-HD (16oz/473ml)";
                case "70139": return "Antiox-6 (8oz/237ml)";
                case "70231": return "Antiox-6 (16oz/473ml)";
                case "70140": return "Rinseaway (16oz/473ml)";

                default:
                    return "???????";
            }
        }

        public static string Name(string partNumber)
        {
            if (partNumber == null || partNumber =="")
                return "";
            
            string _pdname, _pdsize;
            if (DB_Product.FindProductName(partNumber,out _pdname,out _pdsize))
                return _pdname +" "+ _pdsize;
            else
                return "???????";
        }

        public static string Name(BottleChangedEventArgs ea)
        {
            string _pdname, _pdsize;
            if (ea.Station.PartNumber == null)
                return "";
            
            // 2014 11/30
            if (DB_Product.FindProductName(ea.Station.PartNumber, out _pdname, out _pdsize))
                return _pdname + _pdsize;
            else
            {
                if (ea.Station.PartNumber != "" & ea.Station.PartNumber != null)
                {
                    DB_Product.InsertProductInfo(ea.Station.PartNumber, ea.Station.ProductName, ea.Station.ProductSize);

                    if (DB_Product.FindProductName(ea.Station.PartNumber, out _pdname, out _pdsize))
                        return _pdname + _pdsize;
                    else
                        return "???????";
                    //Load_dic_Product();
                }
                return "???????";
            }
        }

        // Not in use
        public static string BottleSize(string partNumber)
        {
            if (partNumber == null)
                return "";

            switch (partNumber)
            {
                //case "70001":
                //    return "(8oz/237ml)";

                //case "70002":
                //    return "(8oz/237ml)";

                //case "70003":
                //    return "(8oz/237ml)";

                //case "70004":
                //    return "(8oz/237ml)";

                // sww added
                case "70137": return "(8oz/237ml)";
                case "70229": return "(16oz/473ml)";
                case "70138": return "(8oz/237ml)";
                case "70230": return "(16oz/473ml)";
                case "70139": return "(8oz/237ml)";
                case "70231": return "(16oz/473ml)";
                case "70140": return "(16oz/473ml)";

                default:
                    return "???????";
            }
        }
    }

}
