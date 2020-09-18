using System;
using NetEti.Globals;
using Vishnu.Interchange;
using CheckServer;

namespace CheckServerDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            CheckServer.CheckServer checkServer = new CheckServer.CheckServer();
            checkServer.NodeProgressChanged += SubNodeProgressChanged;
            bool? rtn = checkServer.Run("Localhost|1000|3", new TreeParameters("MainTree", null), null);
            Console.WriteLine("Ergebnis: {0}, {1}", rtn == null ? "null" : rtn.ToString(),
                (checkServer.ReturnObject as ComplexServerReturnObject).ToString());
            Console.ReadLine();
        }

        static void SubNodeProgressChanged(object sender, CommonProgressChangedEventArgs args)
        {
            Console.WriteLine("{0}: {1} von {2}", args.ItemName, args.CountSucceeded, args.CountAll);
        }
    }
}
