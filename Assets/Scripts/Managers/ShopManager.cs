using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour {

    public GameObject[] pages;
    public GameObject[] circles;
	bool deactivate = false;
	public GameObject objectToDeactivate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadPage(GameObject pageToLoad)
    {
        for(int i = 0; i < pages.Length; i++)
        {
            if(pageToLoad != pages[i])
            {
                pages[i].SetActive(false);
                circles[i].SetActive(false);
            }
            else
            {
                pages[i].SetActive(true);
                circles[i].SetActive(true);
            }
        }
    }

    public void ShowToolTip(GameObject toolTip)
    {
        toolTip.SetActive(true);
    }

    public void HideToolTip(GameObject toolTip)
    {
        toolTip.SetActive(false);
    }

    public void Deactivate(GameObject objectToDeactiveate)
    {
		this.objectToDeactivate = objectToDeactiveate;
		objectToDeactiveate.GetComponent<Animator> ().SetTrigger ("doAnim");
		Invoke ("StopAnimation", 0.4f);
    }

    public void Activate(GameObject objectToActivate)
    {
        objectToActivate.SetActive(true);
    }

	public void StopAnimation()
	{
		objectToDeactivate.SetActive (false);
	}

}
