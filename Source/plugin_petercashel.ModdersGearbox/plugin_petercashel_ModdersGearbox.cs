using System;
using System.Collections;
using System.Reflection;
using Harmony;
using petercashel.ModdersGearboxAPI.Attributes;
using plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.Patches;
using plugin_petercashel_ModdersGearbox.Features.EventSystem;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
    public class plugin_petercashel_ModdersGearbox : FortressCraftMod
    {
        #region  Fields and Properties

        HarmonyInstance _HarmonyInstance;
        bool _Patched;
        bool mInitalised;
        bool mIsRunning;
        string mWorkingDir;

        [EventHandlers]
        public EventsInstance eventsInstance = new EventsInstance();

        #endregion

        #region  Constructors

        public plugin_petercashel_ModdersGearbox()
        {
            //Harmony();
        }

        #endregion

        #region  Methods

        public void HarmonyPatcher()
        {
            if (_Patched) return;
            _Patched = true;
            _HarmonyInstance = HarmonyInstance.Create(UtilClass.modId);

			//PATCH THIS FIRST

			var original = typeof(SetUVOnCubeToTerrainIndex).GetMethod("SetMaterialUV");
            var prefix = typeof(SetUVOnCubeToTerrainIndexPatch_SetMaterialUV).GetMethod("Prefix");
            _HarmonyInstance.Patch(original, new HarmonyMethod(prefix));

			_HarmonyInstance.PatchAll(typeof(plugin_petercashel_ModdersGearbox).Assembly);

			Debug.Log(String.Format("{0} {1}", UtilClass.modName, "- Harmony Patching Complete"));
		}



		public void Awake()
        {
            HarmonyPatcher();
            ModdersGearboxMain.Awake();
        }

        public void Start()
        {
            HarmonyPatcher();

			foreach (ModConfiguration current in ModManager.mModConfigurations.Mods)
            {
                if (current.Id.Contains(UtilClass.modId))
                {
                    UtilClass.WriteLine($"Found {current.Id} {current.Name}");
                    UtilClass.modName = current.Name;
                    mWorkingDir = current.Path;
                    break;
                }
            }

            UtilClass.WriteLine("Loaded! Using data directory " + mWorkingDir);
            mInitalised = true;
            mIsRunning = true;
            ModdersGearboxMain.Start();
		}

		public override void LowFrequencyUpdate()
        {
            ModdersGearboxMain.LowFrequencyUpdate();
		}

        public void Update()
        {
            ModdersGearboxMain.Update();

            if (EventRegistration.EventsRegistered && !EventRegistration.EventsRegisteredTest)
            {
                EventRegistration.EventsRegisteredTest = true;
				EventRegistration.SendIntermodComms("ModdersGearbox", "Test", null);
			}
		}


        public void OnApplicationQuit()
        {
            ModdersGearboxMain.OnApplicationQuit();
        }


		#endregion



        [SubscribeEvent(SubscribableEvents.IntermodComm)]
        public void HandleIMC(string ModKey, string Message, object payload)
        {
            if (ModKey != "ModdersGearbox") return;
            if (Message.ToLower().Contains("test")) Debug.LogWarning("IntermodComm Instance Plugin Test" + " - " + Message);
		}
	}
}