using System.Windows.Controls;

namespace BreakpointManager.Views
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	public partial class ProjectView : UserControl
	{
		public ProjectView()
		{
			InitializeComponent();
			ViewContent.Content = new OperationsBaseView(Enums.eOperationMode.Project);
		}
	}
}
