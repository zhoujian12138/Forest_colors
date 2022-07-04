using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCanvasUI : MonoBehaviour
{
    Button QuitGameBtn;
    public AudioClip clip;
    

    void Awake()
    {
        
        QuitGameBtn = transform.GetChild(3).GetComponent<Button>();
        QuitGameBtn.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
       
        SceneController.Instance.TransitionToMain();        
    }
}
