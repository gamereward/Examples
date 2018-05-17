using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Grd;
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
    public Text message;
	public bool isInLogin;

	private void Start()
	{
		isInLogin = true;
		buttonLogin.SetActive(true);
		buttonRegister.SetActive(false);
        GrdManager.Init("060fb1767e03af2c1028c5a3cd2f8f4f7feafd82","2723d092b63885e0d7c260cc007e8b9d2f2b265625d76a6704b08093c652fd799c82c7143c102b71c593d98d96093fde8d701f4cf6278b7a09134c0ef81dabdf");
	}

	public void SwitchScreen()
	{
        message.text = "";
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
		string password = inputPass.text;
        GrdManager.Login(email,password,"",(error, args) => {
            if(error==0){
                SceneManager.LoadScene("GRDMines");
            }
			else
			{
                message.text = args.ErrorMessage;
			}
        });
	}

	public void Register()
	{
		string email = inputEmail.text;
		string password = inputPass.text;
        GrdManager.Register(email,password,email,"",(error, args) => {
            if(error==0){
                SwitchScreen();
            }else{
                message.text = args.ErrorMessage;
            }
        });
		
	}
}
