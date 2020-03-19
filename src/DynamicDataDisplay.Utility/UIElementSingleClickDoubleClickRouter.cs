using Microsoft.Research.DynamicDataDisplay.Common;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Utility
{
	/// <summary>
	/// Handles the left mouse down/up and timings to determine if the user
	/// intended a single click or a double click.
	/// </summary>
	public class UIElementSingleClickDoubleClickRouter
	{
		private enum CurrentState
		{
			Idle,
			FirstClickDown,
			FirstClickUp,
			SecondClickDown,
			FirstClickDownHeld
		}

		/// <summary>
		/// Flag to indicate that the mouse has moved since the left button was depressed
		/// </summary>
		private bool _leftMouseMoved = false;
		/// <summary>
		/// Timer to determine if it is a double click or not
		/// </summary>
		private DispatcherTimer _leftClickWaitTimer;
		/// <summary>
		/// State of the mouse click
		/// </summary>
		private CurrentState _leftClickState;
		/// <summary>
		/// Time the last click ended
		/// </summary>
		private DateTime _lastLeftClickTime;
		/// <summary>
		/// Position of the last click
		/// </summary>
		private Point _lastLeftClickPoint;
		/// <summary>
		/// The element being clicked on
		/// </summary>
		private UIElement _hookElement;
		/// <summary>
		/// Element mouse click point is relative to
		/// </summary>
		private UIElement _positionElement;

		public UIElementSingleClickDoubleClickRouter(UIElement hookElement, UIElement positionElement)
		{
			_leftClickWaitTimer = new DispatcherTimer
			(
			  TimeSpan.FromMilliseconds(Win32Stuff.GetDoubleClickTime()),
			  DispatcherPriority.Background,
			  LeftMouseWaitTimer_Tick,
			  Dispatcher.CurrentDispatcher
			);

			_positionElement = positionElement;
			_hookElement = hookElement;
			_hookElement.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
			_hookElement.PreviewMouseLeftButtonUp += Element_MouseLeftButtonUp;
			_hookElement.PreviewMouseMove += Element_MouseMove;
		}

		private void Element_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_leftMouseMoved = true;
		}

		public void ExternalMouseEvent(MouseEventArgs args)
		{
			switch (args.RoutedEvent.Name)
			{
				case "MouseMove":
					Element_MouseMove(_hookElement, args);
					break;
				case "MouseLeftButtonUp":
					Element_MouseLeftButtonUp(_hookElement, args as MouseButtonEventArgs);
					break;
				case "MouseLeftButtonDown":
					Element_MouseLeftButtonDown(_hookElement, args as MouseButtonEventArgs);
					break;
			}
		}

		public void FakeLeftClick(Point point)
		{
			_lastLeftClickTime = DateTime.Now;
			_lastLeftClickPoint = point;
			OnLeftSingleClick();
		}

		private void Element_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_lastLeftClickTime = DateTime.Now;
			_lastLeftClickPoint = e.GetPosition(_positionElement); ;
			switch (_leftClickState)
			{
				case CurrentState.Idle:
					break;
				case CurrentState.FirstClickDown:
					_leftClickState = CurrentState.FirstClickUp;
					if (!_leftMouseMoved && UseLeftClickCancelling)
					{
						OnLeftSingleClick();
					}
					break;
				case CurrentState.FirstClickDownHeld:
					if (!_leftMouseMoved)
					{
						OnLeftSingleClick();
					}
					LeftReset();
					break;
				case CurrentState.SecondClickDown:
					if (!_leftMouseMoved)
					{
						if (UseLeftClickCancelling)
						{
							OnCancelLeftSingleClick();
						}
						OnLeftDoubleClick();
					}
					LeftReset();
					break;

			}
		}

		private void Element_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			switch (_leftClickState)
			{
				case CurrentState.Idle:
				case CurrentState.FirstClickDownHeld: // Occurs if the mouse moved after the last mouse down
					_leftClickState = CurrentState.FirstClickDown;
					_leftMouseMoved = false;
					_leftClickWaitTimer.Start();
					break;
				case CurrentState.FirstClickUp:
					_leftClickState = CurrentState.SecondClickDown;
					break;
				default:
					_leftClickWaitTimer.Stop();
					_leftClickState = CurrentState.Idle;
					break;
			}
		}

		private void LeftMouseWaitTimer_Tick(object sender, EventArgs e)
		{
			_leftClickWaitTimer.Stop();

			switch (_leftClickState)
			{
				case CurrentState.Idle:
					break;
				case CurrentState.FirstClickDown:
					_leftClickState = CurrentState.FirstClickDownHeld;
					break;
				case CurrentState.FirstClickUp:
					if (!_leftMouseMoved && !UseLeftClickCancelling)
					{
						OnLeftSingleClick();
					}
					LeftReset();
					break;
			}
		}

		private void LeftReset()
		{
			_leftClickWaitTimer.Stop();
			_leftMouseMoved = false;
			_leftClickState = CurrentState.Idle;
		}

		private void OnLeftSingleClick()
		{
			Trace.WriteLine($"Left Single Click {DateTime.Now}, Delay = {DateTime.Now - _lastLeftClickTime}, Pos = {_lastLeftClickPoint}");
			MouseLeftSingleClick?.Invoke(_hookElement, _lastLeftClickPoint);
		}

		private void OnLeftDoubleClick()
		{
			Trace.WriteLine($"Left Double Click {DateTime.Now}, Delay = {DateTime.Now - _lastLeftClickTime}, Pos = {_lastLeftClickPoint}");
			MouseLeftDoubleClick?.Invoke(_hookElement, _lastLeftClickPoint);
		}

		private void OnCancelLeftSingleClick()
		{
			Trace.WriteLine($"Cancel Left Single Click {DateTime.Now}, Delay = {DateTime.Now - _lastLeftClickTime}, Pos = {_lastLeftClickPoint}");
			MouseCancelLeftSingleClick?.Invoke(_hookElement, _lastLeftClickPoint);
		}

		/// <summary>
		/// Determines whether to send left single click, then cancel left single click, then left double click,
		/// or to wait a bit and then send a single left single click or left double click, but not both.
		/// </summary>
		public bool UseLeftClickCancelling { get; set; } = true;

		public event EventHandler<Point> MouseLeftSingleClick;
		public event EventHandler<Point> MouseLeftDoubleClick;
		public event EventHandler<Point> MouseCancelLeftSingleClick;
	}
}
