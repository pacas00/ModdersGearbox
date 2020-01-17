using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Harmony;
using plugin_petercashel_ModdersGearbox.Features.EventSystem;

namespace plugin_petercashel_ModdersGearbox.Patch
{
    [HarmonyPatch(typeof(WorldLoadState))]
    [HarmonyPatch("NotifyRegisteredModHandlers")]
	public class WorldStatePatch
	{
        static void Postfix()
		{
            Thread t = new Thread(new ThreadStart(StartInThread));
            t.IsBackground = true;
            t.Start();
        }

        public static void StartInThread()
        {
            try
            {
                EventRegistration.RegisterAllMods();
			}
            catch (Exception exception)
            {
                UtilClass.WriteLine(exception);
            }
		}
	}
}
