using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToHome : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);	
	}
	
    public void OnButtonClick(){
        SceneManager.LoadScene("GRDLogin");
    }
}
