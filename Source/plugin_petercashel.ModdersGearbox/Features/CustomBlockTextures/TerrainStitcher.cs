using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures
{
    public static class TerrainStitcher
    {
        #region  Fields and Properties

        static bool mbHasRun = false;

        static Texture2D mDiffuseTexture;
        static Texture2D mNormalTexture;

        public static Dictionary<string, string> DiffuseTexturesAll = new Dictionary<string, string>();
        public static Dictionary<string, string> DiffuseTexturesTop = new Dictionary<string, string>();
        public static Dictionary<string, string> DiffuseTexturesSide = new Dictionary<string, string>();
        public static Dictionary<string, string> DiffuseTexturesBottom = new Dictionary<string, string>();

        public static Dictionary<string, string> NormalTexturesAll = new Dictionary<string, string>();
        public static Dictionary<string, string> NormalTexturesTop = new Dictionary<string, string>();
        public static Dictionary<string, string> NormalTexturesSide = new Dictionary<string, string>();
        public static Dictionary<string, string> NormalTexturesBottom = new Dictionary<string, string>();

        public static int NextTextureSpriteId = 392;

        #endregion

        #region  Methods

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
                #if DEBUG
                ExportTextures();
                #endif
			}
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

            SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord = new TerrainUVCoord(SegmentMeshCreator.instance.segmentMaterial.mainTexture.width, SegmentMeshCreator.instance.segmentMaterial.mainTexture.height, 128, 128, 9);
            TerrainUV.lnMatPiy = SegmentMeshCreator.instance.segmentMaterial.mainTexture.height;

            //TerrainUV.lnTilesY = (int)(TerrainUV.lnMatPiy / (float)TerrainUV.lnTotalPix);
            var tmp = TerrainUV.lnMatPiy / 146;
            TerrainUV.lnTilesY = (int)tmp;

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

                    if (tmp != null)
                    {
                        var h = tmp.height;
                        var w = tmp.width;

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
                        var h = tmp.height;
                        var w = tmp.width;

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
                mDiffuseTexture = diffuseTmp;
                mNormalTexture = normalTmp;
            }
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

        static void StitchTexturesAndAssignIDs()
        {
            //SegmentMeshCreator.instance.mMeshRenderer.segmentUVCoord.GetSprite() //Does this get the right rect?

            //throw new NotImplementedException();

            ProcessTextures(DiffuseTexturesAll, NormalTexturesAll, TextureSide.All);
            ProcessTextures(DiffuseTexturesTop, NormalTexturesTop, TextureSide.Top);
            ProcessTextures(DiffuseTexturesSide, NormalTexturesSide, TextureSide.Side);
            ProcessTextures(DiffuseTexturesBottom, NormalTexturesBottom, TextureSide.Bottom);

            mDiffuseTexture.Apply();
            mNormalTexture.Apply();
        }

        static void ProcessTextures(Dictionary<string, string> dictionaryDiffuse, Dictionary<string, string> dictionaryNormal, TextureSide textureSide)
        {
            var slot = -1;
            foreach (var key in dictionaryDiffuse.Keys)
            {
                try
                {
                    var bNeedsNewID = key.Contains(".");

                    //Entry to set.
                    TerrainDataEntry entry = TerrainData.mEntriesByKey[key];

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
                    Texture2D newSprite = LoadPNG(dictionaryDiffuse[key], 146, 146);
                    Texture2D targetTexture = mDiffuseTexture;

                    var ypos = targetTexture.height - ((int) target.y - 9);

                    //Correction.
                    ypos = ypos - 146;

                    targetTexture.SetPixels((int) target.x - 9, ypos, 146, 146, newSprite.GetPixels());

                    //Process Normal
                    if (dictionaryNormal.ContainsKey(key))
                    {
                        Texture2D newNormalSprite = LoadPNG(dictionaryNormal[key], 146, 146);
                        targetTexture = mNormalTexture;
                        targetTexture.SetPixels((int)target.x - 9, ypos, 146, 146, newNormalSprite.GetPixels());
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
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        static Texture2D LoadPNG(string v1, int v2, int v3)
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

#endregion

#region  Enums

        public enum TextureSide
        {
            All,
            Top,
            Side,
            Bottom
        }

#endregion
    }
}