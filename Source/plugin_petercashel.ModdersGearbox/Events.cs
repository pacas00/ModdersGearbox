using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using petercashel.ModdersGearboxAPI.Attributes;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
    [EventHandlers]
	public static class Events
	{
        [SubscribeEvent(SubscribableEvents.IntermodComm)]
        public static void HandleIMC(string ModKey, string Message, object payload)
        {
            if (ModKey != "ModdersGearbox") return;
            if (Message.ToLower().Contains("test")) Debug.LogWarning("IntermodComm static Test" + " - " + Message);
		}

	}
}
