using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Xml.Linq;

namespace Edge.Tower2.UI
{
    public class lvWifiData
    {
        /// <summary>
        /// Saves items to MyData.xml file in bin folder.
        /// </summary>
        /// <param name="items"></param>
        public void Save(System.Windows.Data.CollectionView items)
        {
            XDocument xdoc = new XDocument();

            XElement xeRoot = new XElement("Data");
            XElement xeSubRoot = new XElement("Rows");

            foreach (var item in items)
            {
                lvWifiItem lvc = (lvWifiItem)item;

                XElement xRow = new XElement("Row");
                xRow.Add(new XElement("col1", lvc.Col1));
                xRow.Add(new XElement("col2", lvc.Col2));
                xRow.Add(new XElement("col3", lvc.Col3));
                xRow.Add(new XElement("col4", lvc.Col4));
                xRow.Add(new XElement("col5", lvc.Col5));
                xRow.Add(new XElement("col6", lvc.Col6));

                xeSubRoot.Add(xRow);
            }
            xeRoot.Add(xeSubRoot);
            xdoc.Add(xeRoot);

            xdoc.Save("MyData.xml");
        }

        /// <summary>
        /// Gets data from MyData.xml as rows.  
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetRows()
        {
            List<lvWifiItem> rows = new List<lvWifiItem>();

            if (File.Exists("MyData.xml"))
            {
                // Create the query 
                var rowsFromFile = from c in XDocument.Load(
                            "MyData.xml").Elements(
                            "Data").Elements("Rows").Elements("Row")
                                   select c;

                // Execute the query 
                foreach (var row in rowsFromFile)
                {
                    rows.Add(new lvWifiItem(
                        row.Element("col1").Value,
                        row.Element("col2").Value,
                        row.Element("col3").Value,
                        row.Element("col4").Value,
                        row.Element("col5").Value,
                        row.Element("col6").Value
                        ));
                }
            }
            return rows;
        }
    }
}
