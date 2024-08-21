using System;
using UnityEngine;

public interface IStackableItem
{
    event Action CountChanged;

    int Count { get; }
    int MaxCount { get; }
}
