using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class MainMenu : MonoBehaviour
{
	public Button startGameBtn;
	public TextMeshProUGUI gameProgressText;

	public AudioManager audioManager;
	public GameObject mainMenu;

	public AudioClip bip;

	private AudioSource source;

	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		UpdateProgressText();

		Addressables.LoadAssetAsync<AudioComposition>(SoundEffects.LEVEL_START);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			PlayerPrefs.SetInt("LastCompletedLevel", 0);
			UpdateProgressText();
		}
	}

	public void ShowProgressText()
	{
		gameProgressText.gameObject.SetActive(true);
	}

	public void HideProgressText()
	{
		gameProgressText.gameObject.SetActive(false);
	}

	private void UpdateProgressText()
	{
		var progress = Mathf.RoundToInt((PlayerPrefs.GetInt("LastCompletedLevel") / 3f) * 100);
		gameProgressText.text = $"Progress: {progress}%";

		Debug.Log(PlayerPrefs.GetInt("LastCompletedLevel"));
	}

	public void Load(string levelToLoad)
	{
		GlobalManager.GlobalState["InitialLevel"] = (PlayerPrefs.GetInt("LastCompletedLevel") + 1) % 4;

		GlobalManager.instance.LoadScene(levelToLoad, 1f);

		// SceneManager.LoadScene(levelToLoad);
	}

	public void PlayBip()
	{
		source.PlayOneShot(bip, GlobalManager.GlobalVolumeScale);
	}

	public void SwitchMenu(int x)
	{
		/*if (x == 1) {
			levelMenu.SetActive (true);
			mainMenu.SetActive (false);
		} else if (x == 2) {
			levelMenu.SetActive (false);
			mainMenu.SetActive (true);
		}*/
	}

	void OnLevelWasLoaded(int level)
	{
		Time.timeScale = 1;
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
