using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.HD
{
	// Token: 0x02000740 RID: 1856
	public class SetUVOnCubeToTerrainIndex_SD
	{
		// Token: 0x0600374D RID: 14157 RVA: 0x00239918 File Offset: 0x00237B18
		public static void StoreCubeMesh(GameObject cube)
		{
			MeshFilter component = cube.transform.GetComponent<MeshFilter>();
			SetUVOnCubeToTerrainIndex_SD.cubeMesh = component.mesh;
			MeshRenderer component2 = cube.transform.GetComponent<MeshRenderer>();
			SetUVOnCubeToTerrainIndex_SD.cubeMaterial = component2.material;
			SetUVOnCubeToTerrainIndex_SD.mpb = new MaterialPropertyBlock();

            if (cubeMaterial != null && TerrainStitcher.bOverrideSetUVCalls)
            {
                cubeMaterial.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
                cubeMaterial.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));
            }
		}

		// Token: 0x0600374E RID: 14158 RVA: 0x00239960 File Offset: 0x00237B60
		public static void SetCustom(CustomObjectDesign design, GameObject lCubeObject)
		{
			SegmentMeshCreator.instance.RenderCustomDesign(design, ref SetUVOnCubeToTerrainIndex.customMeshes, ref SetUVOnCubeToTerrainIndex.customMaterials);
			MeshFilter component = lCubeObject.transform.GetComponent<MeshFilter>();
			component.mesh = SetUVOnCubeToTerrainIndex.customMeshes[0];
			MeshRenderer component2 = lCubeObject.transform.GetComponent<MeshRenderer>();
			component2.material = SetUVOnCubeToTerrainIndex.customMaterials[0];
			SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", Color.white);
			component2.SetPropertyBlock(SetUVOnCubeToTerrainIndex_SD.mpb);
		}

		// Token: 0x0600374F RID: 14159 RVA: 0x002399D4 File Offset: 0x00237BD4
		public static void SetUVOnly(int cubeType, ushort cubeValue, GameObject lCubeObject)
		{
			int num = 128;
			int num2 = 146;
			int lnGutter = num2 - num;
			MeshFilter component = lCubeObject.GetComponent<MeshFilter>();
			Mesh mesh = component.mesh;
			Mesh mesh2 = component.mesh;
			if (mesh == null)
			{
				Debug.LogError("Mesh is null!");
			}
			Vector2[] array = mesh.uv;
			if (array == null)
			{
				Debug.LogError("theUVs is null!");
			}
			SetUVOnCubeToTerrainIndex_SD.SetFrontUV(cubeType, cubeValue, num2, lnGutter, ref array);
			SetUVOnCubeToTerrainIndex_SD.SetBackUV(cubeType, cubeValue, num2, lnGutter, ref array);
			SetUVOnCubeToTerrainIndex_SD.SetLeftUV(cubeType, cubeValue, num2, lnGutter, ref array);
			SetUVOnCubeToTerrainIndex_SD.SetRightUV(cubeType, cubeValue, num2, lnGutter, ref array);
			SetUVOnCubeToTerrainIndex_SD.SetBotUV(cubeType, cubeValue, num2, lnGutter, ref array);
			SetUVOnCubeToTerrainIndex_SD.SetTopUV(cubeType, cubeValue, num2, lnGutter, ref array);
			mesh.uv = array;
			if (mesh2 != component.mesh)
			{
				Debug.LogError("!");
			}
			array = null;
		}

		// Token: 0x06003750 RID: 14160 RVA: 0x00239AA4 File Offset: 0x00237CA4
		public static void SetUVAndMaterialBlock(int cubeType, ushort cubeValue, GameObject lCubeObject)
		{
			if (SetUVOnCubeToTerrainIndex_SD.mpb == null)
			{
				SetUVOnCubeToTerrainIndex_SD.mpb = new MaterialPropertyBlock();
			}
			if (CubeHelper.IsColorised(cubeType))
			{
				SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", PlayerPainter.FromValue(cubeValue));
			}
			else
			{
				SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", Color.white);
			}
			int lnTotalPix = 146;
			int lnGutter = 0;
			MeshFilter component = lCubeObject.GetComponent<MeshFilter>();
			MeshRenderer component2 = lCubeObject.GetComponent<MeshRenderer>();
			if (SetUVOnCubeToTerrainIndex_SD.cubeMaterial == null)
			{
				Debug.LogError("Error, cubeMaterial hasn't been assigned when setting Cube UVs!");
			}
			component2.material = SetUVOnCubeToTerrainIndex_SD.cubeMaterial;
			component2.SetPropertyBlock(SetUVOnCubeToTerrainIndex_SD.mpb);
			Mesh mesh = component.mesh;
			Vector2[] uv = mesh.uv;
			SetUVOnCubeToTerrainIndex_SD.SetFrontUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetBackUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetLeftUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetRightUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetBotUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetTopUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			mesh.uv = uv;
		}

		// Token: 0x06003751 RID: 14161 RVA: 0x00239BA0 File Offset: 0x00237DA0
		public static void SetUVAndOverrideMaterial(int cubeType, ushort cubeValue, GameObject lCubeObject)
		{
			if (CubeHelper.IsColorised(cubeType))
			{
				SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", PlayerPainter.FromValue(cubeValue));
			}
			else
			{
				SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", Color.white);
			}
			int num = 128;
			int num2 = 146;
			int lnGutter = num2 - num;
			MeshFilter component = lCubeObject.GetComponent<MeshFilter>();
			MeshRenderer component2 = lCubeObject.GetComponent<MeshRenderer>();
			component2.SetPropertyBlock(SetUVOnCubeToTerrainIndex_SD.mpb);
			Mesh mesh = component.mesh;
			Vector2[] uv = mesh.uv;
			SetUVOnCubeToTerrainIndex_SD.SetFrontUV(cubeType, cubeValue, num2, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetBackUV(cubeType, cubeValue, num2, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetLeftUV(cubeType, cubeValue, num2, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetRightUV(cubeType, cubeValue, num2, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetBotUV(cubeType, cubeValue, num2, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetTopUV(cubeType, cubeValue, num2, lnGutter, ref uv);
			mesh.uv = uv;
		}

		// Token: 0x06003752 RID: 14162 RVA: 0x00239C70 File Offset: 0x00237E70
		public static void SetUVAndOverrideStaticMesh(int cubeType, ushort cubeValue, GameObject lCubeObject)
		{
            if (SetUVOnCubeToTerrainIndex_SD.mpb == null)
            {
                SetUVOnCubeToTerrainIndex_SD.mpb = new MaterialPropertyBlock();
            }
			SetUVOnCubeToTerrainIndex_SD.mpb.Clear();
			if (CubeHelper.IsColorised(cubeType))
			{
				SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", PlayerPainter.FromValue(cubeValue));
			}
			else
			{
				SetUVOnCubeToTerrainIndex_SD.mpb.SetColor("_Color", Color.white);
			}
			int lnTotalPix = 146;
			int lnGutter = 16;
			MeshFilter component = lCubeObject.GetComponent<MeshFilter>();
			component.mesh = SetUVOnCubeToTerrainIndex_SD.cubeMesh;
			MeshRenderer component2 = lCubeObject.GetComponent<MeshRenderer>();
			if (SetUVOnCubeToTerrainIndex_SD.cubeMaterial == null)
			{
				Debug.LogError("Error, cubeMaterial hasn't been assigned when setting Cube UVs!");
			}
			component2.material = SetUVOnCubeToTerrainIndex_SD.cubeMaterial;
			component2.SetPropertyBlock(SetUVOnCubeToTerrainIndex_SD.mpb);
			Mesh mesh = component.mesh;
			Vector2[] uv = mesh.uv;
			if (uv == null)
			{
				Debug.LogError("Error, no UVs!?");
			}
			if (uv.Length == 0)
			{
				Debug.LogError(string.Concat(new object[]
				{
				"UVs only had length of ",
				uv.Length,
				":",
				lCubeObject.name
				}));
			}
			SetUVOnCubeToTerrainIndex_SD.SetFrontUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetBackUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetLeftUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetRightUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetBotUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			SetUVOnCubeToTerrainIndex_SD.SetTopUV(cubeType, cubeValue, lnTotalPix, lnGutter, ref uv);
			mesh.uv = uv;
		}

		// Token: 0x06003753 RID: 14163 RVA: 0x00239DC0 File Offset: 0x00237FC0
		public static void SetMaterialUV(Renderer lRenderer, int lnWhich, ushort lValue, bool lbDoBump)
		{
            if (SetUVOnCubeToTerrainIndex_SD.mpb == null)
            {
                SetUVOnCubeToTerrainIndex_SD.mpb = new MaterialPropertyBlock();
            }

			//Original DJ code, modified for MG

			int lnTotalPix = 146; //and-a-bit
            int lnGutter = 9;     //9;//ermlnTotalPix - lnTilePix;

            int lnTiles = 28; //DJ value is 14, our sheets are 2x the width
            int lnCubeId = TerrainData.GetSideTexture(lnWhich, lValue);

            int lnXTile = (lnCubeId % lnTiles);
            int lnYTile = (lnCubeId / lnTiles);

            int lnPixX = lnTotalPix * lnXTile;
            int lnPixY = lnTotalPix * lnYTile;

            //add in gutter so that we start in the top left of the inner 128 tile
            lnPixX += lnGutter;
            lnPixY -= lnGutter;
            lnPixY += lnTotalPix; //due to inversion when converting to UVs

            //convert pixels into UVs
            float lrX = lnPixX / TerrainUV.lnMatPix;
            float lrY = lnPixY / TerrainUV.lnMatPiy;

            lrY = (1.0f - lrY);
            mpb.Clear();

            float fNoGutter = 1.0f / 28.0f;
            float fGutter = (fNoGutter / lnTotalPix) * (lnTotalPix - (lnGutter * 2));

			mpb.SetVector("_MainTex_ST", new Vector4(fGutter, fGutter, lrX, lrY));
            mpb.SetVector("_BumpMap_ST", new Vector4(fGutter, fGutter, lrX, lrY));

            lRenderer.SetPropertyBlock(mpb);

            if (lRenderer?.material?.mainTexture?.height != SegmentMeshCreator.instance?.mMeshRenderer?.materialHeight ||
				lRenderer?.material?.mainTexture?.width != SegmentMeshCreator.instance?.mMeshRenderer?.materialWidth)
            {
                lRenderer.material.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
                lRenderer.material.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));
            }
		}

		// Token: 0x06003754 RID: 14164 RVA: 0x00239E84 File Offset: 0x00238084
		public static void SetFrontUV(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
		{
			int num = 28;
			int sideTexture = global::TerrainData.GetSideTexture(lnWhich, lValue);
			int num2 = sideTexture % num;
			int num3 = sideTexture / num;
			int num4 = lnTotalPix * num2;
			int num5 = lnTotalPix * num3;
			num4 += lnGutter / 2;
			num5 -= lnGutter / 2;
			num5 += lnTotalPix;
			float num6 = (float)num4 / lnMatPix;
			float num7 = (float)num5 / lnMatPiy;
			num7 = 1f - num7;
			float x = num6;
			float y = num7;
			float x2 = num6 + lrUVSizeX;
			float y2 = num7 + lrUVSizeY;
			theUVs[0] = new Vector2(x, y);
			theUVs[1] = new Vector2(x2, y);
			theUVs[2] = new Vector2(x, y2);
			theUVs[3] = new Vector2(x2, y2);
		}

		// Token: 0x06003755 RID: 14165 RVA: 0x00239F5C File Offset: 0x0023815C
		public static void SetBackUV(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
		{
			int num = 28;
			int sideTexture = global::TerrainData.GetSideTexture(lnWhich, lValue);
			int num2 = sideTexture % num;
			int num3 = sideTexture / num;
			int num4 = lnTotalPix * num2;
			int num5 = lnTotalPix * num3;
			num4 += lnGutter / 2;
			num5 -= lnGutter / 2;
			num5 += lnTotalPix;
			float num6 = (float)num4 / lnMatPix;
			float num7 = (float)num5 / lnMatPiy;
			num7 = 1f - num7;
			float x = num6;
			float y = num7;
			float x2 = num6 + lrUVSizeX;
			float y2 = num7 + lrUVSizeY;
			theUVs[6] = new Vector2(x, y);
			theUVs[7] = new Vector2(x2, y);
			theUVs[10] = new Vector2(x, y2);
			theUVs[11] = new Vector2(x2, y2);
		}

		// Token: 0x06003756 RID: 14166 RVA: 0x0023A038 File Offset: 0x00238238
		public static void SetLeftUV(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
		{
			int num = 28;
			int sideTexture = global::TerrainData.GetSideTexture(lnWhich, lValue);
			int num2 = sideTexture % num;
			int num3 = sideTexture / num;
			int num4 = lnTotalPix * num2;
			int num5 = lnTotalPix * num3;
			num4 += lnGutter / 2;
			num5 -= lnGutter / 2;
			num5 += lnTotalPix;
			float num6 = (float)num4 / lnMatPix;
			float num7 = (float)num5 / lnMatPiy;
			num7 = 1f - num7;
			float x = num6;
			float y = num7;
			float x2 = num6 + lrUVSizeX;
			float y2 = num7 + lrUVSizeY;
			theUVs[16] = new Vector2(x, y);
			theUVs[18] = new Vector2(x2, y);
			theUVs[19] = new Vector2(x, y2);
			theUVs[17] = new Vector2(x2, y2);
		}

		// Token: 0x06003757 RID: 14167 RVA: 0x0023A114 File Offset: 0x00238314
		public static void SetRightUV(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
		{
			int num = 28;
			int sideTexture = global::TerrainData.GetSideTexture(lnWhich, lValue);
			int num2 = sideTexture % num;
			int num3 = sideTexture / num;
			int num4 = lnTotalPix * num2;
			int num5 = lnTotalPix * num3;
			num4 += lnGutter / 2;
			num5 -= lnGutter / 2;
			num5 += lnTotalPix;
			float num6 = (float)num4 / lnMatPix;
			float num7 = (float)num5 / lnMatPiy;
			num7 = 1f - num7;
			float x = num6;
			float y = num7;
			float x2 = num6 + lrUVSizeX;
			float y2 = num7 + lrUVSizeY;
			theUVs[20] = new Vector2(x, y);
			theUVs[22] = new Vector2(x2, y);
			theUVs[23] = new Vector2(x, y2);
			theUVs[21] = new Vector2(x2, y2);
		}

		// Token: 0x06003758 RID: 14168 RVA: 0x0023A1F0 File Offset: 0x002383F0
		public static void SetTopUV(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
		{
			int num = 28;
			int topTexture = global::TerrainData.GetTopTexture(lnWhich, lValue);
			int num2 = topTexture % num;
			int num3 = topTexture / num;
			int num4 = lnTotalPix * num2;
			int num5 = lnTotalPix * num3;
			num4 += lnGutter / 2;
			num5 -= lnGutter / 2;
			num5 += lnTotalPix;
			float num6 = (float)num4 / lnMatPix;
			float num7 = (float)num5 / lnMatPiy;
			num7 = 1f - num7;
			float x = num6;
			float y = num7;
			float x2 = num6 + lrUVSizeX;
			float y2 = num7 + lrUVSizeY;
			theUVs[4] = new Vector2(x, y);
			theUVs[5] = new Vector2(x2, y);
			theUVs[8] = new Vector2(x, y2);
			theUVs[9] = new Vector2(x2, y2);
		}

		// Token: 0x06003759 RID: 14169 RVA: 0x0023A2CC File Offset: 0x002384CC
		public static void SetBotUV(int lnWhich, ushort lValue, int lnTotalPix, int lnGutter, ref Vector2[] theUVs)
		{
			int num = 28;
			int bottomTexture = global::TerrainData.GetBottomTexture(lnWhich, lValue);
			int num2 = bottomTexture % num;
			int num3 = bottomTexture / num;
			int num4 = lnTotalPix * num2;
			int num5 = lnTotalPix * num3;
			num4 += lnGutter / 2;
			num5 -= lnGutter / 2;
			num5 += lnTotalPix;
			float num6 = (float)num4 / lnMatPix;
			float num7 = (float)num5 / lnMatPiy;
			num7 = 1f - num7;
			float x = num6;
			float y = num7;
			float x2 = num6 + lrUVSizeX;
			float y2 = num7 + lrUVSizeY;
			theUVs[15] = new Vector2(x, y);
			theUVs[13] = new Vector2(x2, y);
			theUVs[12] = new Vector2(x, y2);
			theUVs[14] = new Vector2(x2, y2);
		}

		// Token: 0x04004D40 RID: 19776
		public static Mesh cubeMesh;

		// Token: 0x04004D41 RID: 19777
		public static Material cubeMaterial;

		// Token: 0x04004D44 RID: 19780
		public static MaterialPropertyBlock mpb;


        public static float lnMatPix = 4088f;

        // Token: 0x04004D6B RID: 19819
        public static float lnMatPiy = 4088f;

        // Token: 0x04004D6C RID: 19820
        private static int lnTilePix = 128;

        // Token: 0x04004D6D RID: 19821
        private static int lnTotalPix = 146;

        // Token: 0x04004D6E RID: 19822
        private static int lnGutter = lnTotalPix - lnTilePix;

        // Token: 0x04004D6F RID: 19823
        public static int lnTilesX = (int)(lnMatPix / (float)lnTotalPix);

        // Token: 0x04004D70 RID: 19824
        public static int lnTilesY = (int)(lnMatPiy / (float)lnTotalPix);

        // Token: 0x04004D71 RID: 19825
        public static float lrUVSizeX = 1f / (float)lnTilesX;

        // Token: 0x04004D72 RID: 19826
        public static float lrUVSizeY = 1f / (float)lnTilesY;
	}

}
