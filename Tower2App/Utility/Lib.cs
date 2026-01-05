using System;
using System.Linq;

//for Flowdocument
using System.IO;
using System.Xml.Linq;
using System.Windows.Documents;
using System.IO.Packaging;
using System.Windows.Media;

using System.Windows.Media.Imaging;

using System.Collections.Generic;

using System.Text;                                                                              // 0106-09

namespace Edge.Tower2.UI.Utility
{
    class Lib
    {
        // Get file name from Path
        public static string getFileName(string s, int length)                                  // not in use
        {
            if (s.Length <= length)
            {  // do nothing
                return "";
            }
            else
            {
                // get Info from file name
                s = s.Substring(length);
                int i = s.IndexOf("_");

                if (i >= 0)
                {
                    s = s.Substring(i);
                    int j = s.IndexOf(".");
                    if (j != 0)
                    {
                        s = s.Substring(0, s.IndexOf("."));
                    }
                }
                return s;
            }
        }

        public static void loadWordML(FlowDocument flowDoc, string filename)
        {
            XElement wordDoc = null;
            try
            {
                Package package = Package.Open("docs/" + filename);
                Uri documentUri = new Uri("/word/document.xml", UriKind.Relative);
                PackagePart documentPart = package.GetPart(documentUri);
                wordDoc = XElement.Load(new StreamReader(documentPart.GetStream()));            // UTF-8 format
                //add by sww
                package.Close();

            }
            catch (Exception)
            {

                flowDoc.Blocks.Add(new Paragraph(new Run("File not found or not in correct format (Word 2007)")));
                return;
            }

            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            var paragraphs = from p in wordDoc.Descendants(w + "p")
                             select p;

            foreach (var p in paragraphs)
            {
                var style = from s in p.Descendants(w + "pPr")
                            select s;

                var font = (from f in style.Descendants(w + "rFonts")
                            select f.FirstAttribute).FirstOrDefault();
                var size = (from s in style.Descendants(w + "sz")
                            select s.FirstAttribute).FirstOrDefault();

                Paragraph par = new Paragraph();
                Run r = new Run(p.Value);
                if (font != null)
                {
                    FontFamilyConverter converter = new FontFamilyConverter();
                    r.FontFamily = (FontFamily)converter.ConvertFrom(font.Value);
                }
                if (size != null)
                {
                    r.FontSize = double.Parse(size.Value);
                }
                par.Inlines.Add(r);

                flowDoc.Blocks.Add(par);
            }
        }

        private static byte[] buffer;
        private static MemoryStream ms;
        private static BitmapImage image;
        public static void LoadImageFile(System.Windows.Controls.Image C, string ID, string VisitDate, string Type) //0106-16
        {
            string str = ControlParams.Params.Photos_Default+"\\" + ID + "_" + VisitDate + "_" + Type + ".edge";
            try
            {
                if (!File.Exists(str))
                {
                    if (Type == "Profile")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_1.png");
                    else if (Type == "Left")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_2.png");
                    else if (Type == "Right")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_3.png");
                    else if (Type == "CloseUp1")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_4.png");
                    else if (Type == "CloseUp2")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_4.png");
                    else if (Type == "CloseUp3")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_4.png");
                    else
                        str = Environment.CurrentDirectory + "\\Skin\\Images\\NoPhoto.png";        // 0106-09
                }

                // 2014 12/05          
                buffer = System.IO.File.ReadAllBytes(str);
                ms = new MemoryStream(buffer);

                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();

                C.Source = image;

                image.Freeze();

                //C.Source = new BitmapImage(new Uri(str));
            }
            catch(Exception)
            {
                Lib.SaveErrorLog("Invalid file =>" + str);
            }
        }

