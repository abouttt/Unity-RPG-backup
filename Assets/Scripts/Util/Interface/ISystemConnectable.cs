using UnityEngine;

public interface ISystemConnectable<T> where T : class
{
    void ConnectSystem(T system);
    void DeconnectSystem();
}
