using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToHome : MonoBehaviour {

	[DllImport("__Internal")]
	private static extern void GoToHome();

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);	
	}
	
    public void OnButtonClick(){
        GoToHome();
    }
}
