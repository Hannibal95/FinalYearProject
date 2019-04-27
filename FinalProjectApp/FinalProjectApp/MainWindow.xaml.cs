using System.Windows.Navigation;

namespace FinalProjectApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            SysVars.CurrentProjectId = 0;
            SysVars.CurrentProjectName = "";
            SysVars.CurrentProjectDescription = "";
            SysVars.CurrentBuildId = 0;
            SysVars.CurrentBuildName = "";
        }
    }
}
