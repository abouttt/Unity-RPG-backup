using UnityEngine;

public interface ISystemConnectable<T> where T : class
{
    T SystemRef { get; }

    void ConnectSystem(T system);
    void DeconnectSystem();
}
