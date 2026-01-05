using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Edge.Tower2.UI
{
    [Serializable]
    [XmlRoot("CreditCodePictureSettings")]
    public class PictureSetting
    {
        [Serializable]
        [XmlRoot("PictureSetting")]
        public class StateSettingPair
        {
            private State k;
            private string v;

            public StateSettingPair()
            {
                //
            }

            public StateSettingPair(KeyValuePair<State, string> s)
            {
                k = s.Key;
                v = s.Value;
            }

            public State Mode
            {
                get { return k; }
                set { k = value; }
            }

            public string FilePath
            {
                get { return v; }
                set { v = value; }
            }
        }

        //serialize and deserialize the picture settings
        private const string FILEPATH_PICTURES_SETTING = @"\Disk\creditPictureSetting.xml";

        [XmlIgnore]
        private Dictionary<State, string> imagePathDic;

        public PictureSetting()
        {
            imagePathDic = new Dictionary<State, string>();
        }

        public string GetSetting(State pState)
        {
            string imageLoc;
            imagePathDic.TryGetValue(pState, out imageLoc);
            return imageLoc;
        }

        public StateSettingPair[] InstructionSettings                                           // sww 0102-32
        {
            get
            {
                List<StateSettingPair> tmp = new List<StateSettingPair>();
                foreach (KeyValuePair<State, string> s in imagePathDic)
                {
                    tmp.Add(new StateSettingPair(s));
                }
                return tmp.ToArray();
            }
            set
            {
                imagePathDic = new Dictionary<State, string>();
                foreach (StateSettingPair s in value)
                {
                    imagePathDic.Add(s.Mode, s.FilePath);
                }
            }
        }
       
        public static void Serialize(PictureSetting setting)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PictureSetting));
            TextWriter textWriter = new StreamWriter(Environment.CurrentDirectory + FILEPATH_PICTURES_SETTING);  // 0106-09  UTF-8 format
            serializer.Serialize(textWriter, setting);
            textWriter.Close();
        }

        public static PictureSetting Deserialize()                                              // UTF-8 format
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(PictureSetting));
            TextReader textReader = new StreamReader(Environment.CurrentDirectory + FILEPATH_PICTURES_SETTING);   // UTF-8 only
            
            PictureSetting setting;
            setting = (PictureSetting)deserializer.Deserialize(textReader);
            textReader.Close();

            //// sww loading images ---------------------------------------//sww 0102-32
            ccPictures ccpictures = null;
            string path = Environment.CurrentDirectory + @"\Disk\creditPictureSetting.xml";     // sww 0102-32
            XmlSerializer serializer = new XmlSerializer(typeof(ccPictures));
            StreamReader reader = new StreamReader(path);   // UTF-8 only
            ccpictures = (ccPictures)serializer.Deserialize(reader);
            reader.Close();

            setting.imagePathDic.Clear();   
       
            for (int i = 0; i < ccpictures.ccpictureSetting.Length; i++)
            {
                setting.imagePathDic.Add(ccpictures.ccpictureSetting[i].mode, ccpictures.ccpictureSetting[i].filepath);
            }
            //--------------------------------------------------------------

            return setting;
        }

        [Serializable()]
        public class ccPictureSetting   //sww 0102-32
        {
            [System.Xml.Serialization.XmlElement("Mode")]
            public State mode { get; set; }

            [System.Xml.Serialization.XmlElement("FilePath")]
            public string filepath { get; set; }
        }


        [Serializable()]
        [XmlRoot("CreditCodePictureSettings")]
        public class ccPictures   //sww 0102-32 
        {
            [XmlArray("InstructionSettings")]
            [XmlArrayItem("PictureSetting", typeof(ccPictureSetting))]
            public ccPictureSetting[] ccpictureSetting { get; set; }
        }
        
    }
}
