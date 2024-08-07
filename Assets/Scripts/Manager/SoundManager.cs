using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class SoundManager : BaseManager<SoundManager>
{
    private Transform _root;
    private readonly List<AudioSource> _audioSources = new();

    protected override void InitProcess()
    {
        _root = Util.Instantiate("Sound_Root", true).transform;

        foreach (var typeName in Enum.GetNames(typeof(SoundType)))
        {
            var go = new GameObject(typeName);
            var audioSource = go.AddComponent<AudioSource>();
            _audioSources.Add(audioSource);
            go.transform.SetParent(_root);
        }

        _audioSources[(int)SoundType.BGM].loop = true;
    }

    protected override void ClearProcess()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    protected override void DisposeProcess()
    {
        if (_root != null)
        {
            Object.Destroy(_root.gameObject);
        }
    }

    public void Play(SoundType audioType, string key)
    {
        Managers.Resource.LoadAsync<AudioClip>(key, audioClip => Play(audioType, audioClip));
    }

    public void Play(SoundType soundType, AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        var audioSource = _audioSources[(int)soundType];

        if (soundType == SoundType.BGM)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Stop(SoundType soundType)
    {
        _audioSources[(int)soundType].Stop();
    }

    public void ChangeVolume(SoundType soundType, float volume)
    {
        _audioSources[(int)soundType].volume = volume;
    }
}
