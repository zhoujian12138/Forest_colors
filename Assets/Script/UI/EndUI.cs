using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndUI : MonoBehaviour
{
    Button QuitGameBtn;
    public Text endgame;

    void Awake()
    {
        QuitGameBtn = transform.GetChild(1).GetComponent<Button>();
        QuitGameBtn.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        endgame.DOText("勇敢的骑士啊！  恭喜你击败了神秘法师!这片森林终于脱离了他的控制，你为森林间的生物追回了自由，为这片森林带来了希望的曙光！你终于可以回到狗狗王国了！",10f);
    }

    void QuitGame()
    {
        SceneController.Instance.TransitionToMain();
        Time.timeScale = 1f;
    }
}
