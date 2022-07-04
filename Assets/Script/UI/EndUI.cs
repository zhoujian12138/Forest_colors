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
        endgame.DOText("�¸ҵ���ʿ����  ��ϲ����������ط�ʦ!��Ƭɭ���������������Ŀ��ƣ���Ϊɭ�ּ������׷�������ɣ�Ϊ��Ƭɭ�ִ�����ϣ������⣡�����ڿ��Իص����������ˣ�",10f);
    }

    void QuitGame()
    {
        SceneController.Instance.TransitionToMain();
        Time.timeScale = 1f;
    }
}
