using System.IO;
using System.Xml.Serialization;
using static ToolMain.Models.TextXmlData;

namespace ToolMain.Lib;

/*
        * format
       <TextExport>
           <Texts>
               <Text>
                 <LineId>-6917528483250110679</LineId>
                 <Text>​총 ​통합 ​세력</Text>
               </Text>
               <Text>
                 <LineId>-6917528174128565175</LineId>
                 <Text>​늑대의 ​깃발</Text>
               </Text>
           </Texts>
        </TextExport>
        */

public static class TextXmlReader
{
    public static TextExport LoadFromFile(string filePath)
    {
        //if (!File.Exists(filePath)) throw new FileNotFoundException("TextExport 파일을 찾을 수 없습니다.", filePath);

        var serializer = new XmlSerializer(typeof(TextExport));

        using var stream = File.OpenRead(filePath);
        var result = (TextExport)serializer.Deserialize(stream)!;
        return result;
    }
}