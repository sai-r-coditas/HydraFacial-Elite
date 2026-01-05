using Edge.IOBoard;

namespace Tester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var r = new ResponseGetTagData();
            r.UpdateFromString(
                "@d4424242422020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020205FFD");
        }
    }
}