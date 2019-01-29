using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using petercashel.ModdersGearboxAPI;
using plugin_petercashel_ModdersGearbox.Features.EventSystem;

namespace plugin_petercashel_ModdersGearbox.API
{
	public class APIInstance : IAPIInstance
	{
        public APIInstance()
        {
            //Setup other things here.

        }

		public void SendIntermodComms(string modKey, string message, object payload)
        {
            EventRegistration.SendIntermodComms(modKey, message, payload);
        }
	}
}
