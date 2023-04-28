using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableSound : MonoBehaviour
{
    public AudioClip clip
    {
        set
        {
            _clipTime = value.length;
            _audioSource.clip = value;
        }
    }

    private AudioSource _audioSource;
    private float _elapsedTime = 0f;
    private float _clipTime = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _elapsedTime = 0f;
        _audioSource.Play();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if(_clipTime <= _elapsedTime)
        {
            SoundStore.ReleaseSound(this);
        }
    }
}
