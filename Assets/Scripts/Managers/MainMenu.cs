using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	public AudioManager audioManager;
	public GameObject mainMenu;

	public AudioClip bip;

	private AudioSource source;

	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	public void Load(string levelToLoad)
	{
		GlobalManager.instance.LoadScene(levelToLoad, 1f);

		// SceneManager.LoadScene(levelToLoad);
	}

	public void PlayBip()
	{
		source.PlayOneShot(bip);
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
