using System.ComponentModel;
using Vishnu.Interchange;

namespace CheckServerDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CheckServer.CheckServer checkServer = new CheckServer.CheckServer();
            checkServer.NodeProgressChanged += SubNodeProgressChanged;
            bool? rtn = checkServer.Run("Localhost|1000|3", new TreeParameters("MainTree", null), TreeEvent.UndefinedTreeEvent);
            Console.WriteLine("Ergebnis: {0}, {1}", rtn == null ? "null" : rtn.ToString(),
                checkServer.ReturnObject?.ToString() ?? "null");
            Console.ReadLine();
        }

        static void SubNodeProgressChanged(object? sender, ProgressChangedEventArgs args)
        {
            Console.WriteLine(args.ProgressPercentage);
        }
    }
}