using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
    public class plugin_petercashel_ModdersGearbox : FortressCraftMod
	{
		private bool _Patched;
		private HarmonyInstance _HarmonyInstance;
		private bool mInitalised;
		private bool mIsRunning;
		private string mWorkingDir;

		public plugin_petercashel_ModdersGearbox()
		{
			Harmony();
		}

		public void Harmony()
		{

			if (_Patched) return;
			_Patched = true;
			_HarmonyInstance = HarmonyInstance.Create(UtilClass.modId);
			_HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

			foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
			{
				_HarmonyInstance.PatchAll(assemb);
			}

			try
			{
				Debug.Log(UtilClass.modName + " - Harmony Patching Complete");
			}
			catch
			{
			}
		}

		public void Awake()
		{
			Harmony();
		}

		public void Start()
		{
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
		}
	}
}
