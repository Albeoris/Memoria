namespace Memoria
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