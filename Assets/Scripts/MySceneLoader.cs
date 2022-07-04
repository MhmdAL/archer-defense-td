using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneLoader : MonoBehaviour {

	public static MySceneLoader instance;

	public GameObject defaultLoadingScreenPrefab;

	private bool loading;

	private AsyncOperation ao;

	private GameObject loadingScreen;

	void Awake(){
		DontDestroyOnLoad (gameObject);

		instance = this;
	}

	public void LoadScene(string sceneName){
		StartCoroutine (Load (sceneName));
	}

	IEnumerator Load(string sceneName){
		loading = true;

		loadingScreen = Instantiate (defaultLoadingScreenPrefab) as GameObject;
		loadingScreen.transform.SetParent (FindObjectOfType<Canvas>().transform);
		loadingScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);

		ao = SceneManager.LoadSceneAsync (sceneName);
		ao.allowSceneActivation = false;

		while(!ao.isDone)
		{
			if (ao.progress == 0.9f) {
				yield return new WaitForSeconds (0.5f);
				ao.allowSceneActivation = true;
			}
			yield return null;
		}
	}
}
