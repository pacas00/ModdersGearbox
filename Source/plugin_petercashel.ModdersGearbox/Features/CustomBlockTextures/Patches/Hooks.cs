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
	
	[HarmonyPatch(typeof(SetUVOnCubeToTerrainIndex))]
	[HarmonyPatch("SetMaterialUV")]
	class SetUVOnCubeToTerrainIndexPatch_SetMaterialUV
	{
		static bool Prefix(Renderer lRenderer, int lnWhich, ushort lValue, bool lbDoBump)
		{
			SetMaterialUV(lRenderer, lnWhich, lValue, lbDoBump);
			return false;
		}
	
		public static void SetMaterialUV(Renderer lRenderer, int lnWhich, ushort lValue, bool lbDoBump)
		{
			int num = 146;
			int num2 = 0;
			int num3 = 14;
			int sideTexture = global::TerrainData.GetSideTexture(lnWhich, lValue);
			int num4 = sideTexture % num3;
			int num5 = sideTexture / num3;
			float num8 = (float)(num * num4);
			int num6 = num * num5;
			float num9 = num8 + (float)num2;
			num6 -= num2;
			num6 += num;
			float z = num9 / TerrainUV.lnMatPix;
			float num7 = (float)num6 / TerrainUV.lnMatPiy;
			num7 = 1f - num7;

			Type type = typeof(SetUVOnCubeToTerrainIndex);
			FieldInfo info = type.GetField("mpb", BindingFlags.NonPublic | BindingFlags.Static);
			object value = info.GetValue(null);

			MaterialPropertyBlock mpb = value as MaterialPropertyBlock;

			//TODO, Load original texture sheet as from game and use its height instead of hardcoding.

			mpb.Clear();
			mpb.SetVector("_MainTex_ST", new Vector4(0.0714285746f, 0.0357142873f / (TerrainUV.lnMatPiy / 4088), z, num7));
			mpb.SetVector("_BumpMap_ST", new Vector4(0.0714285746f, 0.0357142873f / (TerrainUV.lnMatPiy / 4088), z, num7));

			if (lRenderer.material.mainTexture.height != SegmentMeshCreator.instance.mMeshRenderer.materialHeight)
			{
				lRenderer.material.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
				lRenderer.material.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));
			}
			
			lRenderer.SetPropertyBlock(mpb);
		}
	}
}
