namespace Memoria.Prime.Threading
{
	public enum TaskState
	{
		Created = 0,
		WaitingToRun,
		Running,
		Success,
		Canceled,
		Faulted
	}
}
