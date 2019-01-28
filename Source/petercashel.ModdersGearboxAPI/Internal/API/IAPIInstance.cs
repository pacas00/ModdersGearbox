using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace petercashel.ModdersGearboxAPI.Internal.API
{
    /// <summary>
	/// Interface for the API, Implementation is in Modders Gearbox.
	/// </summary>
    public interface IAPIInstance
    {
        //Sub APIs Here






        //Intermod Comms

        /// <summary>
		/// Send a Intermod Comms message to mods that support it.
		/// </summary>
		/// <param name="modKey">Unique key that the mod identifies as</param>
		/// <param name="message">Message id</param>
		/// <param name="payload">What ever payload data object you need, or null</param>
        void SendIntermodComms(string modKey, string message, object payload);




    }
}
