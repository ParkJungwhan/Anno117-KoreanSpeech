using System.IO;
using System.Xml.Serialization;
using static ToolMain.Models.TextXmlData;

namespace ToolMain.Models;

public class SpeechModel
{
    public static SpeechModel Singleton = new SpeechModel();

    public TextExport ConfigData { get; set; }
    public SpeechDataModel SpeechData { get; set; }
}

public class SpeechDataModel
{
    public List<SpeechDataRow> Rows { get; set; } = new();
}

public class SpeechDataRow
{
    public int RowNo { get; set; }
    public bool IsModified { get; set; } = false;
    public string SpeechText { get; set; }
    public string WEMFilePath { get; set; }
    public string FullPath { get; set; }
    public long LineID { get; set; }

    public int FileNumber
    {
        get { return Convert.ToInt32(Path.GetFileNameWithoutExtension(WEMFilePath)); }
    }
}

public class TextXmlData
{
    // 루트 노드 <TextExport>
    [XmlRoot("TextExport")]
    public class TextExport
    {
        [XmlArray("Texts")]
        [XmlArrayItem("Text")]
        public List<TextEntry> Texts { get; set; } = new();
    }

    // <Texts> 안의 <Text> 하나
    public class TextEntry
    {
        // <LineId> -6917...
        [XmlElement("LineId")]
        public long LineId { get; set; }

        public string HexID
        { get { return LineId.ToString("X"); } }

        // <Text> ​총 ​통합 ​세력
        // 내부 태그 이름이 Text라서 프로퍼티 이름 헷갈릴 수 있으니 Value로 둠
        [XmlElement("Text")]
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{LineId}: {Value}";
        }
    }

    public class SpeechJsonData
    {
        public MediaList SoundBanksInfo { get; set; }
    }

    public class MediaList
    {
        public List<SoundBank> SoundBanks { get; set; }
    }

    public class SoundBank
    {
        public List<JsonData> Media { get; set; }
    }

    public class JsonData
    {
        public string Id { get; set; }

        public int NoId
        { get { return Convert.ToInt32(Id); } }

        public string ShortName { get; set; }

        public string Path { get; set; }
        public string PrefetchSize { get; set; }
        //public int PrefetchSize { get; set; }
    }
}