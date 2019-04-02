using UnityEngine;
using UnityEngine.UI;

public class Exit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void OnClick()
    {
        Application.Quit();
    }
}
