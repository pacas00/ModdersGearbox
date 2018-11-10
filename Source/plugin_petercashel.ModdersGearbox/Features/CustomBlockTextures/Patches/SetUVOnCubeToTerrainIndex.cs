using System;
using System.Reflection;
using Harmony;
using plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.HD;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.Patches
{
	[HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
	[HarmonyPatch("StoreCubeMesh")]
	class StoreCubeMesh
	{
		static bool Prefix(GameObject cube)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.StoreCubeMesh(cube);
			}
            else
            {
                SetUVOnCubeToTerrainIndex_HD.StoreCubeMesh(cube);
			}
			return false;
		}
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetCustom")]
    class SetCustom
	{
        static bool Prefix(CustomObjectDesign design, GameObject lCubeObject)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetCustom(design, lCubeObject);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetCustom(design, lCubeObject);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetUVOnly")]
    class SetUVOnly
	{
        static bool Prefix(int cubeType, ushort cubeValue, GameObject lCubeObject)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetUVOnly(cubeType, cubeValue, lCubeObject);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetUVOnly(cubeType, cubeValue, lCubeObject);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetUVAndMaterialBlock")]
    class SetUVAndMaterialBlock
	{
        static bool Prefix(int cubeType, ushort cubeValue, GameObject lCubeObject)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetUVAndMaterialBlock(cubeType, cubeValue, lCubeObject);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetUVAndMaterialBlock(cubeType, cubeValue, lCubeObject);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetUVAndOverrideMaterial")]
    class SetUVAndOverrideMaterial
	{
        static bool Prefix(int cubeType, ushort cubeValue, GameObject lCubeObject)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetUVAndOverrideMaterial(cubeType, cubeValue, lCubeObject);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetUVAndOverrideMaterial(cubeType, cubeValue, lCubeObject);
            }
            return false;
        }
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetUVAndOverrideStaticMesh")]
    class SetUVAndOverrideStaticMesh
	{
        static bool Prefix(int cubeType, ushort cubeValue, GameObject lCubeObject)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetUVAndOverrideStaticMesh(cubeType, cubeValue, lCubeObject);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetUVAndOverrideStaticMesh(cubeType, cubeValue, lCubeObject);
            }
            return false;
        }
	}
    
	[HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
	[HarmonyPatch("SetMaterialUV")]
	class SetUVOnCubeToTerrainIndexPatch_SetMaterialUV
	{
		static bool Prefix(Renderer lRenderer, int lnWhich, ushort lValue, bool lbDoBump)
		{
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
			if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetMaterialUV(lRenderer, lnWhich, lValue, lbDoBump);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetMaterialUV(lRenderer, lnWhich, lValue, lbDoBump);
            }
			return false;
		}
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetFrontUV")]
    class SetFrontUV
	{
        static bool Prefix(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetFrontUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetFrontUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            return false;
        }
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetBackUV")]
    class SetBackUV
	{
        static bool Prefix(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetBackUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetBackUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            return false;
        }
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetLeftUV")]
    class SetLeftUV
	{
        static bool Prefix(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetLeftUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetLeftUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetRightUV")]
    class SetRightUV
	{
        static bool Prefix(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetRightUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetRightUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            return false;
        }
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetBotUV")]
    class SetBotUV
	{
        static bool Prefix(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetBotUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetBotUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            return false;
        }
	}

    [HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
    [HarmonyPatch("SetTopUV")]
    class SetTopUV
	{
        static bool Prefix(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
        {
            if (!TerrainStitcher.bOverrideSetUVCalls) return true;
            if (TerrainStitcher.TextureDefinition == TerrainStitcher.TextureMode.SD)
            {
                SetUVOnCubeToTerrainIndex_SD.SetTopUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            else
            {
                SetUVOnCubeToTerrainIndex_HD.SetTopUV(lnWhich, lValue, lnTotalPix, lnGutter, ref theUVs);
            }
            return false;
        }
    }
}
