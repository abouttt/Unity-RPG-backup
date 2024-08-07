using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LoadingUpdater : MonoBehaviour
{
    public event Action UpdateEnded;

    public float LoadingProgress { get; private set; }

    public void StartUpdate(AsyncOperationHandle<SceneInstance> handle)
    {
        if (!handle.IsValid())
        {
            return;
        }

        StartCoroutine(UpdateLoadingProgress(handle));
    }

    private IEnumerator UpdateLoadingProgress(AsyncOperationHandle<SceneInstance> handle)
    {
        float timer = 0f;

        while (!handle.IsDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (handle.PercentComplete < 0.9f)
            {
                LoadingProgress = Mathf.Lerp(LoadingProgress, handle.PercentComplete, timer);
            }
            else
            {
                break;
            }
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            timer = 0f;

            while (true)
            {
                yield return null;

                timer += Time.deltaTime;
                LoadingProgress = Mathf.Lerp(LoadingProgress, 1f, timer);
                if (LoadingProgress >= 1f)
                {
                    UpdateEnded?.Invoke();
                    LoadingProgress = 0f;
                    yield break;
                }
            }
        }
    }
}
