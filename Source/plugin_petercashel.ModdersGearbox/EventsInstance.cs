﻿using petercashel.ModdersGearboxAPI.Attributes;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
    public class EventsInstance
	{
		public EventsInstance()
		{

		}

        [SubscribeEvent(SubscribableEvents.IntermodComm)]
        public void HandleIMC(string ModKey, string Message, object payload)
        {
            if (ModKey != "ModdersGearbox") return;
            if (Message.ToLower().Contains("test")) Debug.LogWarning("IntermodComm Instance EventsInstance Test" + " - " + Message);
        }
	}
}