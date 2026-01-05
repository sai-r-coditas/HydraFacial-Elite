using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Edge.Tower2.UI
{
    public class ListViewData
    {
        #region Saves items to product.xml file in bin folder.
        public void Save(System.Windows.Data.CollectionView items)
        {
            XDocument ndoc = new XDocument();

            XElement nRoot = new XElement("productlist");
            XElement nSubRoot = new XElement("products");

            foreach (var item in items)
            {
                ListViewItem lv = (ListViewItem)item;

                XElement xRow = new XElement("product");
                
                xRow.Add(new XElement("name", lv.Col1));
                xRow.Add(new XElement("price", lv.Col2));
                xRow.Add(new XElement("photo", lv.Col3));

                nSubRoot.Add(xRow);
            }
            
            nRoot.Add(nSubRoot);
            ndoc.Add(nRoot);

            //ndoc.Save(Environment.CurrentDirectory+"\\Products\\Products.xml");  // 0101-06
        }
        #endregion

        #region Gets data from product.xml as rows.
        public IEnumerable<object> GetRows()
        {
            List<ListViewItem> rows = new List<ListViewItem>();

            if (File.Exists(Environment.CurrentDirectory+"\\Products\\Products.xml"))
            {
                // Create the query 
                var rowsFromFile = from c in XDocument.Load(
                            Environment.CurrentDirectory + "\\Products\\Products.xml").Elements(
                            "productlist").Elements("products").Elements("product")
                                   select c;

                // Execute the query 
                foreach (var row in rowsFromFile)
                {
                    rows.Add(new ListViewItem(row.Element("name").Value,
                            row.Element("price").Value,row.Element("photo").Value));
                }
            }
            return rows;
        }
        #endregion
    }
}
