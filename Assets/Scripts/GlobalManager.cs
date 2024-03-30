using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTimer;

public class GlobalManager : MonoBehaviour
{
    public static float GlobalVolumeScale = 1.0f;

    public static Dictionary<string, object> GlobalState = new Dictionary<string, object>();
    
    public LoadingScreen loadingScreen;

    private static GlobalManager _instance;
    public static GlobalManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalManager>();

                if (_instance == null)
                {
                    var managerPrefab = Resources.Load<GameObject>("GlobalManager");

                    if (managerPrefab != null)
                    {
                        var go = Instantiate(managerPrefab);
                        _instance = go.GetComponent<GlobalManager>();
                        go.name = "GlobalManager";
                        DontDestroyOnLoad(go);
                    }
                    else
                    {
                        Debug.LogError("GlobalManager prefab not found!");
                    }
                }
            }

            return _instance;
        }
    }

    public void ShowLoadingScreen(float duration = 1f)
    {
        loadingScreen.gameObject.SetActive(true);

        loadingScreen.FadeInOut(duration);
    }

    public void LoadScene(string name, float duration = 1f)
    {
        StartCoroutine(LoadSceneAsync(name, duration));
    }

    private IEnumerator LoadSceneAsync(string name, float duration)
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.FadeIn();

        yield return new WaitForSeconds(1f);

        var result = SceneManager.LoadSceneAsync(name);

        yield return new WaitUntil(() => result.isDone);

        loadingScreen.FadeOut();
    }
}
