using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    AudioSource[] musicSources;
    

    public static AudioManager instance;
    public EnemyController boss;

    public AudioClip Theme;
    public AudioClip Battle;

    bool isPlayBGM;
    bool isPlayBattleMusic;

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
        musicSources[0].clip = Theme;
        musicSources[1].clip = Battle;
        PlayMusic(0, 2);
        isPlayBGM = true;
        isPlayBattleMusic = false;
    }
    
    void Update()
    {
        if (boss.enemyStates == EnemyStates.CHASE)
        {
            if (!isPlayBattleMusic && isPlayBGM)
            {
                StopMusic(0, 4);
                isPlayBGM = false;
                PlayMusic(1, 4);
                isPlayBattleMusic = true;
            }
        }
        else
        {
            if (isPlayBattleMusic && !isPlayBGM)
            {
                StopMusic(1, 4);
                isPlayBattleMusic = false;
                PlayMusic(0, 4);
                isPlayBGM = true;
            }
        }
    }

   

    public void PlaySound(AudioClip clip, Vector3 pos)
    {

        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, 1);
        }
    }

    public void PlayMusic(int AudioNum, float fadeDuration)
    {
        musicSources[AudioNum].loop = true;
        
        musicSources[AudioNum].Play();
        StartCoroutine(playFade(AudioNum,fadeDuration));
    }

    public void StopMusic(int AudioNum, float fadeDuration)
    {
        musicSources[AudioNum].Stop();
        StartCoroutine(stopFade(AudioNum, fadeDuration));
    }

    IEnumerator stopFade(int fadeNum, float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[fadeNum].volume = Mathf.Lerp(0.2f, 0, percent);
            yield return null;
        }
    }


    IEnumerator playFade(int fadeNum,float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[fadeNum].volume = Mathf.Lerp(0, 0.2f, percent);
            yield return null;
        }
    }
}
