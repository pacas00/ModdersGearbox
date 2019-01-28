using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plugin_petercashel_ModdersGearbox.API;
using plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox
{
	public static class ModdersGearboxMain
	{
		//These are called by appropriate places

		//Init, Called Always
		public static void Awake()
        {
            petercashel.ModdersGearboxAPI.API.Instance = new APIInstance();
            petercashel.ModdersGearboxAPI.API.APIReady = true;
        }

		//Init, Called Once
		public static void Start()
        {

        }

        public static void Update()
        {

        }

		public static void LowFrequencyUpdate()
        {

        }

        public static void OnApplicationQuit()
		{
			
		}
	}
}
