using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GRDLogin : MonoBehaviour
{

	public Text title;
	public InputField inputEmail;
	public InputField inputPass;
	public GameObject buttonLogin;
	public GameObject buttonRegister;
	public Text registerLink;
	public bool isInLogin;

	private void Start()
	{
		isInLogin = true;
		buttonLogin.SetActive(true);
		buttonRegister.SetActive(false);
	}

	public void SwitchScreen()
	{
		if (isInLogin)
		{
			isInLogin = false;
			buttonLogin.SetActive(false);
			buttonRegister.SetActive(true);
			title.text = "Register";
			registerLink.text = "Already have an account?";
		}
		else
		{
			isInLogin = true;
			buttonLogin.SetActive(true);
			buttonRegister.SetActive(false);
			title.text = "Login";
			registerLink.text = "Not registered yet!";
		}
	}

	public void Login()
	{
		string email = inputEmail.text;
		string password = Utils.Md5Sum(inputPass.text);
        GrdManager.Instance.Login(email,password,"",(error, data) => {
            if(error==0){
                SceneManager.LoadScene("GRDMines");
            }
			else
			{
                Debug.Log(data.ToString());
			}
        });
	}

	public void Register()
	{
		string email = inputEmail.text;
		string password = Utils.Md5Sum(inputPass.text);
        GrdManager.Instance.Register(email,password,email,"",(error, data) => {
            if(error==0){
                SwitchScreen();
            }else{
                Debug.Log(data.ToString());
            }
        });
		
	}
}
