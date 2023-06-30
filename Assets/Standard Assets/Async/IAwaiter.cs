using System.Runtime.CompilerServices;

namespace StayAlive.Extensions.Async
{
  public interface IAwaiter : INotifyCompletion
  {
    bool IsCompleted { get; }
    void GetResult();
    IAwaiter GetAwaiter();
  }
  public interface IAwaiter<out T> : IAwaiter
  {
    bool IsCompleted { get; }
    T GetResult();
    IAwaiter<T> GetAwaiter();
  }
}