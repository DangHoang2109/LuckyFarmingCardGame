using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneLoader : MonoBehaviour
{
    //Will be remove when have joingame datas and joingamemanager
    public static bool _isTutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        this.StartCoroutine(this.LoadGameScene());
    }

    public void OnClickJoinGameNormal()
    {
        _isTutorial = false;
        JoinGame();
    }
    public void OnClickJoinGameTutorial()
    {
        _isTutorial = true;
        JoinTutorialGame();
    }
    void JoinTutorialGame()
    {
        this.StartCoroutine(this.CheckLoadGameSceneDone());
    }
    void JoinGame(System.Action _onDoneCb = null)
    {
        this.StartCoroutine(this.CheckLoadGameSceneDone(_onDoneCb));
    }
    private IEnumerator CheckLoadGameSceneDone(System.Action _onDoneCb = null)
    {
        //yield return new WaitForEndOfFrame();
        //TempSceneManager.Instance.UnLoadScene(SceneName.HOME);
        yield return new WaitForEndOfFrame();
        TempSceneManager.Instance.ShowScene(SceneName.GAME);
        yield return new WaitForEndOfFrame();
        _onDoneCb?.Invoke();
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false); 
    }
    private IEnumerator LoadGameScene(System.Action _onDoneCb = null)
    {
        yield return TempSceneManager.Instance.PreLoadScene(SceneName.GAME);
        _onDoneCb?.Invoke();
    }
}