        public static void LoadImageFile_wTag(ref System.Windows.Controls.Image C, string ID, string VisitDate, string Type)  // 0106-16
        {
            string str = ControlParams.Params.Photos_Default+"\\" + ID + "_" + VisitDate + "_" + Type + ".edge";
            try
            {
                if (!File.Exists(str))
                {
                    if (Type == "Profile")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_1.png");
                    else if (Type == "Left")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_2.png");
                    else if (Type == "Right")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_3.png");
                    else if (Type == "CloseUp1")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_4.png");
                    else if (Type == "CloseUp2")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_5.png");
                    else if (Type == "CloseUp3")
                        str = CheckFileExist(Environment.CurrentDirectory + "\\Skin\\Images\\profile_6.png");
                    else
                        str = Environment.CurrentDirectory + "\\Skin\\Images\\NoPhoto.png";         // 0106-09

                    C.Tag = "";
                }
                else
                    C.Tag = "ShowPhoto";

                // 2014 12/05
                buffer = System.IO.File.ReadAllBytes(str);
                ms = new MemoryStream(buffer);

                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();

                C.Source = image;
                image.Freeze();

                //C.Source = new BitmapImage(new Uri(str));
            }
            catch(Exception)
            {
                Lib.SaveErrorLog("Invalid file =>" + str);
            }
        }

        public static void getFileDateTime(System.Windows.Controls.Label L, string ID, string VisitDate, string Type)  // 0103-06
        {
            string str = ControlParams.Params.Photos_Default + "\\" + ID + "_" + VisitDate + "_" + Type + ".edge";
            if (File.Exists(str))
            {
                DateTime dateModified = System.IO.File.GetLastWriteTime(str);
                L.Content = Type +" "+ dateModified.ToString("dd/MM/yy HH:mm:ss");
            }
            else
            {
                L.Content = Type;
            }
        }

        /// <summary>
        /// Check if image file exist, otherwise show NoPhoto image
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private static string CheckFileExist(string FileName)
        {
            if (!File.Exists(FileName))
                return Environment.CurrentDirectory + "\\Skin\\Images\\NoPhoto.png";            // 0106-09
            else
                return FileName;
        }

        // not for photo capture
        public static void LoadImage(System.Windows.Controls.Image C, string imgFile )          // 0106-16
        {
            string str = Environment.CurrentDirectory + "\\Skin\\Images" + imgFile;
            
            try
            {
                if (!File.Exists(str))
                {
                    str = Environment.CurrentDirectory + "\\Skin\\Images\\NoPhoto.png";         // 0106-09
                }

                //// 2014 12/05
                buffer = System.IO.File.ReadAllBytes(str);
                ms = new MemoryStream(buffer);

                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();

                C.Source = image;

                image.Freeze();

                //C.Source = new BitmapImage(new Uri(str));
            }
            catch(Exception)
            {
                Lib.SaveErrorLog("Invalid file =>" + str);
            }
        }

        public static void LoadImageFromAppDir(System.Windows.Controls.Image C, string imgFile) // 0106-16
        {
            string str = Environment.CurrentDirectory + imgFile;

            try
            {
                if (!File.Exists(str))
                {
                    return;
                }

                buffer = System.IO.File.ReadAllBytes(str);
                ms = new MemoryStream(buffer);

                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();

                C.Source = image;

                image.Freeze();

                //C.Source = new BitmapImage(new Uri(str));
            }
            catch(Exception ex)
            {
                Lib.SaveErrorLog("Invalid file =>" + str);
            }
        }

        /// <summary>
        /// load image with return true/false
        /// </summary>
        /// <param name="C"></param>
        /// <param name="imgFile"></param>
        /// <returns></returns>
        public static bool rLoadImageFromAppDir(System.Windows.Controls.Image C, string imgFile)
        {
            string str = Environment.CurrentDirectory + imgFile;
            if (!File.Exists(str))
            {
                return false;
            }

            try                                                                                 // 0102-39
            {
                //// 2014 12/05
                byte[] buffer = System.IO.File.ReadAllBytes(str);
                MemoryStream ms = new MemoryStream(buffer);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
                C.Source = image;

                image.Freeze();

                //C.Source = new BitmapImage(new Uri(str));

                return true;
            }
            catch (Exception ex)
            {
                Lib.SaveErrorLog("Invalid file =>" + str);
                return false;
            }
        }

