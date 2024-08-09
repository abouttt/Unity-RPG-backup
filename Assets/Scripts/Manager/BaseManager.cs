using System;
using UnityEngine;

public abstract class BaseManager<T> where T : class
{
    private bool _isInit;

    public void Init()
    {
        if (_isInit)
        {
            return;
        }

        OnInit();

        _isInit = true;
    }

    public void Clear()
    {
        if (!_isInit)
        {
            return;
        }

        OnClear();
    }

    public void Dispose()
    {
        if (!_isInit)
        {
            return;
        }

        OnClear();
        OnDispose();

        _isInit = false;
    }

    protected void CheckInit()
    {
        if (!_isInit)
        {
            throw new InvalidOperationException($"{typeof(T)} is not initialized");
        }
    }

    protected abstract void OnInit();
    protected abstract void OnClear();
    protected abstract void OnDispose();
}
