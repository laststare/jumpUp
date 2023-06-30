using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using StayAlive.Extensions.Async;

using Object = UnityEngine.Object;

namespace JumpUp.External
{
    public abstract class BaseDisposable : IDisposable
    {
        private bool _isDisposed;
        private List<IDisposable> _mainThreadDisposables;
        private List<Object> _unityObjects;
        private HashSet<IDisposableAwaiter> _operations;
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            if (_operations != null)
            {
                foreach (IDisposableAwaiter operation in _operations)
                {
                    operation.Dispose();
                }
                _operations.Clear();
                _operations = null;
            }

            if (_mainThreadDisposables != null)
            {
                List<IDisposable> mainThreadDisposables = _mainThreadDisposables;
                for (int i = mainThreadDisposables.Count - 1; i >= 0; i--)
                {
                    mainThreadDisposables[i]?.Dispose();
                }
                mainThreadDisposables.Clear();
            }

            try
            {
                OnDispose();
            }
            catch (Exception e)
            {
                Debug.Log($"This exception can be ignored. Disposable of {GetType().Name}: {e}");
            }

            if (_unityObjects != null)
            {
                foreach (Object obj in _unityObjects)
                {
                    if (obj)
                        Object.Destroy(obj);
                }
            }
        }

        protected virtual void OnDispose()
        {
        }

        protected TDisposable AddUnsafe<TDisposable>(TDisposable disposable) where TDisposable : IDisposable
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");
                return default;
            }
            if (disposable == null)
            {
                return default;
            }

            if (_mainThreadDisposables == null)
            {
                _mainThreadDisposables = new List<IDisposable>(1);
            }
            _mainThreadDisposables.Add(disposable);
            return disposable;
        }

        protected IAwaiter KeepOperation(IAwaiter awaiter)
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");
                return default;
            }
            if (awaiter == null)
            {
                Debug.Log("can't keep null operation");
                return default;
            }
            if (awaiter.IsCompleted)
            {
                return awaiter;
            }
            IDisposableAwaiter disposableAwaiter = awaiter.AsDisposable();
            TrackOperation(disposableAwaiter);
            return disposableAwaiter;
        }
        protected IAwaiter<T> KeepOperation<T>(IAwaiter<T> awaiter)
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");
                return default;
            }
            if (awaiter == null)
            {
                Debug.Log("can't keep null operation");
                return default;
            }
            if (awaiter.IsCompleted)
            {
                return awaiter;
            }
            IDisposableAwaiter<T> disposableAwaiter = awaiter.AsDisposable();
            TrackOperation(disposableAwaiter);
            return disposableAwaiter;
        }

        protected IAwaiter KeepOperation(Task task)
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");
                return default;
            }
            if (task == null)
            {
                Debug.Log("can't keep null operation");
                return default;
            }
            IDisposableAwaiter disposableAwaiter = task.AsDisposable();
            if (!disposableAwaiter.IsCompleted)
            {
                TrackOperation(disposableAwaiter);
            }
            return disposableAwaiter;
        }

        protected IAwaiter<T> KeepOperation<T>(Task<T> task)
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");
                return default;
            }
            if (task == null)
            {
                Debug.Log("can't keep null operation");
                return default;
            }
            IDisposableAwaiter<T> disposableAwaiter = task.AsDisposable();
            if (!disposableAwaiter.IsCompleted)
            {
                TrackOperation(disposableAwaiter);
            }
            return disposableAwaiter;
        }

        protected Object AttachComponent(Object obj)
        {
            if (_isDisposed)
            {
                Debug.Log("disposed");
                return default;
            }
            if (!obj)
            {
                Debug.Log("can't add null object");
                return default;
            }
            if (_unityObjects == null)
            {
                _unityObjects = new List<Object>(1);
            }
            _unityObjects.Add(obj);
            return obj;
        }

        private void TrackOperation(IDisposableAwaiter awaiter)
        {
            _operations = _operations ?? new HashSet<IDisposableAwaiter>();
            _operations.Add(awaiter);
            awaiter.OnCompleted(() => _operations?.Remove(awaiter));
        }
    }
}