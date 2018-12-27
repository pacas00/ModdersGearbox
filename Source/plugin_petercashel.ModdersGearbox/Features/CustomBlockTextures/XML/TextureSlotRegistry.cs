using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.XML
{
	public class TextureSlotRegistry
	{
        public static Dictionary<string, TextureMappingData> TextureSlotMapping = new Dictionary<string, TextureMappingData>();
        public static string settingsFileName = "TextureSlotMappings.xml";


        public static string GetDataPath()
        {
            var dataSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "ModdersGearbox";
            if (!Directory.Exists(dataSavePath))
            {
                Directory.CreateDirectory(dataSavePath);
            }

            dataSavePath += Path.DirectorySeparatorChar + "Cache" + Path.DirectorySeparatorChar + TerrainStitcher.TextureHashString + Path.DirectorySeparatorChar;
            if (!Directory.Exists(dataSavePath))
            {
                Directory.CreateDirectory(dataSavePath);
            }

            return dataSavePath;
        }

        public static void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TextureSlotMapping[]), new XmlRootAttribute() { ElementName = "TextureSlotMapping" });

            if (File.Exists(GetDataPath() + settingsFileName))
            {
                try
                {
                    using (TextReader reader = File.OpenText(GetDataPath() + settingsFileName))
                    {
                        TextureSlotMapping = ((TextureSlotMapping[])serializer.Deserialize(reader)).ToDictionary(i => i.Key, i => i.Data);
                    }

                }
                catch (Exception ex)
                {
                    UtilClass.WriteLine(String.Format("Load(): {0}", ex.ToString()));
                    TextureSlotMapping = new Dictionary<string, TextureMappingData>();
                }
            }
            else
            {

            }
        }

        public static void Save()
        {
			XmlSerializer serializer = new XmlSerializer(typeof(TextureSlotMapping[]), new XmlRootAttribute() { ElementName = "TextureSlotMapping" });

			using (TextWriter writer = File.CreateText(GetDataPath() + settingsFileName))
            {
                serializer.Serialize(writer, TextureSlotMapping.Select(kv => new TextureSlotMapping() { Key = kv.Key, Data = kv.Value }).ToArray());
            }
        }

        public static TextureMappingData GetMappingForKey(string key)
		{
            if (TextureSlotMapping.ContainsKey(key))
            {
                return TextureSlotMapping[key];
            }
            else
            {
                var mapping = new TextureMappingData();
                TextureSlotMapping.Add(key,mapping);
				return mapping;
            }
		}
	}
}
