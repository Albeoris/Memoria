namespace FF8.Core
{
    public interface IAwaitable
    {
        IAwaiter GetAwaiter();
    }
}