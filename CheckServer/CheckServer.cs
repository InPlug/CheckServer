using System;
using System.Threading;
using System.Net.NetworkInformation;
using Vishnu.Interchange;
using System.ComponentModel;

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
        public event ProgressChangedEventHandler? NodeProgressChanged;

        /// <summary>
        /// Rückgabe-Objekt des Checkers
        /// </summary>
        public object? ReturnObject
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
        public bool? Run(object? checkerParameters, TreeParameters treeParameters, TreeEvent source)
        {
            this.OnNodeProgressChanged(0);
            ComplexServerReturnObject rtn = new ComplexServerReturnObject();
            this._returnObject = rtn;
            string pString = (checkerParameters)?.ToString()?.Trim() ??
                throw new ArgumentException(String.Format("Es wurden keine Parameter mitgegeben."));
            string[] paraStrings = pString.Split('|');
            string server = paraStrings.Length > 0 ? paraStrings[0] : "";
            if (String.IsNullOrEmpty(server))
            {
                return false;
            }
            rtn.Server = server;
            if (!(paraStrings.Length > 1) || !Int32.TryParse(paraStrings[1], out int timeout))
            {
                timeout = 2000;
            }
            rtn.Timeout = timeout;
            if (!(paraStrings.Length > 2) || !Int32.TryParse(paraStrings[2], out int retries))
            {
                retries = 3;
            }
            rtn.Retries = retries;
            int retry = 0;
            Thread.Sleep(300); // damit man überhaupt was sieht.
            while (retry++ < retries && !this.canPing(server, timeout))
            {
                this.OnNodeProgressChanged((int)(((100.0 * retry) / retries) + .5));
            }
            this.OnNodeProgressChanged(100);
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

        private object? _returnObject = null;

        private void OnNodeProgressChanged(int progressPercentage)
        {
            NodeProgressChanged?.Invoke(null, new ProgressChangedEventArgs(progressPercentage, null));
        }
    }
}
