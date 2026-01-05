using System;

//For XML
using System.Xml.Serialization;

namespace Edge.Tower2.UI.CreditCode
{
    [Serializable()]
    public class N_PictureSetting
    {
        [System.Xml.Serialization.XmlElement("Mode")]
        public string Mode { get; set; }

        [System.Xml.Serialization.XmlElement("FilePath")]
        public string FilePath { get; set; }

    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("CreditCodePictureSettings")]
    public class CreditCodePictureSettings
    {
        [XmlArray("InstructionSettings")]
        [XmlArrayItem("PictureSetting", typeof(N_PictureSetting))]
        public N_PictureSetting[] N_PictureSetting { get; set; }
    }
}
