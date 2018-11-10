﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using plugin_petercashel_ModdersGearbox.Support.Image;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures
{
    public static class TerrainStitcher
    {
        #region  Fields and Properties

        static bool mbHasRun = false;

        static Texture2D mDiffuseTexture;
        static Texture2D mNormalTexture;

		//Key, File
        public static Dictionary<string, ModTextureEntry> DiffuseTexturesAll = new Dictionary<string, ModTextureEntry>();
        public static Dictionary<string, ModTextureEntry> DiffuseTexturesTop = new Dictionary<string, ModTextureEntry>();
        public static Dictionary<string, ModTextureEntry> DiffuseTexturesSide = new Dictionary<string, ModTextureEntry>();
        public static Dictionary<string, ModTextureEntry> DiffuseTexturesBottom = new Dictionary<string, ModTextureEntry>();

        public static Dictionary<string, ModTextureEntry> NormalTexturesAll = new Dictionary<string, ModTextureEntry>();
        public static Dictionary<string, ModTextureEntry> NormalTexturesTop = new Dictionary<string, ModTextureEntry>();
        public static Dictionary<string, ModTextureEntry> NormalTexturesSide = new Dictionary<string, ModTextureEntry>();
        public static Dictionary<string, ModTextureEntry> NormalTexturesBottom = new Dictionary<string, ModTextureEntry>();


		//Ore Support - Key, Stage, File
        public static Dictionary<string, Dictionary<int, ModTextureEntry>> DiffuseTexturesOreStages = new Dictionary<string, Dictionary<int, ModTextureEntry>>();
        public static Dictionary<string, Dictionary<int, ModTextureEntry>> NormalTexturesOreStages = new Dictionary<string, Dictionary<int, ModTextureEntry>>();

		public static int NextTextureSpriteId = 392;
        public static TextureMode TextureDefinition = TextureMode.SD;
		public static bool bOverrideSetUVCalls = false;
        public static bool bSwitchToHD = false;

		#endregion

		#region  Methods

		internal static void Build()
        {
            if (!mbHasRun)
            {
                mbHasRun = true;

                if (WorldScript.mbDedicated)
                {
                    return;
                }

                LoadAndExtendTextureSheets();
				SetTerrainTextures();
                
                bOverrideSetUVCalls = true;

				FindAllTextures();

                if (bSwitchToHD && TextureDefinition == TextureMode.SD)
                {
                    TextureDefinition = TextureMode.HD;
					TextureScale.Point(mDiffuseTexture, mDiffuseTexture.width * 2, mDiffuseTexture.height * 2);
                    TextureScale.Point(mNormalTexture, mNormalTexture.width * 2, mNormalTexture.height * 2);
                    SetTerrainTextures();
				}

                StitchTexturesAndAssignIDs();
                SetTerrainTextures();

                PurgeTempTexture();
			}
        }

        static void PurgeTempTexture()
        {
            foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in DiffuseTexturesAll)
            {
				UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            DiffuseTexturesAll.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in DiffuseTexturesBottom)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            DiffuseTexturesBottom.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in DiffuseTexturesSide)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            DiffuseTexturesSide.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in DiffuseTexturesTop)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            DiffuseTexturesTop.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in NormalTexturesAll)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            NormalTexturesAll.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in NormalTexturesBottom)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            NormalTexturesBottom.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in NormalTexturesSide)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            NormalTexturesSide.Clear();

			foreach (KeyValuePair<string, ModTextureEntry> modTextureEntry in NormalTexturesTop)
            {
                UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
			}
            NormalTexturesTop.Clear();
            
			foreach (KeyValuePair<string, Dictionary<int, ModTextureEntry>> diffuseTexturesOreStage in DiffuseTexturesOreStages)
            {
                foreach (KeyValuePair<int, ModTextureEntry> modTextureEntry in diffuseTexturesOreStage.Value)
                {
					UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
				}
                diffuseTexturesOreStage.Value.Clear();
            }
            DiffuseTexturesOreStages.Clear();

			foreach (KeyValuePair<string, Dictionary<int, ModTextureEntry>> normalTexturesOreStage in NormalTexturesOreStages)
            {
				foreach (KeyValuePair<int, ModTextureEntry> modTextureEntry in normalTexturesOreStage.Value)
                {
                    UnityEngine.Object.Destroy(modTextureEntry.Value.texture);
				}
                normalTexturesOreStage.Value.Clear();
			}
            NormalTexturesOreStages.Clear();
		}

		static void ExportTextures()
        {
            try
            {
                //Save textures out.

                // Encode texture into PNG
                byte[] bytes = mDiffuseTexture.EncodeToPNG();

                var dataSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "ModdersGearbox";
                if (!Directory.Exists(dataSavePath))
                {
                    Directory.CreateDirectory(dataSavePath);
                }

                dataSavePath += Path.DirectorySeparatorChar;

                File.WriteAllBytes(dataSavePath + "Terrain_" + 0 + ".png", bytes);


                // Encode texture into PNG
                bytes = mNormalTexture.EncodeToPNG();

                dataSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "ModdersGearbox";
                if (!Directory.Exists(dataSavePath))
                {
                    Directory.CreateDirectory(dataSavePath);
                }

                dataSavePath += Path.DirectorySeparatorChar;

                File.WriteAllBytes(dataSavePath + "Terrain_" + 1 + ".png", bytes);
            }
            catch
            {
            }
        }

        static void SetTerrainTextures()
        {
            if (mDiffuseTexture == null || mNormalTexture == null)
            {
                return;
            }

            SegmentMeshCreator.instance.segmentMaterial.mainTexture = mDiffuseTexture;
            SegmentMeshCreator.instance.segmentMaterial.SetTexture("_BumpMap", mNormalTexture);
            Debug.LogWarning("Assigning Diffuse and Normal maps to Terrain!");

            //And fix everything.

            Type type = typeof(SetUVOnCubeToTerrainIndex);
            FieldInfo info = type.GetField("cubeMaterial", BindingFlags.NonPublic | BindingFlags.Static);
            object value = info.GetValue(null);

            var material = value as Material;

            if (TextureDefinition == TextureMode.SD)
            {
                SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord = new TerrainUVCoord(SegmentMeshCreator.instance.segmentMaterial.mainTexture.width, SegmentMeshCreator.instance.segmentMaterial.mainTexture.height, 128, 128, 9);
			}
            else
            {
                SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord = new TerrainUVCoord(SegmentMeshCreator.instance.segmentMaterial.mainTexture.width, SegmentMeshCreator.instance.segmentMaterial.mainTexture.height, 256, 256, 18);
			}

            TerrainUV.lnMatPix = SegmentMeshCreator.instance.segmentMaterial.mainTexture.width;
			TerrainUV.lnMatPiy = SegmentMeshCreator.instance.segmentMaterial.mainTexture.height;

            //TODO WARNING ALERT
            //I HAVE NO FUCKING CLUE HERE
            //THAT IS ALL

            //TerrainUV.lnTilesY = (int)(TerrainUV.lnMatPiy / (float)TerrainUV.lnTotalPix);
            if (TextureDefinition == TextureMode.SD)
            {
                var tmp = TerrainUV.lnMatPix / 146;
                TerrainUV.lnTilesX = (int)tmp;

				tmp = TerrainUV.lnMatPiy / 146;
                TerrainUV.lnTilesY = (int)tmp;
			}
            else
            {
                var tmp = TerrainUV.lnMatPix / 292;
                TerrainUV.lnTilesX = (int)tmp;

				tmp = TerrainUV.lnMatPiy / 292;
                TerrainUV.lnTilesY = (int)tmp;
			}


            SegmentMeshCreator.instance.currentMaterialWidth = SegmentMeshCreator.instance.segmentMaterial.mainTexture.width;
			SegmentMeshCreator.instance.currentMaterialHeight = SegmentMeshCreator.instance.segmentMaterial.mainTexture.height;

            CurrentBlockMaterialUpdate.instance.CurrentBlockMat.mainTexture = SegmentMeshCreator.instance.segmentMaterial.mainTexture;
            CurrentBlockMaterialUpdate.instance.CurrentBlockMat.SetTexture("_BumpMap", SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap"));

            var component2 = SpawnableObjectManagerScript.instance.maSpawnableObjects[(int)SpawnableObjectEnum.ItemCube].transform.GetComponent<MeshRenderer>();
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

        static void LoadAndExtendTextureSheets()
        {
            //throw new NotImplementedException();
            Texture2D diffuseTmp = null;
            Texture2D normalTmp = null;

            //Get Textures from mods
            {
                foreach (ModConfiguration modConfiguration in ModManager.mModConfigurations.Mods)
                {
                    var text = Path.Combine(modConfiguration.Path, "Textures");
                    if (Directory.Exists(text))
                    {
                        var directoryInfo = new DirectoryInfo(text);
                        if (directoryInfo.GetFiles().Length != 0)
                        {
                            if (directoryInfo.GetFiles().Length < 2)
                            {
                            }
                            else
                            {
                                var text2 = text + Path.DirectorySeparatorChar + "terrain-diffuse.png";
                                var text3 = text + Path.DirectorySeparatorChar + "terrain-normal.png";
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
                    var tmp = SegmentMeshCreator.instance.segmentMaterial.mainTexture as Texture2D;
                    RenderTexture rt = null;
                    if (tmp != null)
                    {
                        var h = tmp.height;
                        var w = tmp.width;
                        rt = new RenderTexture(w, h, 32);

                        RenderTexture.active = rt;
                        Graphics.Blit(tmp, RenderTexture.active);

                        diffuseTmp = new Texture2D(w, h, tmp.format, true);
                        Paint(diffuseTmp, new Color(0f, 0f, 0f));

						diffuseTmp.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                        diffuseTmp.Apply();

                        RenderTexture.active = originalRenderTexture;
                        UnityEngine.Object.Destroy(rt);
                    }

                    tmp = SegmentMeshCreator.instance.segmentMaterial.GetTexture("_BumpMap") as Texture2D;

                    if (tmp != null)
                    {
                        var h = tmp.height;
                        var w = tmp.width;
                        rt = new RenderTexture(w, h, 32);

                        RenderTexture.active = rt;
                        Graphics.Blit(tmp, RenderTexture.active);

                        normalTmp = new Texture2D(w, h, tmp.format, true);
                        Paint(normalTmp, new Color(0.5f, 0.5f, 0.5f, 0.5f));

                        normalTmp.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                        normalTmp.Apply();

                        RenderTexture.active = originalRenderTexture;
						UnityEngine.Object.Destroy(rt);
					}
                }
                catch (Exception ex)
                {
                }
            }

            if (diffuseTmp.width == 2044)
            {
                TextureDefinition = TextureMode.SD;
                mDiffuseTexture = new Texture2D(4088, 4088, diffuseTmp.format, false);
                mNormalTexture = new Texture2D(4088, 4088, normalTmp.format, false);
			}
            else
            {
                TextureDefinition = TextureMode.HD;
                mDiffuseTexture = new Texture2D(8176, 8176, diffuseTmp.format, false);
                mNormalTexture = new Texture2D(8176, 8176, normalTmp.format, false);
			}

			/// TODO
			/// 
			/// Process 14 wide sheet to 28 wide
			/// Create new sheet,
			/// fpr 0-392
			/// Read from sheet with SetUVOnCubeToTerrainIndex, write with SetUVOnCubeToTerrainIndex_SD or SetUVOnCubeToTerrainIndex_HD
			/// See Stitching Code

            TerrainUVCoord uvCoord;

            if (TextureDefinition == TextureMode.SD)
            {
                uvCoord = new TerrainUVCoord(mDiffuseTexture.width, mDiffuseTexture.height, 128, 128, 9);
			}
            else
            {
				uvCoord = new TerrainUVCoord(mDiffuseTexture.width, mDiffuseTexture.height, 256, 256, 18);
			}


			for (int i = 0; i < 392; i++)
            {
                try
                {
                    //Diffuse
                    if (TextureDefinition == TextureMode.SD)
                    {
                        const int TextureSize = 146;
                        const int MagicNumber = 9;

                        Rect sourceTarget = SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite(i);
                        Rect Target = uvCoord.GetSprite(i);

                        var ypos = diffuseTmp.height - ((int)sourceTarget.y - MagicNumber);
                        ypos = ypos - TextureSize;

                        var ypos2 = mDiffuseTexture.height - ((int)Target.y - MagicNumber);
                        ypos2 = ypos2 - TextureSize;

                        Color[] colors = diffuseTmp.GetPixels((int)sourceTarget.x - MagicNumber, ypos, TextureSize, TextureSize);

						mDiffuseTexture.SetPixels((int)Target.x - MagicNumber, ypos2, TextureSize, TextureSize, colors);
                    }
                    else
                    {
                        const int TextureSize = 292;
                        const int MagicNumber = 18;

                        Rect sourceTarget = SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite(i);
                        Rect Target = uvCoord.GetSprite(i);

                        var ypos = diffuseTmp.height - ((int)sourceTarget.y - MagicNumber);
                        ypos = ypos - TextureSize;

                        var ypos2 = mDiffuseTexture.height - ((int)Target.y - MagicNumber);
                        ypos2 = ypos2 - TextureSize;

                        Color[] colors = diffuseTmp.GetPixels((int)sourceTarget.x - MagicNumber, ypos, TextureSize, TextureSize);

						mDiffuseTexture.SetPixels((int)Target.x - MagicNumber, ypos2, TextureSize, TextureSize, colors);
                    }

                    //Normals
                    if (TextureDefinition == TextureMode.SD)
                    {
                        const int TextureSize = 146;
                        const int MagicNumber = 9;

                        Rect sourceTarget = SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite(i);
                        Rect Target = uvCoord.GetSprite(i);

                        var ypos = normalTmp.height - ((int)sourceTarget.y - MagicNumber);
                        ypos = ypos - TextureSize;

                        var ypos2 = mNormalTexture.height - ((int)Target.y - MagicNumber);
                        ypos2 = ypos2 - TextureSize;

                        Color[] colors = normalTmp.GetPixels((int)sourceTarget.x - MagicNumber, ypos, TextureSize, TextureSize);

                        mNormalTexture.SetPixels((int)Target.x - MagicNumber, ypos2, TextureSize, TextureSize, colors);
                    }
                    else
                    {
                        const int TextureSize = 292;
                        const int MagicNumber = 18;

                        Rect sourceTarget = SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite(i);
                        Rect Target = uvCoord.GetSprite(i);

                        var ypos = normalTmp.height - ((int)sourceTarget.y - MagicNumber);
                        ypos = ypos - TextureSize;

                        var ypos2 = mNormalTexture.height - ((int)Target.y - MagicNumber);
                        ypos2 = ypos2 - TextureSize;

                        Color[] colors = normalTmp.GetPixels((int)sourceTarget.x - MagicNumber, ypos, TextureSize, TextureSize);
                        
                        mNormalTexture.SetPixels((int)Target.x - MagicNumber, ypos2, TextureSize, TextureSize, colors);
					}
                }
                catch
                {
                    // Later, We will care
                    // For now, until the math is right, we dont.
                }
			}

            mDiffuseTexture.Apply();
            mNormalTexture.Apply();

			//if (diffuseTmp != null && normalTmp != null)
			//{
			//    diffuseTmp.Apply(true, false);
			//    normalTmp.Apply(true, false);

			//    mDiffuseTexture = diffuseTmp;
			//    mNormalTexture = normalTmp;
			//}
		}

		static void Paint(Texture2D texture2D, Color color)
        {
            for (var x = 0; x < texture2D.width; x++)
            {
                for (var y = 0; y < texture2D.height; y++)
                {
                    texture2D.SetPixel(x, y, color);
                }
            }
        }

        static void FindAllTextures()
        {
            foreach (ModConfiguration modConfiguration in ModManager.mModConfigurations.Mods)
            {
                var modPath = Path.Combine(modConfiguration.Path, "BlockTextures");
                if (Directory.Exists(modPath))
                {
                    //Diffuse \\  

                    var pathDiffuse = Path.Combine(modPath, "Diffuse");

                    if (Directory.Exists(pathDiffuse))
                    {
                        foreach (var path2 in Directory.GetFiles(pathDiffuse, "*.png"))
                        {
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path2);

                            var key = fileNameWithoutExtension;

                            if (fileNameWithoutExtension.ToLower().Contains("_top"))
                            {
                                key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 4);

                                if (DiffuseTexturesTop.ContainsKey(key))
                                {
                                    DiffuseTexturesTop.Remove(key);
                                }

                                DiffuseTexturesTop.Add(key, new ModTextureEntry(path2));
                            }
                            else if (fileNameWithoutExtension.ToLower().Contains("_side"))
                            {
                                key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 5);

                                if (DiffuseTexturesSide.ContainsKey(key))
                                {
                                    DiffuseTexturesSide.Remove(key);
                                }

                                DiffuseTexturesSide.Add(key, new ModTextureEntry(path2));
                            }
                            else if (fileNameWithoutExtension.ToLower().Contains("_bottom"))
                            {
                                key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 7);

                                if (DiffuseTexturesBottom.ContainsKey(key))
                                {
                                    DiffuseTexturesBottom.Remove(key);
                                }

                                DiffuseTexturesBottom.Add(key, new ModTextureEntry(path2));
                            }
                            else
                            {
                                if (fileNameWithoutExtension.ToLower().Contains("_stage"))
                                {
                                    key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.IndexOf("_stage"));

                                    if (!DiffuseTexturesOreStages.ContainsKey(key))
                                    {
                                        DiffuseTexturesOreStages.Add(key, new Dictionary<int, ModTextureEntry>());
									}

                                    int start = fileNameWithoutExtension.IndexOf("_stage") + 6;
									int stage = Int32.Parse(fileNameWithoutExtension.Substring(start, fileNameWithoutExtension.Length - start));

                                    var stages = DiffuseTexturesOreStages[key];

                                    if (stages.ContainsKey(stage))
                                    {
                                        stages.Remove(stage);
									}
                                    stages.Add(stage, new ModTextureEntry(path2));

								}
                                else
                                {
                                    if (DiffuseTexturesAll.ContainsKey(key))
                                    {
                                        DiffuseTexturesAll.Remove(key);
                                    }

                                    DiffuseTexturesAll.Add(fileNameWithoutExtension, new ModTextureEntry(path2));
								}
                            }
                        }
                    }


                    var pathNormal = Path.Combine(modPath, "Normal");

                    if (Directory.Exists(pathNormal))
                    {
                        foreach (var path2 in Directory.GetFiles(pathNormal, "*.png"))
                        {
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path2);
                            var key = fileNameWithoutExtension;

                            if (fileNameWithoutExtension.ToLower().Contains("_top"))
                            {
                                key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 4);

                                if (NormalTexturesTop.ContainsKey(key))
                                {
                                    NormalTexturesTop.Remove(key);
                                }

                                NormalTexturesTop.Add(key, new ModTextureEntry(path2));
                            }
                            else if (fileNameWithoutExtension.ToLower().Contains("_side"))
                            {
                                key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 5);

                                if (NormalTexturesSide.ContainsKey(key))
                                {
                                    NormalTexturesSide.Remove(key);
                                }

                                NormalTexturesSide.Add(key, new ModTextureEntry(path2));
                            }
                            else if (fileNameWithoutExtension.ToLower().Contains("_bottom"))
                            {
                                key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - 7);

                                if (NormalTexturesBottom.ContainsKey(key))
                                {
                                    NormalTexturesBottom.Remove(key);
                                }

                                NormalTexturesBottom.Add(key, new ModTextureEntry(path2));
                            }
                            else
                            {
                                if (fileNameWithoutExtension.ToLower().Contains("_stage"))
                                {
                                    key = fileNameWithoutExtension.Remove(fileNameWithoutExtension.IndexOf("_stage"));

                                    if (!NormalTexturesOreStages.ContainsKey(key))
                                    {
                                        NormalTexturesOreStages.Add(key, new Dictionary<int, ModTextureEntry>());
                                    }

                                    int start = fileNameWithoutExtension.IndexOf("_stage") + 6;
                                    int stage = Int32.Parse(fileNameWithoutExtension.Substring(start, fileNameWithoutExtension.Length - start));

                                    var stages = NormalTexturesOreStages[key];

                                    if (stages.ContainsKey(stage))
                                    {
                                        stages.Remove(stage);
                                    }
                                    stages.Add(stage, new ModTextureEntry(path2));

                                }
                                else
								{
                                    if (NormalTexturesAll.ContainsKey(key))
                                    {
                                        NormalTexturesAll.Remove(key);
                                    }

                                    NormalTexturesAll.Add(fileNameWithoutExtension, new ModTextureEntry(path2));
                                }
                            }
                        }
                    }
                }
            }
        }

        static void StitchTexturesAndAssignIDs()
        {
            //SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite() //Does this get the right rect?

            //throw new NotImplementedException();

            ProcessTextures(DiffuseTexturesAll, NormalTexturesAll, TextureSide.All);
            ProcessTextures(DiffuseTexturesTop, NormalTexturesTop, TextureSide.Top);
            ProcessTextures(DiffuseTexturesSide, NormalTexturesSide, TextureSide.Side);
            ProcessTextures(DiffuseTexturesBottom, NormalTexturesBottom, TextureSide.Bottom);


            ProcessOreTextures(DiffuseTexturesOreStages, NormalTexturesOreStages);



            mDiffuseTexture.Apply();
			mNormalTexture.Apply();
            
            #if DEBUG
			ExportTextures();
            #endif

			//Finalise to the GPU. From here on, We cannot read it anymore.
			mDiffuseTexture.Compress(true);
			mDiffuseTexture.Apply(true, true);
            mNormalTexture.Compress(true);
			mNormalTexture.Apply(true, true);
        }

        static void ProcessTextures(Dictionary<string, ModTextureEntry> dictionaryDiffuse, Dictionary<string, ModTextureEntry> dictionaryNormal, TextureSide textureSide)
        {
            int textureSize = (int)TerrainStitcher.TextureDefinition;
            int unpaddedSize = (TextureDefinition == TextureMode.SD? (int)TextureSize.SD128 : (int)TextureSize.HD256);

			int totalPaddingSize = textureSize - unpaddedSize;
            int halfPaddingSize = totalPaddingSize / 2;
            

			var slot = -1;
            foreach (var key in dictionaryDiffuse.Keys)
            {
                try
                {
                    var bNeedsNewID = key.Contains(".");

                    //Entry to set.
                    TerrainDataEntry entry = TerrainData.mEntriesByKey[key];

                    dictionaryDiffuse[key].UpscaleAsNeeded();

					if (bNeedsNewID)
                    {
                        slot = NextTextureSpriteId++;
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

                    //Process for Diffuse
                    Texture2D newSprite = dictionaryDiffuse[key].texture;
                    Texture2D targetTexture = mDiffuseTexture;

                    var ypos = targetTexture.height - ((int) target.y - halfPaddingSize);

                    //Correction.
                    ypos = ypos - textureSize;

                    targetTexture.SetPixels((int) target.x - halfPaddingSize, ypos, textureSize, textureSize, newSprite.GetPixels());

                    //Process Normal
                    if (dictionaryNormal.ContainsKey(key))
                    {
                        dictionaryNormal[key].UpscaleAsNeeded();
						Texture2D newNormalSprite = dictionaryNormal[key].texture;
                        targetTexture = mNormalTexture;
                        targetTexture.SetPixels((int)target.x - halfPaddingSize, ypos, textureSize, textureSize, newNormalSprite.GetPixels());
                        UnityEngine.Object.Destroy(newNormalSprite);
					}

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

                    UnityEngine.Object.Destroy(newSprite);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

		static void ProcessOreTextures(Dictionary<string, Dictionary<int, ModTextureEntry>> dictionaryDiffuse, Dictionary<string, Dictionary<int, ModTextureEntry>> dictionaryNormal)
		{
            int textureSize = (int)TerrainStitcher.TextureDefinition;
            int unpaddedSize = (TextureDefinition == TextureMode.SD ? (int)TextureSize.SD128 : (int)TextureSize.HD256);

            int totalPaddingSize = textureSize - unpaddedSize;
            int halfPaddingSize = totalPaddingSize / 2;

			var slot = -1;
			foreach (var terrainKey in dictionaryDiffuse.Keys)
            {
                Dictionary<int, ModTextureEntry> stagesDiffuse = dictionaryDiffuse[terrainKey];
                Dictionary<int, ModTextureEntry> stagesNormal = null;

                if (dictionaryNormal.ContainsKey(terrainKey))
                {
                    stagesNormal = dictionaryNormal[terrainKey];
                }

				foreach (KeyValuePair<int, ModTextureEntry> keyValuePair in stagesDiffuse)
                {
                    int key = keyValuePair.Key;
					try
					{
                        var bNeedsNewID = terrainKey.Contains(".");

                        //Entry to set.
                        TerrainDataEntry terrainDataEntry = TerrainData.mEntriesByKey[terrainKey];
                        TerrainDataStageEntry entry = terrainDataEntry.Stages[keyValuePair.Key];

						keyValuePair.Value.UpscaleAsNeeded();

						if (bNeedsNewID)
                        {
                            slot = NextTextureSpriteId++;
                        }
                        else
                        {
                            slot = entry.TopTexture;
                        }

                        Rect target = SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite(slot);

                        //Process for Diffuse
                        Texture2D newSprite = stagesDiffuse[key].texture;
                        Texture2D targetTexture = mDiffuseTexture;

                        var ypos = targetTexture.height - ((int)target.y - halfPaddingSize);

                        //Correction.
                        ypos = ypos - textureSize;

                        targetTexture.SetPixels((int)target.x - halfPaddingSize, ypos, textureSize, textureSize, newSprite.GetPixels());

                        //Process Normal
                        if (stagesNormal != null && stagesNormal.ContainsKey(key))
                        {
                            Texture2D newNormalSprite = stagesNormal[key].texture;
                            targetTexture = mNormalTexture;
                            targetTexture.SetPixels((int)target.x - halfPaddingSize, ypos, textureSize, textureSize, newNormalSprite.GetPixels());
                            UnityEngine.Object.Destroy(newNormalSprite);
						}

                        if (bNeedsNewID)
                        {
                            entry.TopTexture = slot;
                            entry.SideTexture = slot;
                            entry.BottomTexture = slot;
						}

						UnityEngine.Object.Destroy(newSprite);
					}
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
				}

            }
		}

		//static Texture2D LoadPNG(string filePath)
  //      {
  //          Texture2D texture2D = null;
  //          if (File.Exists(filePath))
  //          {
  //              byte[] data = File.ReadAllBytes(filePath);
  //              texture2D = new Texture2D(2, 2); //Size is irrelevent and replaced by the LoadImage call
  //              texture2D.LoadImage(data);
  //          }

  //          return texture2D;
  //      }

        #endregion

        #region  Enums

        public enum TextureSide
        {
            All,
            Top,
            Side,
            Bottom
        }

        public enum TextureSize
        {
            SD128 = 128,
            SD146 = 146,
            HD256 = 256,
            HD292 = 292,
            Invalid = -1
		}

        public enum TextureMode
        {
            SD = 146,
            HD = 292
        }

        public struct ModTextureEntry
        {
            public string filePath;
            public TextureSize Size;
            public Texture2D texture;

            public static implicit operator string(ModTextureEntry entry)
            {
                return entry.filePath;
            }

            public static implicit operator TextureSize(ModTextureEntry entry)
            {
                return entry.Size;
            }

            public static implicit operator Texture2D(ModTextureEntry entry)
            {
                return entry.texture;
            }

            public override string ToString()
            {
                return filePath;
            }

			public ModTextureEntry(string filePath)
            {
                this.filePath = filePath;
                texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture.LoadImage(File.ReadAllBytes(filePath));

                if (texture.width != texture.height)
                {
                    Size = TextureSize.Invalid;
                    UnityEngine.Object.Destroy(texture);
                    texture = null;
                }
                else
                {
                    switch (texture.width)
                    {
                        case 128:
                        {
                            Size = TextureSize.SD128;
                            break;
                        }
                        case 146:
                        {
                            Size = TextureSize.SD146;
                            break;
                        }
                        case 256:
                        {
                            Size = TextureSize.HD256;
                            bSwitchToHD = true;
                            break;
                        }
                        case 292:
                        {
                            Size = TextureSize.HD292;
                            bSwitchToHD = true;
							break;
                        }
                        default:
                        {
                            Size = TextureSize.Invalid;
                            UnityEngine.Object.Destroy(texture);
                            texture = null;
							break;
                        }
					}
				}
            }

            public void UpscaleAsNeeded()
            {
                // IF SD and 128 -> Pad
                // IF HD and 128 -> Pad -> Upscale
                // IF HD and 256 -> PadHD

                if (Size == TextureSize.SD128)
                {
                    Pad();
                    Size = TextureSize.SD146;
                }

                if (Size == TextureSize.SD146 && TextureDefinition == TextureMode.HD)
                {
                    Upscale();
                    Size = TextureSize.HD292;
                }

                if (Size == TextureSize.HD256 && TextureDefinition == TextureMode.HD)
                {
                    PadHD();
                    Size = TextureSize.HD292;
                }
			}

            private void Upscale()
            {
                TextureScale.Point(texture, 292, 292);
            }

            private void Pad()
            {
                //consts for my sanity
                const int sourceSize = 128;
                const int targetSize = 146;

				const int totalPaddingSize = targetSize - sourceSize;
                const int halfPaddingSize = totalPaddingSize / 2;
                
                const int farOffsetSize = sourceSize + halfPaddingSize;

				//Time to fix the image
				var image = new BitMap(texture);
				var newImage = new BitMap(targetSize, targetSize);

                for (int x = 0; x < targetSize; x++)
                {
                    for (int y = 0; y < targetSize; y++)
                    {
                        newImage.SetPixel(x, y, Color.black);
                    }
                }

                for (int x = 0; x < sourceSize; x++)
                {
                    for (int y = 0; y < sourceSize; y++)
                    {
                        newImage.SetPixel(x + halfPaddingSize, y + halfPaddingSize, image.GetPixel(x, y));
                    }
                }

                int farOffset = farOffsetSize;
                int farOffset2 = farOffset - 1;

                //////Ok build the blur.
                for (int x = 0; x < targetSize; x++)
                {
                    for (int y = 0; y < halfPaddingSize; y++)
                    {
                        newImage.SetPixel(x, y, newImage.GetPixel(x, totalPaddingSize - y));
                        newImage.SetPixel(x, farOffset + y, newImage.GetPixel(x, farOffset2 - y));
                    }
                }

                for (int y = 0; y < targetSize; y++)
                {
                    for (int x = 0; x < halfPaddingSize; x++)
                    {
                        newImage.SetPixel(x, y, newImage.GetPixel(totalPaddingSize - x, y));
                        newImage.SetPixel(farOffset + x, y, newImage.GetPixel(farOffset2 - x, y));
                    }
                }
                texture = newImage;
			}

            private void PadHD()
			{
				//consts for my sanity
				const int sourceSize = 256;
				const int targetSize = 292;

				const int totalPaddingSize = targetSize - sourceSize;
				const int halfPaddingSize = totalPaddingSize / 2;

				const int farOffsetSize = sourceSize + halfPaddingSize;

				//Time to fix the image
				var image = new BitMap(texture);
				var newImage = new BitMap(targetSize, targetSize);

				for (int x = 0; x < targetSize; x++)
				{
					for (int y = 0; y < targetSize; y++)
					{
						newImage.SetPixel(x, y, Color.black);
					}
				}

				for (int x = 0; x < sourceSize; x++)
				{
					for (int y = 0; y < sourceSize; y++)
					{
						newImage.SetPixel(x + halfPaddingSize, y + halfPaddingSize, image.GetPixel(x, y));
					}
				}

				int farOffset = farOffsetSize;
				int farOffset2 = farOffset - 1;

				//////Ok build the blur.
				for (int x = 0; x < targetSize; x++)
				{
					for (int y = 0; y < halfPaddingSize; y++)
					{
						newImage.SetPixel(x, y, newImage.GetPixel(x, totalPaddingSize - y));
						newImage.SetPixel(x, farOffset + y, newImage.GetPixel(x, farOffset2 - y));
					}
				}

				for (int y = 0; y < targetSize; y++)
				{
					for (int x = 0; x < halfPaddingSize; x++)
					{
						newImage.SetPixel(x, y, newImage.GetPixel(totalPaddingSize - x, y));
						newImage.SetPixel(farOffset + x, y, newImage.GetPixel(farOffset2 - x, y));
					}
				}
				texture = newImage;
			}

		}

        #endregion
    }
}