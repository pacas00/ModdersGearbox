using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using petercashel.ModdersGearboxAPI.Internal.API;

namespace petercashel.ModdersGearboxAPI
{
    /// <summary>
	/// Primary API Entrypoint
	/// </summary>
    public static class API
    {
        /// <summary>
		/// At runtime, this will be populated with an instance of the API
		/// </summary>
        public static IAPIInstance Instance;

        /// <summary>
		/// APIReady will be set true once the API is ready is initialised.
		/// </summary>
        public static bool APIReady = false;

    }
}
