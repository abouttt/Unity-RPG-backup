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

        InitProcess();

        _isInit = true;
    }

    public void Clear()
    {
        if (!_isInit)
        {
            return;
        }

        ClearProcess();
    }

    public void Dispose()
    {
        if (!_isInit)
        {
            return;
        }

        ClearProcess();
        DisposeProcess();

        _isInit = false;
    }

    protected void CheckInit()
    {
        if (!_isInit)
        {
            throw new InvalidOperationException($"{typeof(T)} is not initialized");
        }
    }

    protected abstract void InitProcess();
    protected abstract void ClearProcess();
    protected abstract void DisposeProcess();
}
