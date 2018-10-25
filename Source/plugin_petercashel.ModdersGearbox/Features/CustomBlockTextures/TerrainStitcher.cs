using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures
{
	public static class TerrainStitcher
	{
		private static bool mbHasRun = false;

		internal static void Build()
		{
			if (!mbHasRun)
			{
				mbHasRun = true;
				LoadAndExtendTextureSheets();
				SetTerrainTextures();
				FindAllTextures();
				StitchTexturesAndAssignIDs();
				SetTerrainTextures();

				//try
				//{
				//	//Save textures out.

				//	// Encode texture into PNG
				//	byte[] bytes = diffuseTexture.EncodeToPNG();

				//	mDataSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "ModdersGearbox";
				//	if (!System.IO.Directory.Exists(mDataSavePath))
				//	{
				//		System.IO.Directory.CreateDirectory(mDataSavePath);
				//	}

				//	mDataSavePath += Path.DirectorySeparatorChar;

				//	File.WriteAllBytes(mDataSavePath + "Terrain_" + 0 + ".png",
				//		bytes);


				//	// Encode texture into PNG
				//	bytes = normalTexture.EncodeToPNG();

				//	mDataSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "ModdersGearbox";
				//	if (!System.IO.Directory.Exists(mDataSavePath))
				//	{
				//		System.IO.Directory.CreateDirectory(mDataSavePath);
				//	}

				//	mDataSavePath += Path.DirectorySeparatorChar;

				//	File.WriteAllBytes(mDataSavePath + "Terrain_" + 1 + ".png",
				//		bytes);
				//}
				//catch
				//{

				//}
			}
			else
			{
				//We may run multiple times while it waits for us to be done, so we dont
				//Maybe start a thread and run this there so we don't block, but then the game might load before we have changed terrain.
			}
			
		}

		private static Texture2D diffuseTexture;
		private static Texture2D normalTexture;

		private static void SetTerrainTextures()
		{
			if (diffuseTexture == null || normalTexture == null)
			{
				return;
			}

			SegmentMeshCreator.instance.segmentMaterial.mainTexture = diffuseTexture;
			SegmentMeshCreator.instance.segmentMaterial.SetTexture("_BumpMap", normalTexture);
			Debug.LogWarning("Assigning Diffuse and Normal maps to Terrain!");

			//And fix everything.

			Type type = typeof(SetUVOnCubeToTerrainIndex);
			FieldInfo info = type.GetField("cubeMaterial", BindingFlags.NonPublic | BindingFlags.Static);
			object value = info.GetValue(null);

			Material material = value as Material;

			SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord = new TerrainUVCoord(SegmentMeshCreator.instance.segmentMaterial.mainTexture.width, SegmentMeshCreator.instance.segmentMaterial.mainTexture.height, 128, 128, 9);
			TerrainUV.lnMatPiy = SegmentMeshCreator.instance.segmentMaterial.mainTexture.height;
			
			//TerrainUV.lnTilesY = (int)(TerrainUV.lnMatPiy / (float)TerrainUV.lnTotalPix);
			float tmp = (TerrainUV.lnMatPiy / (float)146);
			TerrainUV.lnTilesY = (int)tmp;

			SegmentMeshCreator.instance.currentMaterialHeight = (float)SegmentMeshCreator.instance.segmentMaterial.mainTexture.height;

			CurrentBlockMaterialUpdate.instance.CurrentBlockMat.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
			CurrentBlockMaterialUpdate.instance.CurrentBlockMat.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));

			MeshRenderer component2 = SpawnableObjectManagerScript.instance.maSpawnableObjects[(int)SpawnableObjectEnum.ItemCube].transform.GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				component2.material.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
				component2.material.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));
			}

			if (material != null)
			{
				material.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
				material.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));
				info.SetValue(null, material);
			}
		}

		private static void LoadAndExtendTextureSheets()
		{
			//throw new NotImplementedException();
			Texture2D diffuseTmp = null;
			Texture2D normalTmp = null;
			
			//Get Textures from mods
			{
				foreach (ModConfiguration modConfiguration in ModManager.mModConfigurations.Mods)
				{
					string text = Path.Combine(modConfiguration.Path, "Textures");
					if (Directory.Exists(text))
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(text);
						if (directoryInfo.GetFiles().Length != 0)
						{
							if (directoryInfo.GetFiles().Length < 2)
							{
							}
							else
							{
								string text2 = text + Path.DirectorySeparatorChar.ToString() + "terrain-diffuse.png";
								string text3 = text + Path.DirectorySeparatorChar.ToString() + "terrain-normal.png";
								if (File.Exists(text2) && File.Exists(text3))
								{
									diffuseTmp = ModManager.LoadPNG(text2);
									normalTmp = ModManager.LoadPNG(text3);
									break;
								}
							}
						}
					}
				}
			}

			if (diffuseTmp == null || normalTmp == null)
			{
				//try to get textures back from the game.
				RenderTexture originalRenderTexture = RenderTexture.active;

				try
				{
					Texture2D tmp = SegmentMeshCreator.instance.segmentMaterial.mainTexture as Texture2D;
					
					if (tmp != null)
					{

						int h = tmp.height;
						int w = tmp.width;

						RenderTexture.active = new RenderTexture(w, h, 32);
						Graphics.Blit(tmp, RenderTexture.active);
						
						diffuseTmp = new Texture2D(w, h * 2, tmp.format, true);
						Graphics.CopyTexture(tmp, 0, 0, 0, 0, tmp.width, tmp.height, diffuseTmp, 0, 0, 0, tmp.height);
						diffuseTmp.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, tmp.height);
						diffuseTmp.Apply();

						RenderTexture.active = originalRenderTexture;
					}

					tmp = SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap") as Texture2D;

					if (tmp != null)
					{

						int h = tmp.height;
						int w = tmp.width;

						RenderTexture.active = new RenderTexture(w, h, 32);
						Graphics.Blit(tmp, RenderTexture.active);

						normalTmp = new Texture2D(w, h * 2, tmp.format, true);
						Paint(normalTmp, new Color(0.5f, 0.5f, 0.5f, 0.5f));

						Graphics.CopyTexture(tmp, 0, 0, 0, 0, tmp.width, tmp.height, normalTmp, 0, 0, 0, tmp.height);
						normalTmp.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, tmp.height);
						normalTmp.Apply();

						RenderTexture.active = originalRenderTexture;
					}

				}
				catch (Exception ex)
				{

				}

			}

			if (diffuseTmp != null && normalTmp != null)
			{
				diffuseTexture = diffuseTmp;
				normalTexture = normalTmp;
			}
			
		}

		private static void Paint(Texture2D normalTmp, Color color)
		{
			for (int x = 0; x < normalTmp.width; x++)
			{
				for (int y = 0; y < normalTmp.height; y++)
				{
					normalTmp.SetPixel(x,y,color);
				}
			}
		}

		public static Dictionary<string, string> DiffuseTexturesAll = new Dictionary<string, string>();
		public static Dictionary<string, string> DiffuseTexturesTop = new Dictionary<string, string>();
		public static Dictionary<string, string> DiffuseTexturesSide = new Dictionary<string, string>();
		public static Dictionary<string, string> DiffuseTexturesBottom = new Dictionary<string, string>();

		public static Dictionary<string, string> NormalTexturesAll = new Dictionary<string, string>();
		public static Dictionary<string, string> NormalTexturesTop = new Dictionary<string, string>();
		public static Dictionary<string, string> NormalTexturesSide = new Dictionary<string, string>();
		public static Dictionary<string, string> NormalTexturesBottom = new Dictionary<string, string>();
		private static string mDataSavePath;

		private static void FindAllTextures()
		{
			foreach (ModConfiguration modConfiguration in ModManager.mModConfigurations.Mods)
			{
				string modPath = Path.Combine(modConfiguration.Path, "BlockTextures");
				if (Directory.Exists(modPath))
				{
					//Diffuse \\  

					string pathDiffuse = Path.Combine(modPath, "Diffuse");

					if(Directory.Exists(pathDiffuse))
					{
						foreach (string path2 in Directory.GetFiles(pathDiffuse, "*.png"))
						{
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path2);

							string key = fileNameWithoutExtension;

							if (fileNameWithoutExtension.ToLower().Contains("_top"))
							{
								key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 4);

								if (DiffuseTexturesTop.ContainsKey(key))
								{
									DiffuseTexturesTop.Remove(key);
								}

								DiffuseTexturesTop.Add(key, path2);
							}
							else if (fileNameWithoutExtension.ToLower().Contains("_side"))
							{
								key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 5);

								if (DiffuseTexturesSide.ContainsKey(key))
								{
									DiffuseTexturesSide.Remove(key);
								}
								
								DiffuseTexturesSide.Add(key, path2);
							}
							else if (fileNameWithoutExtension.ToLower().Contains("_bottom"))
							{
								key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 7);

								if (DiffuseTexturesBottom.ContainsKey(key))
								{
									DiffuseTexturesBottom.Remove(key);
								}

								DiffuseTexturesBottom.Add(key, path2);
							}
							else
							{
								if (DiffuseTexturesAll.ContainsKey(key))
								{
									DiffuseTexturesAll.Remove(key);
								}
								
								DiffuseTexturesAll.Add(fileNameWithoutExtension, path2);
							}
						}
					}
					

					string pathNormal = Path.Combine(modPath, "Normal");

					if (Directory.Exists(pathNormal))
					{
						foreach (string path2 in Directory.GetFiles(pathNormal, "*.png"))
						{
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path2);
							string key = fileNameWithoutExtension;

							if (fileNameWithoutExtension.ToLower().Contains("_top"))
							{
								key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 4);

								if (NormalTexturesTop.ContainsKey(key))
								{
									NormalTexturesTop.Remove(key);
								}

								NormalTexturesTop.Add(key, path2);
							}
							else if (fileNameWithoutExtension.ToLower().Contains("_side"))
							{
								key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 5);

								if (NormalTexturesSide.ContainsKey(key))
								{
									NormalTexturesSide.Remove(key);
								}

								NormalTexturesSide.Add(key, path2);
							}
							else if (fileNameWithoutExtension.ToLower().Contains("_bottom"))
							{
								key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 7);

								if (NormalTexturesBottom.ContainsKey(key))
								{
									NormalTexturesBottom.Remove(key);
								}

								NormalTexturesBottom.Add(key, path2);
							}
							else
							{
								if (NormalTexturesAll.ContainsKey(key))
								{
									NormalTexturesAll.Remove(key);
								}

								NormalTexturesAll.Add(fileNameWithoutExtension, path2);
							}
						}
					}
				}
			}
		}

		public static int NextTextureSpriteID = 392;

		private static void StitchTexturesAndAssignIDs()
		{
			//SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite() //Does this get the right rect?

			//throw new NotImplementedException();

			ProcessTextures(DiffuseTexturesAll, TextureType.Diffuse, TextureSide.All);
			ProcessTextures(DiffuseTexturesTop, TextureType.Diffuse, TextureSide.Top);
			ProcessTextures(DiffuseTexturesSide, TextureType.Diffuse, TextureSide.Side);
			ProcessTextures(DiffuseTexturesBottom, TextureType.Diffuse, TextureSide.Bottom);

			ProcessTextures(NormalTexturesAll, TextureType.Normal, TextureSide.All);
			ProcessTextures(NormalTexturesTop, TextureType.Normal, TextureSide.Top);
			ProcessTextures(NormalTexturesSide, TextureType.Normal, TextureSide.Side);
			ProcessTextures(NormalTexturesBottom, TextureType.Normal, TextureSide.Bottom);

			diffuseTexture.Apply();
			normalTexture.Apply();
		}

		static void ProcessTextures(Dictionary<string, string> dictionary, TextureType textureType, TextureSide textureSide)
		{
			bool bNeedsNewID = false;
			int slot = -1;
			foreach (string key in dictionary.Keys)
			{
				bNeedsNewID = key.Contains(".");

				//Entry to set.
				TerrainDataEntry entry = TerrainData.mEntriesByKey[key];

				if (bNeedsNewID)
				{
					slot = NextTextureSpriteID++;
				}
				else
				{
					switch (textureSide)
					{
						case TextureSide.All:
							{
								slot = entry.TopTexture;
								break;
							}
						case TextureSide.Top:
							{
								slot = entry.TopTexture;
								break;
							}
						case TextureSide.Side:
							{
								slot = entry.SideTexture;
								break;
							}
						case TextureSide.Bottom:
							{
								slot = entry.BottomTexture;
								break;
							}
					}
				}

				Rect target = SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite(slot);

				Texture2D newSprite = LoadPNG(dictionary[key], 146, 146);

				Texture2D targetTexture = (textureType == TextureType.Diffuse ? diffuseTexture : normalTexture);

				int ypos = (targetTexture.height) - ((int)target.y - 9);

				//Correction.
				ypos = ypos - 146;

				targetTexture.SetPixels((int)target.x - 9, ypos, 146, 146, newSprite.GetPixels());


				if (bNeedsNewID)
				{
					//Time to go set IDs
					switch (textureSide)
					{
						case TextureSide.All:
							{
								entry.TopTexture = slot;
								entry.SideTexture = slot;
								entry.BottomTexture = slot;
								break;
							}
						case TextureSide.Top:
							{
								entry.TopTexture = slot;
								break;
							}
						case TextureSide.Side:
							{
								entry.SideTexture = slot;
								break;
							}
						case TextureSide.Bottom:
							{
								entry.BottomTexture = slot;
								break;
							}
					}
				}
			}

		}

		private static Texture2D LoadPNG(string v1, int v2, int v3)
		{
			Texture2D texture2D = null;
			if (File.Exists(v1))
			{
				byte[] data = File.ReadAllBytes(v1);
				texture2D = new Texture2D(v2, v3);
				texture2D.LoadImage(data);
			}

			return texture2D;
		}

		public enum TextureType
		{
			Diffuse,
			Normal
		}

		public enum TextureSide
		{
			All,
			Top,
			Side,
			Bottom,
		}
	}
}
