using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopCanvasUI : MonoBehaviour
{
    Button BackGameBtn;
    Button SaveGameBtn;
    Button QuitGameBtn;

    void Awake()
    {
        BackGameBtn= transform.GetChild(2).GetComponent<Button>();
        SaveGameBtn= transform.GetChild(3).GetComponent<Button>();
        QuitGameBtn= transform.GetChild(4).GetComponent<Button>();

        BackGameBtn.onClick.AddListener(BackGame);
        SaveGameBtn.onClick.AddListener(SaveGame);
        QuitGameBtn.onClick.AddListener(QuitGame);
    }

    void BackGame()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
    }

    void SaveGame()
    {
        SaveManager.Instance.SavePlayerData();
        SceneController.Instance.TransitionToMain();
        Time.timeScale = 1f;
    }

    void QuitGame()
    {
        SceneController.Instance.TransitionToMain();
        Time.timeScale = 1f;
    }
}
