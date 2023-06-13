using System;
using System.Windows.Input;

namespace BreakpointManager.BaseClasses
{
	public class RelayCommand : ICommand
	{
		readonly Action<object> _execute;
		readonly Predicate<object> _canExecute;

		public RelayCommand(Action<object> execute)
			: this(execute, null)
		{

		}
		public event EventHandler CanExecuteChanged;

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}


		public void Execute(object parameter)
		{
			if (CanExecute(parameter))
				_execute(parameter);
		}

		public void OnCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}

	}
}
