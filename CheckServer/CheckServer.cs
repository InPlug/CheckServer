using System;
using System.Threading;
using System.Net.NetworkInformation;
using Vishnu.Interchange;
using NetEti.Globals;

namespace CheckServer
{
    /// <summary>
    /// Prüft, ob bestimmte Server auf Ping reagieren.
    /// </summary>
    /// <remarks>
    /// File: CheckServer.cs
    /// Autor: Erik Nagel
    ///
    /// 05.04.2014 Erik Nagel: erstellt
    /// </remarks>
    public class CheckServer : INodeChecker
    {

        /// <summary>
        /// Kann aufgerufen werden, wenn sich der Verarbeitungs-Fortschritt
        /// des Checkers geändert hat, muss aber zumindest aber einmal zum
        /// Schluss der Verarbeitung aufgerufen werden.
        /// </summary>
        public event CommonProgressChangedEventHandler NodeProgressChanged;

        /// <summary>
        /// Rückgabe-Objekt des Checkers
        /// </summary>
        public object ReturnObject
        {
            get
            {
                return this._returnObject;
            }
            set
            {
                this._returnObject = value;
            }
        }

        /// <summary>
        /// Hier wird der (normalerweise externe) Arbeitsprozess ausgeführt (oder beobachtet).
        /// </summary>
        /// <param name="checkerParameters">Spezifische Aufrufparameter oder null.</param>
        /// <param name="treeParameters">Für den gesamten Tree gültige Parameter oder null.</param>
        /// <param name="source">Auslösendes TreeEvent oder null.</param>
        /// <returns>True, False oder null</returns>
        public bool? Run(object checkerParameters, TreeParameters treeParameters, TreeEvent source)
        {
            ComplexServerReturnObject rtn = new ComplexServerReturnObject();
            this._returnObject = rtn;
            string[] para = checkerParameters.ToString().Split('|');
            string server = para.Length > 0 ? para[0] : "";
            if (String.IsNullOrEmpty(server))
            {
                return false;
            }
            rtn.Server = server;
            int timeout;
            if (!(para.Length > 1) || !Int32.TryParse(para[1], out timeout))
            {
                timeout = 2000;
            }
            rtn.Timeout = timeout;
            int retries;
            if (!(para.Length > 2) || !Int32.TryParse(para[2], out retries))
            {
                retries = 3;
            }
            rtn.Retries = retries;
            int retry = 0;
            this.OnNodeProgressChanged(this.GetType().Name, retries, retry, ItemsTypes.items);
            Thread.Sleep(300); // damit man überhaupt was sieht.
            while (retry++ < retries && !this.canPing(server, timeout))
            {
                this.OnNodeProgressChanged(this.GetType().Name, retries, retry, ItemsTypes.items);
            }
            this.OnNodeProgressChanged(this.GetType().Name, 100, 100, ItemsTypes.items);
            if (retry > retries)
            {
                rtn.SuccessfulRetry = 0;
                return false;
            }
            else
            {
                rtn.SuccessfulRetry = retry;
                return true;
            }
        }

        private bool canPing(string address, int timeout)
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(address, timeout);
                if (reply == null) return false;

                return (reply.Status == IPStatus.Success);
            }
            catch (PingException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        private object _returnObject = null;

        private void OnNodeProgressChanged(string itemsName, int countAll, int countSucceeded, ItemsTypes itemsType)
        {
            if (NodeProgressChanged != null)
            {
                NodeProgressChanged(null, new CommonProgressChangedEventArgs(itemsName, countAll, countSucceeded, itemsType, null));
            }
        }
    }
}
