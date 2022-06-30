using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    public GameObject deathCanvasUIPrefab;
    bool fadeFinished;

    GameObject player;
    NavMeshAgent playerAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

 

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        SaveManager.Instance.SavePlayerData();
        //InventoryManager.Instance.SaveData();
        //QuestManager.Instance.SaveQuestManager();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return StartCoroutine(fade.FadeOut(0.5f));
            yield return SceneManager.LoadSceneAsync (sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fade.FadeIn(0.5f));
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            yield return StartCoroutine(fade.FadeOut(0.5f));
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield return StartCoroutine(fade.FadeIn(0.5f));
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToFirstLevel()
    {
        
        StartCoroutine(LoadLevel("testScene"));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(0.5f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            SaveManager.Instance.SavePlayerData();
            //InventoryManager.Instance.SaveData();
            yield return StartCoroutine(fade.FadeIn(0.5f));
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        //yield return new WaitForSeconds(2);
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(0.5f));
        yield return SceneManager.LoadSceneAsync("MainMenu");
        yield return StartCoroutine(fade.FadeIn(0.5f));
        yield break;
    }


    public void EndNotify()
    {
        if (fadeFinished)
        {           
            fadeFinished = false;
            GameObject death= Instantiate(deathCanvasUIPrefab);
            death.SetActive(true);
        }
    }
}
