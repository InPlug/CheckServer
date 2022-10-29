using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CheckServer
{
    /// <summary>
    /// Demo-Klasse zur Demonstration der Auflösung von komplexen
    /// Return-Objects in einem dynamisch geladenen UserNodeControl.
    /// </summary>
    /// <remarks>
    /// File: ComplexServerReturnObject.cs
    /// Autor: Erik Nagel
    ///
    /// 19.11.2014 Erik Nagel: erstellt
    /// </remarks>
    [Serializable()]
    public class ComplexServerReturnObject
    {
        /// <summary>
        /// Name des Servers, der angepingt werden soll.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Timeout für einen einzelnen Ping.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Anzahl Ping-Versuche, bevor ein Fehler erzeugt wird.
        /// </summary>
        public int Retries { get; set; }

        /// <summary>
        /// Erfolgreicher Ping.
        /// </summary>
        public int SuccessfulRetry { get; set; }

        /// <summary>
        /// Standard Konstruktor.
        /// </summary>
        public ComplexServerReturnObject() { }

        /// <summary>
        /// Deserialisierungs-Konstruktor.
        /// </summary>
        /// <param name="info">Property-Container.</param>
        /// <param name="context">Übertragungs-Kontext.</param>
        protected ComplexServerReturnObject(SerializationInfo info, StreamingContext context)
        {
            this.Server = info.GetString("Server");
            this.Timeout = (int)info.GetValue("Timeout", typeof(int));
            this.Retries = (int)info.GetValue("Retries", typeof(int));
            this.SuccessfulRetry = (int)info.GetValue("SuccessfulRetry", typeof(int));
        }

        /// <summary>
        /// Serialisierungs-Hilfsroutine: holt die Objekt-Properties in den Property-Container.
        /// </summary>
        /// <param name="info">Property-Container.</param>
        /// <param name="context">Serialisierungs-Kontext.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Server", this.Server);
            info.AddValue("Timeout", this.Timeout);
            info.AddValue("Retries", this.Retries);
            info.AddValue("SuccessfulRetry", this.SuccessfulRetry);
        }

        /// <summary>
        /// Überschriebene ToString()-Methode - stellt alle öffentlichen Properties
        /// als einen (zweizeiligen) aufbereiteten String zur Verfügung.
        /// </summary>
        /// <returns>Alle öffentlichen Properties als ein String aufbereitet.</returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder(String.Format("Server: {0}, Attempt: {1}, Timeout: {2}, max Retries: {3}", this.Server, this.SuccessfulRetry, this.Timeout, this.Retries));
            return str.ToString();
        }
    }
}
