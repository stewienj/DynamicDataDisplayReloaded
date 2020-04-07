namespace DynamicDataDisplay.Common.UndoSystem
{
	public abstract class UndoAction
	{
		public abstract void Do();
		public abstract void Undo();
	}
}
