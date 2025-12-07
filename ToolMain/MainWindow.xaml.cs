using System.Windows;
using CommunityToolkit.Mvvm.Messaging;

namespace ToolMain;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ViewProcessChangedMessage>(this, (r, m) =>
        {
            // Handle the message here, with r being the recipient and m being the
            // input message. Using the recipient passed as input makes it so that
            // the lambda expression doesn't capture "this", improving performance.
            switch (m.Value)
            {
                case eProcess.Path:
                    root.Children.Clear();
                    root.Children.Add(new PathView());
                    break;

                case eProcess.Script:
                    root.Children.Clear();
                    root.Children.Add(new ScriptSummaryView());
                    break;

                case eProcess.Packaging:
                    root.Children.Clear();
                    root.Children.Add(new PackagingView());
                    break;
            }
        });

        WeakReferenceMessenger.Default.Send(new ViewProcessChangedMessage(eProcess.Path));
    }
}