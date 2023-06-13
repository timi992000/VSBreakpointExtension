using System.Windows.Controls;

namespace BreakpointManager.Views
{
	/// <summary>
	/// Interaction logic for SolutionView.xaml
	/// </summary>
	public partial class SolutionView : UserControl
	{
		public SolutionView()
		{
			InitializeComponent();
			ViewContent.Content = new OperationsBaseView(Enums.eOperationMode.Solution);
		}
	}
}
