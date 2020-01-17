using System;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.Patches
{
	[HarmonyPatch(typeof(ModManager))]
	[HarmonyPatch("LoadTextures")]
	class ModManagerPatch_LoadTextures
	{
		static void Postfix()
		{
			//Call our terrain stitcher here.
			TerrainStitcher.Build();
		}
	}

    //When the script starts up, should grab the new textures.
    [HarmonyPatch(typeof(SurvivalDigScript))]
	[HarmonyPatch("Update")]
	class SurvivalDigScriptPatch_Update
    {
        public static bool bTexturesSet = false;

		static void Postfix(SurvivalDigScript __instance)
		{
            if (bTexturesSet || (!TerrainStitcher.bNeedsToStitch && !TerrainStitcher.bOverrideSetUVCalls))
            {
                return;
            }

            if (PersistentSettings.mbHeadlessServer || GameState.State != GameStateEnum.Playing ||
                WorldScript.mLocalPlayer == null ||
                WorldScript.instance == null)
                return;


			try
            {
                Traverse traverse = Traverse.Create(__instance);
                var particleSystem = traverse.Field("DigParticles").GetValue<ParticleSystem>();

                if (particleSystem != null)
                {
                    var renderer = particleSystem.GetComponent<Renderer>();
                    renderer.material.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
                    renderer.material.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));
                    bTexturesSet = true;
				}
            }
            catch (Exception ex)
            {
                //Something broke.
                Debug.LogException(ex);
                bTexturesSet = false;
            }
		}
	}
}
