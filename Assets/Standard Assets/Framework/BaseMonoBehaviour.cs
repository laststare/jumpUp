using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using UnityEngine;

using StayAlive.Extensions.Async;

namespace StayAlive
{
    public class BaseMonoBehaviour : MonoBehaviour
    {
        private HashSet<IDisposable> _operations;


        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {
            if (_operations != null)
            {
                foreach (IDisposable operation in _operations)
                {
                    operation.Dispose();
                }
                _operations.Clear();
                _operations = null;
            }

            FieldInfo[] allFields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in allFields)
            {
                Type fieldType = field.FieldType;
                if (typeof(IList).IsAssignableFrom(fieldType))
                {
                    if (field.GetValue(this) is IList list)
                    {
                        list.Clear();
                    }
                }
                if (typeof(IDictionary).IsAssignableFrom(fieldType))
                {
                    if (field.GetValue(this) is IDictionary dictionary)
                    {
                        dictionary.Clear();
                    }
                }
                if (!fieldType.IsPrimitive)
                {
                    field.SetValue(this, null);
                }
            }
        }

        protected IAwaiter<T> KeepOperation<T>(IAwaiter<T> awaiter)
        {
            if (awaiter == null)
            {
                Debug.LogError("can't keep null awaiter", this);
                return default;
            }
            return KeepOperation(awaiter.AsDisposable());
        }

        protected IAwaiter<T> KeepOperation<T>(Task<T> task)
        {
            if (task == null)
            {
                Debug.LogError("can't keep null task", this);
                return default;
            }
            return KeepOperation(task.AsDisposable());
        }

        private IAwaiter<T> KeepOperation<T>(IDisposableAwaiter<T> awaiter)
        {
            _operations = _operations ?? new HashSet<IDisposable>();
            _operations.Add(awaiter);
            awaiter.OnCompleted(() => _operations?.Remove(awaiter));
            return awaiter;
        }
    }
}