using System;

namespace JumpUp.External
{
    public interface IReadOnlyReactiveTrigger
    {
        IDisposable Subscribe(Action action);
    }
}