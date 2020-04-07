using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicDataDisplay.Common.Auxiliary
{
	/// <summary>
	/// Preferable to use RelayCommandFactory to generate these
	/// </summary>
	public class RelayCommandD3 : ICommand
	{
		#region Fields

		private readonly Action<object> _execute;
		private readonly Predicate<object> _canExecute;
		private bool _isRunning = false;

		#endregion // Fields

		#region Constructors

		protected RelayCommandD3(Predicate<object> canExecute)
		{
			_execute = null;
			_canExecute = canExecute;
		}

		/// <summary>
		/// Internal constructor, use the RelayCommandFactory for generating instances
		/// </summary>
		/// <param name="execute"></param>
		internal RelayCommandD3(Action<object> execute) : this(execute, null)
		{
		}

		/// <summary>
		/// Internal constructor, use the RelayCommandFactory for generating instances
		/// </summary>
		/// <param name="execute"></param>
		/// <param name="canExecute"></param>
		internal RelayCommandD3(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}
		#endregion // Constructors

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return !_isRunning && (_canExecute?.Invoke(parameter) ?? true);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public virtual void Execute(object parameter)
		{
			_isRunning = true;
			CommandManager.InvalidateRequerySuggested();
			_execute(parameter);
			_isRunning = false;
			CommandManager.InvalidateRequerySuggested();
		}

		#endregion // ICommand Members

		public bool IsRunning { get => _isRunning; set => _isRunning = value; }
	}

	public class RelayCommandAsyncD3 : RelayCommandD3
	{
		private Func<object, Task> _executeTask;
		public RelayCommandAsyncD3(Func<object, Task> execute)
		  : this(execute, null)
		{
		}

		public RelayCommandAsyncD3(Func<object, Task> execute, Predicate<object> canExecute)
		  : base(canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_executeTask = execute;
		}

		public RelayCommandAsyncD3(Action<object> execute)
		  : this(execute, null)
		{
		}

		public RelayCommandAsyncD3(Action<object> execute, Predicate<object> canExecute)
		  : this(parameter => Task.Factory.StartNew(() => execute(parameter)), canExecute)
		{
		}

		public override void Execute(object parameter)
		{
			var context = SynchronizationContext.Current;
			IsRunning = true;
			CommandManager.InvalidateRequerySuggested();
			_executeTask(parameter).ContinueWith((t) =>
			{
				IsRunning = false;
				context?.Post(x => CommandManager.InvalidateRequerySuggested(), null);
			});
		}

	}


	/// <summary>
	/// Preferred way of creating RelayCommand objects
	/// </summary>
	/// <example>
	///   private RelayCommandFactory _loadFilesCommand = new RelayCommandFactory();
	///   public ICommand LoadFilesCommand =>
	///     _loadFilesCommand.GetCommandAsync(async () =>
	///     {
	///       OpenFileDialog dialog = new OpenFileDialog();
	///       dialog.Multiselect = true;
	///       if (dialog.ShowDialog() == true)
	///       {
	///         foreach (var filename in dialog.FileNames)
	///         {
	///           await LoadFile(filename);
	///         }
	///       }
	///       MessageBox.Show("Finished Loading FIles");
	///     });
	///   }
	///
	///   private RelayCommandFactory _testCommand = new RelayCommandFactory();
	///   public ICommand TestCommand =>
	///       _testCommand.GetCommandAsync(async () => await Task.Run(() =>
	///       {
	///         for (int i = 0; i < 100; ++i)
	///         {
	///           Thread.Sleep(100);
	///           Progress = i / 99.0;
	///         }
	///       }));
	/// 
	/// 
	///   private RelayCommandFactory _removeLastCommand = new RelayCommandFactory();
	///   public ICommand RemoveLastCommand =>
	///       _removeLastCommand.GetCommandAsync(async () =>
	///       {
	///         var itemRemoved = await Task.Run(() => TestCollection.RemoveLast());
	///         Message($"Removed {itemRemoved}");
	///       });
	///   
	/// // The first example can also be done like this without the task and async
	/// 
	///   private RelayCommandFactory _testCommand = new RelayCommandFactory();
	///   public ICommand TestCommand =>
	///       _testCommand.GetCommandAsync(() =>
	///       {
	///         for (int i = 0; i < 100; ++i)
	///         {
	///           Thread.Sleep(100);
	///           Progress = i / 99.0;
	///         }
	///       }));
	/// </example>
	public class RelayCommandFactoryD3
	{
		private RelayCommandD3 _relayCommand = null;

		public RelayCommandFactoryD3()
		{

		}

		/// <summary>
		/// Gets the command and blocks execution while the command is running
		/// </summary>
		/// <param name="execute"></param>
		/// <param name="canExecute"></param>
		/// <returns></returns>
		public ICommand GetCommand(Action<object> execute, Predicate<object> canExecute)
		{
			_relayCommand = _relayCommand ?? new RelayCommandD3(execute, canExecute);
			return _relayCommand;
		}
		public ICommand GetCommand(Action execute, Predicate<object> canExecute)
		{
			return GetCommand(param => execute(), canExecute);
		}
		public ICommand GetCommand(Action<object> execute, Func<bool> canExecute)
		{
			return GetCommand(execute, param => canExecute());
		}
		public ICommand GetCommand(Action execute, Func<bool> canExecute)
		{
			return GetCommand(param => execute(), param => canExecute());
		}
		public ICommand GetCommand(Action<object> execute)
		{
			return GetCommand(execute, (Predicate<object>)null);
		}
		public ICommand GetCommand(Action execute)
		{
			return GetCommand(param => execute(), (Predicate<object>)null);
		}
		public ICommand GetCommandAsync(Action<object> execute, Predicate<object> canExecute)
		{
			_relayCommand = _relayCommand ?? new RelayCommandAsyncD3(execute, canExecute);
			return _relayCommand;
		}
		public ICommand GetCommandAsync(Action execute, Predicate<object> canExecute)
		{
			return GetCommandAsync(param => execute(), canExecute);
		}
		public ICommand GetCommandAsync(Action<object> execute, Func<bool> canExecute)
		{
			return GetCommandAsync(execute, param => canExecute());
		}
		public ICommand GetCommandAsync(Action execute, Func<bool> canExecute)
		{
			return GetCommandAsync(param => execute(), param => canExecute());
		}
		public ICommand GetCommandAsync(Action<object> execute)
		{
			return GetCommandAsync(execute, (Predicate<object>)null);
		}
		public ICommand GetCommandAsync(Action execute)
		{
			return GetCommandAsync(param => execute(), (Predicate<object>)null);
		}


		public ICommand GetCommandAsync(Func<object, Task> execute, Predicate<object> canExecute)
		{
			_relayCommand = _relayCommand ?? new RelayCommandAsyncD3(execute, canExecute);
			return _relayCommand;
		}
		public ICommand GetCommandAsync(Func<Task> execute, Predicate<object> canExecute)
		{
			return GetCommandAsync(param => execute(), canExecute);
		}
		public ICommand GetCommandAsync(Func<object, Task> execute, Func<bool> canExecute)
		{
			return GetCommandAsync(execute, param => canExecute());
		}
		public ICommand GetCommandAsync(Func<Task> execute, Func<bool> canExecute)
		{
			return GetCommandAsync(param => execute(), param => canExecute());
		}
		public ICommand GetCommandAsync(Func<object, Task> execute)
		{
			return GetCommandAsync(execute, (Predicate<object>)null);
		}
		public ICommand GetCommandAsync(Func<Task> execute)
		{
			return GetCommandAsync(param => execute(), (Predicate<object>)null);
		}

	}
	/*
	public class RelayCommandTest : NotifyPropertyChanged
	{
	  public static RelayCommandTest PreviousInstance { get; set; }
	  public static RelayCommandTest NewInstance
	  {
		get
		{
		  PreviousInstance = new RelayCommandTest();
		  return PreviousInstance;
		}
	  }

	  private RelayCommandFactory _testCommandAsync = new RelayCommandFactory();
	  public ICommand TestCommandAsync =>
		_testCommandAsync.GetCommandAsync(() =>
		{
		  for (int i = 0; i < 100; ++i)
		  {
			Thread.Sleep(100);
			Progress = i / 99.0;
		  }
		});

	  private RelayCommandFactory _testCommand = new RelayCommandFactory();
	  public ICommand TestCommand => _testCommand.GetCommand(param =>
		{
		  for (int i = 0; i < 100; ++i)
		  {
			Thread.Sleep(10);
			Progress = i / 99.0;
		  }
		});

	  private double _progress = 0;
	  public double Progress
	  {
		get
		{
		  return _progress;
		}
		set
		{
		  SetValue(ref _progress, value);
		}
	  }
	}
	*/
}
