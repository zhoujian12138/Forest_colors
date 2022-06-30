using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName="";
    public GameObject StopCanvasPrefab;
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }
    //bool once=true;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject pause = Instantiate(StopCanvasPrefab);
            Time.timeScale = 0f;
            pause.SetActive(true);
            //TODO:解决两次esc暂停界面开启和关闭
           /* if (once) {
                once = false;
                GameObject pause = Instantiate(StopCanvasPrefab);
                Time.timeScale = 0f;
                pause.SetActive(true);
            }
            else {
                once = true;
                //TODO:清除生成的屏幕                           
            }*/
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}