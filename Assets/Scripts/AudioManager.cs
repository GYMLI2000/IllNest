using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] sfxSources;
    private int sourceIndex = 0;

    [Header("Audio Data")]
    public Sound[] musicTracks;
    public Sound[] sfxClips;

    private Coroutine loopCoroutine;
    private Coroutine musicManagerCoroutine;

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

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxClips, x => x.name == name);
        if (s == null) return;


        AudioSource source = sfxSources[sourceIndex];

        // posunout index pro další pøehrávání, aby se støídaly zdroje
        sourceIndex = (sourceIndex + 1) % sfxSources.Length;

        source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        source.PlayOneShot(s.clip);
    }

    public void StartMusicSystem()
    {
        if (musicManagerCoroutine != null) StopCoroutine(musicManagerCoroutine);
        musicManagerCoroutine = StartCoroutine(MusicPlaylistLoop());
    }

    public void StopMusicSystem()
    {
        if (musicManagerCoroutine != null)
            StopCoroutine(musicManagerCoroutine);
    }

    private IEnumerator MusicPlaylistLoop()
    {
        while (true)
        {
            int randomIndex = Random.Range(1, 4); // zatim 3 ruzny st
            string trackName = "Game" + randomIndex;

            PlayMusic(trackName);

            yield return new WaitForSeconds(musicSource.clip.length * Random.Range(2, 4));
        }
    }

}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
