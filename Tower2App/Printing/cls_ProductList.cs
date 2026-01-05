using System;

using System.Xml.Serialization;

namespace Edge.Tower2.UI.Printing
{
 
    [Serializable()]
    public class product_
    {
        [System.Xml.Serialization.XmlElement("name")]
        public string name { get; set; }

        [System.Xml.Serialization.XmlElement("price")]
        public string price { get; set; }

        [System.Xml.Serialization.XmlElement("photo")]
        public string photo{ get; set; }

        [System.Xml.Serialization.XmlElement("mark")]      // 0101-06
        public string mark { get; set; }
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("productlist")]
    public class productlist
    {
        [XmlArray("products")]
        [XmlArrayItem("product", typeof(product_))]
        public product_[] item { get; set; }
    }

}
