using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualBasic.FileIO;
using Microsoft.WindowsAPICodePack.Dialogs;
using RDAExplorer;
using ToolMain.Lib;
using ToolMain.Models;

namespace ToolMain
{
    /// <summary>
    /// PathView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PathView : UserControl
    {
        private const string initpath = "C:\\Program Files (x86)\\Ubisoft\\Ubisoft Game Launcher\\games\\Anno 117 - Pax Romana\\maindata";

        private const string tempfolder = "speech";
        private const string configfolder = "config";
        private string CurrentConfigFileName;

        public PathView()
        {
            InitializeComponent();

            if (Path.Exists(initpath)) tbPath.Text = initpath;

            // config.rda에서 가져온 파일을 가져와서 읽기위한 폴더 만들기
            var configpath = Directory.CreateDirectory(configfolder);
            Config.ConfigFolderPath = configpath.FullName;

            // en_us0.rda 파일 임시 폴더 만들기
            var lang = Directory.CreateDirectory(tempfolder);
            Config.LanguageFolderPath = lang.FullName;
        }

        private void NextView()
        {
            WeakReferenceMessenger.Default.Send(new ViewProcessChangedMessage(eProcess.Script));
        }

        private void btnPath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // 폴더 선택 다이얼로그 띄우기
            var dialog = new CommonOpenFileDialog();
            if (Path.Exists(initpath))
            {
                dialog.InitialDirectory = initpath;
            }

            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                tbPath.Text = dialog.FileName; // 테스트용, 폴더 선택이 완료되면 선택된 폴더를 label에 출력
                CheckPath(tbPath.Text);
            }
        }

        private void btnSet_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // 경로 및 파일 체크 후 다음 뷰로 이동
            // 그리고 읽는동안 로딩 애니메이션 보여주기
            if (string.IsNullOrEmpty(tbPath.Text)) return;

            CheckPath(tbPath.Text);
        }

        private void CheckPath(string path)
        {
            var configpath = Path.Combine(path, "config.rda");
            var langpath = Path.Combine(path, "en_us0.rda");

            var bConfig = File.Exists(configpath);
            var bLang = File.Exists(Path.Combine(path, langpath));

            if (bLang)
            {
                view_language.Visibility = System.Windows.Visibility.Visible;
                //GetConfigFile(langpath);
            }

            if (bConfig)
            {
                view_config.Visibility = System.Windows.Visibility.Visible;

                GetConfigFile(configpath);
            }
        }

        private void GetConfigFile(string fileName)
        {
            CurrentConfigFileName = null;

            string configFileName = "texts_korean.xml";
            //A00001C9F893464D
            // config.rda 파일 읽기
            RDAReader reader = new RDAReader();
            CurrentConfigFileName = fileName;
            fileName = Path.Combine(Config.ConfigFolderPath, "config.rda");
            var newconfigfile = Path.Combine(Config.ConfigFolderPath, configFileName);
            reader.FileName = fileName;
            reader.backgroundWorker = new BackgroundWorker();
            reader.backgroundWorker.WorkerSupportsCancellation = true;
            reader.backgroundWorker.WorkerReportsProgress = true;
            reader.backgroundWorker.ProgressChanged += (s, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    // 변경진행도 보여주기
                });
            };
            reader.backgroundWorker.DoWork += (sender2, e2) =>
            {
                try
                {
                    // CurrentFileName = "C:\\Program Files (x86)\\Ubisoft\\Ubisoft Game Launcher\\games\\Anno 117 - Pax Romana\\maindata\\config.rda"
                    // fileName = "C:\\Users\\{UserName}\\AppData\\Local\\Temp\\RDAExplorer\\Instance4\\config.rda"
                    File.Delete(fileName);

                    FileSystem.CopyFile(CurrentConfigFileName, fileName, UIOption.AllDialogs, UICancelOption.ThrowException);

                    reader.ReadRDAFile();

                    var allfilelist = reader.rdaFolder.GetAllFiles();
                    var find = allfilelist.Find(x => x.FileName.Contains(configFileName));
                    if (find is null)
                    {
                        Debug.Assert(false, "한국어 대사 파일이 없음");
                        reader.backgroundWorker.CancelAsync();
                        return;
                    }
                    // 여기까지가 config.rda 파일에서 texts_korean.xml 파일을 찾아내는 과정
                    find.Extract(newconfigfile);

                    // 모델 새로 만들기
                    SpeechModel.Singleton.ConfigData = TextXmlReader.LoadFromFile(newconfigfile);
                    SpeechModel.Singleton.SpeechData = new SpeechDataModel();

                    reader.backgroundWorker.ReportProgress(100);
                }
                catch (Exception ex)
                {
                    reader.backgroundWorker.CancelAsync();
                    reader.backgroundWorker.ReportProgress(0);
                }
            };
            reader.backgroundWorker.RunWorkerCompleted += (sender2, e2) =>
            {
                // 파일 생성까지 완료된걸로 처리
                //reader.backgroundWorker.ReportProgress(100);
            };

            reader.backgroundWorker.RunWorkerAsync();
        }

        private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NextView();
        }
    }
}