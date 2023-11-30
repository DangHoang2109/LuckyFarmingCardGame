using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneManager : MonoSingleton<TempSceneManager>
{
    private string curSceneName;
    public string CurrentSceneName => curSceneName;
    public Dictionary<string, BaseScene> scenes = new Dictionary<string, BaseScene>();

    public IEnumerator PreLoadScene(string sceneName, System.Action onComplete = null)
    {
        yield return new WaitForEndOfFrame();
        BaseScene s = this.CreateScene(sceneName);
        if (s != null)
        {
            s.UnloadScene();
        }
        yield return new WaitForEndOfFrame();
    }

    public void UnLoadScene(string sceneName)
    {
        if (this.scenes.TryGetValue(sceneName, out BaseScene scene))
        {
            scene.UnloadScene();
        }
    }

    public void ShowScene(string sceneName, System.Action callback = null)
    {
        if (this.scenes.TryGetValue(sceneName, out BaseScene scene))
        {
            scene.transform.SetAsLastSibling();
            scene.StartScene();
            this.curSceneName = sceneName;
        }
    }

    private BaseScene CreateScene(string sceneName)
    {
        if (this.scenes.TryGetValue(sceneName, out BaseScene scene))
        {
            scene.gameObject.SetActive(true);
            scene.InitScene();
            return scene;
        }

        var prefab = LoaderUtility.Instance.GetAsset<BaseScene>($"Scenes/{sceneName}");
        if (prefab != null)
        {
            scene = Instantiate(prefab, this.transform);
            scene.gameObject.SetActive(true);
            scene.InitScene();
            this.scenes.Add(sceneName, scene);
            return scene;
        }

        return null;
    }
    
}
