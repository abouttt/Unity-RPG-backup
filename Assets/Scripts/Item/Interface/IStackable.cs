using System;
using UnityEngine;

public interface IStackable
{
    event Action StackChanged;

    int Count { get; }
    int MaxCount { get; }
}
