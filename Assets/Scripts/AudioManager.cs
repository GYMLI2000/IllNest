using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Data")]
    public Sound[] musicTracks;
    public Sound[] sfxClips;

    private Coroutine loopCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicTracks, x => x.name == name);
        if (s == null) return;

        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxClips, x => x.name == name);
        if (s == null) return;

        sfxSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(s.clip);
    }

    public void LoopSound(string name, bool play)
    {
        Sound s = System.Array.Find(sfxClips, x => x.name == name);
        if (s == null) return;

        if (play)
        {
            if (loopCoroutine != null) return;

            loopCoroutine = StartCoroutine(LoopCoroutine(s));
        }
        else
        {
            if (loopCoroutine != null)
            {
                StopCoroutine(loopCoroutine);
                loopCoroutine = null;
            }
        }
    }

    private IEnumerator LoopCoroutine(Sound s)
    {
        while (true)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(s.clip);

            float delay = s.clip.length * 0.85f; 
            yield return new WaitForSeconds(delay);
        }
    }
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
