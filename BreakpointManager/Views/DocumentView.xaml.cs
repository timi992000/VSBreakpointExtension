using System.Windows.Controls;

namespace BreakpointManager.Views
{
	/// <summary>
	/// Interaction logic for DocumentView.xaml
	/// </summary>
	public partial class DocumentView : UserControl
	{
		public DocumentView()
		{
			InitializeComponent();
			ViewContent.Content = new OperationsBaseView(Enums.eOperationMode.Document);
		}
	}
}
