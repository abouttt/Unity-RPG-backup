using System;
using UnityEngine;

public interface IStackable
{
    event Action StackChanged;

    int Quantity { get; }
    int MaxQuantity { get; }
}
