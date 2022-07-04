using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TimeManagment : MonoBehaviour {

    public GameObject pausebutton;
    public GameObject ffbutton;
    public GameObject playbutton2;
	public GameObject pauseMenu;

	void Start(){
		Time.timeScale = 1.0f;
	}

    public void pauseGame()
    {
        Time.timeScale = 0.0f;
        //playbutton.SetActive(true);
        pausebutton.SetActive(false);
        playbutton2.SetActive(false);
        ffbutton.SetActive(true);
		pauseMenu.SetActive (true);
    }

    public void playGame(string s)
    {
        Time.timeScale = 1.0f;
        if (s == "Play1")
        {
            //playbutton.SetActive(false);
            pausebutton.SetActive(true);
            playbutton2.SetActive(false);
        }else if(s == "Play2")
        {
            playbutton2.SetActive(false);
            ffbutton.SetActive(true);
        }
    }

    public void fastForwardGame()
    {
        Time.timeScale = 2f;
        ffbutton.SetActive(false);
        playbutton2.SetActive(true);
        pausebutton.SetActive(true);
        //playbutton.SetActive(false);
    }

    public void myOnClick(string s)
    {
        if(s == "Pause1")
        {
            pauseGame();
        }else if (s == "Play1" || s == "Play2")
        {
            playGame(s);
        }else if(s == "FF1")
        {
            fastForwardGame();
        }
    }

	public void Resume(){
		pauseMenu.SetActive (false);
		pausebutton.SetActive(true);
		Time.timeScale = 1.0f;
	}

	public void QuitToMenu(){
		SceneManager.LoadScene ("menu");
	}
}
