using System;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T s_instance;

    public static T Instance
    {
        get
        {
            if (s_instance == null)
            {
                var assets = Resources.LoadAll<T>("");
                if (assets == null || assets.Length == 0)
                {
                    throw new Exception($"Could not find any singleton scriptable object instance in the resources : {typeof(T)}");
                }
                else if (assets.Length > 1)
                {
                    Debug.Log($"Multiple instance of the singleton scriptable object found in the resources : {typeof(T)}");
                }

                s_instance = assets[0];
                s_instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }

            return s_instance;
        }
    }

    private void OnEnable()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
        }
        else
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(s_instance);
            Debug.LogWarning($"{name} is a singleton scriptable object. This asset already exists : {path}");
#endif
        }
    }

    private void OnDisable()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }
}
