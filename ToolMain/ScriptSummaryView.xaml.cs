using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using ToolMain.Models;
using static ToolMain.Models.TextXmlData;

namespace ToolMain
{
    /// <summary>
    /// ScriptSummaryView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScriptSummaryView : UserControl
    {
        public ScriptSummaryView()
        {
            InitializeComponent();

            dgTable.Columns.Add(new DataGridTextColumn() { Header = "No", Binding = new System.Windows.Data.Binding("RowNo") });
            dgTable.Columns.Add(new DataGridCheckBoxColumn() { Header = "", Binding = new System.Windows.Data.Binding("IsModified") });
            dgTable.Columns.Add(new DataGridTextColumn() { Header = "LineID", Binding = new System.Windows.Data.Binding("LineID") });
            dgTable.Columns.Add(new DataGridTextColumn() { Header = "FileName", Binding = new System.Windows.Data.Binding("WEMFilePath") });
            dgTable.Columns.Add(new DataGridTextColumn() { Header = "Description", Binding = new System.Windows.Data.Binding("SpeechText") });

            dgTable.AutoGenerateColumns = false;

            // json 파일을 읽어서 모델화 한 다음 이걸로 config 정보와 row 데이터의 LineID 매칭을 해야 함.

            pgBar.IsIndeterminate = true;

            // sample
            string WEMmallfilefolder = @"E:\400. Anno117-Project\en_us0\data\sound\generatedsoundbanks\windows\media\en_us";

            var allfiles = Directory.GetFiles(WEMmallfilefolder, "*.wem");
            SpeechModel.Singleton.SpeechData.Rows = new List<SpeechDataRow>(allfiles.Count());

            var rows = new List<SpeechDataRow>(allfiles.Count());

            int index = 1;
            foreach (var file in allfiles)
            {
                FileInfo fileinfo = new FileInfo(file);

                rows.Add(new SpeechDataRow()
                {
                    RowNo = index++,
                    WEMFilePath = fileinfo.Name,
                    FullPath = file
                });
            }

            var speechjsondata = JsonSerializer.Deserialize<SpeechJsonData>(
                File.ReadAllText(@"E:\400. Anno117-Project\en_us0\data\sound\generatedsoundbanks\windows\en_us\1648518890.json"));

            var listtext = SpeechModel.Singleton.ConfigData.Texts;

            var medialist = speechjsondata.SoundBanksInfo.SoundBanks[0].Media;

            index = 0;
            int limit = 100;

            foreach (var item in listtext)
            {
                var foundmedia = medialist.Find(x => x.ShortName.Contains(item.HexID));
                if (foundmedia is null) continue;

                var foundrow = rows.Find(x => x.FileNumber == foundmedia.NoId);
                if (foundrow is null) continue;

                foundrow.SpeechText = item.Value;
                foundrow.LineID = item.LineId;
            }
            SpeechModel.Singleton.SpeechData.Rows = rows;

            dgTable.ItemsSource = SpeechModel.Singleton.SpeechData.Rows;

            pgBar.IsIndeterminate = false;
        }
    }
}