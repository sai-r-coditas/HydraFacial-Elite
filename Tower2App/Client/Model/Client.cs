using System;

namespace Edge.Tower2.UI.Client.Model
{
    public class Client: IComparable
    {
        public string Name { get; set; }
        public int CompareTo(object obj)
        {
            return String.Compare(Name, ((Client)obj).Name, StringComparison.Ordinal);
        }
    }
}