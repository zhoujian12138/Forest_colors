using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    public AudioClip Theme;

    void Awake()
    {
        instance = this;
        musicSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }

    }

    void Start()
    {
        PlayMusic(Theme, 2);
    }


    public void PlaySound(AudioClip clip, Vector3 pos)
    {

        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, 0.5f);
        }
    }

    public void PlayMusic(AudioClip clip,float fadeDuration)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].loop = true;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }


    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, 1, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(1, 0, percent);
            yield return null;
        }
    }
}
