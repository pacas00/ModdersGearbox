using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace plugin_petercashel_ModdersGearbox.Features.CustomBlockTextures.XML
{
    [Serializable]
    [XmlInclude(typeof(TextureMappingData))]
    [XmlInclude(typeof(TextureMappingDataStage))]
	public class TextureSlotMapping
	{
        [XmlAttribute]
        public string Key;

        [XmlElement("Data", typeof(TextureMappingData))]
        public TextureMappingData Data;
	}

    [Serializable]
    [XmlInclude(typeof(TextureMappingDataStage))]
	public class TextureMappingData
	{

		[XmlElement("Top")]
        public int Top;

        [XmlElement("Side")]
        public int Side;

        [XmlElement("Bottom")]
        public int Bottom;

        [XmlElement("HasStages")]
		public bool HasStages = false;

        [XmlArray]
        public List<TextureMappingDataStage> Stages = new List<TextureMappingDataStage>();

        public TextureMappingDataStage GetStage(int key)
        {
            TextureMappingDataStage stage = Stages.First(x => x.Key == key);
            if (stage != null)
            {
                return stage;
            }

            HasStages = true;
			stage = new TextureMappingDataStage() {Key = key};
            Stages.Add(stage);
            return stage;
        }
    }

    [Serializable]
    public class TextureMappingDataStage
	{
        [XmlElement("Key")]
		public int Key;

		[XmlElement("Top")]
        public int Top;

        [XmlElement("Side")]
        public int Side;

        [XmlElement("Bottom")]
        public int Bottom;
	}
}