        /// <summary>
        /// Load image free locking
        /// </summary>
        /// <param name="C"></param>
        /// <param name="imgFile"></param>
        public static void LoadImageNoLock(System.Windows.Controls.Image C, string imgFile)     // 0106-16
        {
            string str = Environment.CurrentDirectory + imgFile;
            try
            {
                if (!File.Exists(str))
                {
                    SaveErrorLog("File not found-" + imgFile);                                  // 0106
                    return;
                }

                byte[] buffer = System.IO.File.ReadAllBytes(str);
                MemoryStream ms = new MemoryStream(buffer);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();

                C.Source = image;

                image.Freeze();
            }
            catch(Exception)
            {
                Lib.SaveErrorLog("Invalid file =>" + str);
            }
        }

        /// <summary>
        /// Creditcode log file for troubleshooting
        /// </summary>
        /// <param name="msg"></param>
        public static void CreditLog(string msg)                                                // 0103-05 0103-06
        {
            var filename = "credit.log";

            FileInfo f = new FileInfo(filename);
            long L = f.Length;
            if (L > 900000)                                                                     // if file size > 900k
            {
                var lines = System.IO.File.ReadAllLines(filename,Encoding.ASCII);               // 0106-09
                int cnt = lines.Count();

                // Cut all lines in half 
                File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);    // 0106-09
            }

            File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "credit.log"),
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + msg + Environment.NewLine,
                      Encoding.ASCII);                                                          // 0106-09
        }

        /// <summary>
        /// Save update information
        /// </summary>
        /// <param name="msg"></param>
        public static void Savetoupdatelog(string msg)                                          // 0103-05 0103-06
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "update.log");

            if(!File.Exists(filename))
                using (StreamWriter w = new StreamWriter(filename,true,Encoding.ASCII)) ;       // 0106-09

            FileInfo f = new FileInfo(filename);
            long L = f.Length;
            if (L > 900000)                                                                     // if file size > 900k
            {
                var lines = System.IO.File.ReadAllLines(filename, Encoding.ASCII);              // 0106-09
                int cnt = lines.Count();

                // Cut all lines in half 
                File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);    // 0106-09
            }

            File.AppendAllText( filename,
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + msg +
                      Environment.NewLine,
                      Encoding.ASCII);                                                          // 0106-09
        }

        /// <summary>
        /// Record the error information
        /// </summary>
        /// <param name="msg"></param>
        public static void SaveErrorLog(string msg)                                             // 0103-09
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "error.log");

            if (!File.Exists(filename))
                using (StreamWriter w = new StreamWriter(filename, true, Encoding.ASCII)) ;     // 0106-09

            FileInfo f = new FileInfo(filename);
            long L = f.Length;
            if (L > 900000)                                                                     // if file size > 900k
            {
                var lines = System.IO.File.ReadAllLines(filename, Encoding.ASCII);              // 0106-09
                int cnt = lines.Count();

                // Cut all lines in half 
                File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);    // 0106-09
            }

            File.AppendAllText(filename,
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + msg + Environment.NewLine,
                      Encoding.ASCII);                                                          // 0106-09
        }

        public static Dictionary<string, string> dicTextMessages = new Dictionary<string, string>(); // 0103-06
        
        /// <summary>
        /// Supporting multiple language access
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string getTextMessages(string keyword)
        {
            try
            {
                return  Utility.Lib.dicTextMessages[keyword];
            }
            catch (Exception ex)
            {
                System.Media.SystemSounds.Beep.Play();
                System.Media.SystemSounds.Beep.Play();

                SaveErrorLog("Language keyword '" + keyword + "' not found");                   // 0103-09
                return "";
            }
        }
       
    }
}
