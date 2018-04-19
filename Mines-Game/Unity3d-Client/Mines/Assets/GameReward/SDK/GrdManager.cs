using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrdManager : MonoBehaviour
{
    private delegate void ApiEventHandler(string data);
    private const string apiUrl = "http://www.gamereward.io/appapi/";
    private static readonly string apiId = "060fb1767e03af2c1028c5a3cd2f8f4f7feafd82";
    private static readonly string apiSecret = "2723d092b63885e0d7c260cc007e8b9d2f2b265625d76a6704b08093c652fd799c82c7143c102b71c593d98d96093fde8d701f4cf6278b7a09134c0ef81dabdf";
    private static GrdManager instance;
    private string token = "";
    /// <summary>
    /// User information
    /// </summary>
    public GrdUser User
    {
        get
        {
            return user;
        }
    }
    private GrdUser user;
    public static GrdManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject g = new GameObject();
                g.name = "GrdManager";
                DontDestroyOnLoad(g);
                instance = g.AddComponent<GrdManager>();
            }
            return instance;
        }
    }
    private IEnumerator Post(ApiEventHandler callback, string action, Dictionary<string, string> pars = null)
    {
        WWWForm wwwForm = new WWWForm();
        foreach (string key in pars.Keys)
        {
            wwwForm.AddField(key, pars[key]);
        }
        wwwForm.AddField("api_id", apiId);
        wwwForm.AddField("api_key", GetApiKey());
        if (token.Length > 0)
            wwwForm.AddField("token", token);
        WWW www = new WWW(apiUrl + action, wwwForm);
        yield return www;
        callback(www.text);
    }

    private string GetApiKey()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int t = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        t = t / 15;
        var k = t % 20;
        var len = apiSecret.Length / 20;
        var str = apiSecret.Substring(k * len, len);
        str = Md5Sum(str) + Md5Sum(t.ToString());
        return str;
    }
    private IEnumerator Get(ApiEventHandler callback, string action, Dictionary<string, string> pars = null)
    {
        string url = apiUrl + action;
        if (pars != null && pars.Count > 0)
        {
            url += "/";
            foreach (string key in pars.Keys)
            {
                url += key + "=" + pars[key] + "&";
            }
            url = url + "api_id=" + apiId + "&api_key=" + GetApiKey();
            if (token.Length > 0)
            {
                url += "&token=" + token;
            }
        }
        WWW www = new WWW(url);
        yield return www;
        callback(www.text);
    }
    private static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
    private bool IsActionSuccess(Dictionary<string, object> result)
    {
        return result.ContainsKey("error") && result["error"].ToString() == "0";
    }
    private Dictionary<string, object> GetObjectData(string data)
    {
        Dictionary<string, object> result = null;
        try
        {
            result = (Dictionary<string, object>)MiniJSON.Json.Deserialize(data);
        }
        catch
        {
        }
        if (result == null)
        {
            result = new Dictionary<string, object>();
            result.Add("error", 100);
            result.Add("message", data);
        }
        return result;
    }
    /// <summary>
    /// LogOut user from system.
    /// </summary>
    /// <param name="callback"></param>
    public void LogOut(GrdEventHandler callback)
    {
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), result["message"]);
            }
        }, "logout", null));
    }
    /// <summary>
    /// This method use to reset password for user. System will send an email change password for user
    /// </summary>
    /// <param name="username">Username user use to login</param>
    /// <param name="password">Password user use to login</param>
    /// <param name="callback">Call when completed.</param>
    public void ResetPassword(string usernameOrEmail,  GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        pars.Add("email", usernameOrEmail);
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), result["message"]);
            }
        }, "requestresetpassword", pars));
    }
    /// <summary>
    /// This function use to register a new user
    /// </summary>
    /// <param name="username">Username use to login</param>
    /// <param name="password">Password of the account</param>
    /// <param name="email">Email use for reseting password or receiving the message from app.</param>
    /// <param name="userdata">Any data in string</param>
    /// <param name="callback">Call when finish.</param>
    public void Register(string username, string password, string email, string userdata, GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        pars.Add("username", username);
        pars.Add("password", Md5Sum(password));
        pars.Add("email", email);
        pars.Add("userdata", userdata);
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), result["message"]);
            }
        }, "createaccount", pars));
    }

    /// <summary>
    /// This method use for user login to system. Server response the result is success or failed.
    /// </summary>
    /// <param name="username">Username user use to login</param>
    /// <param name="password">Password user use to login</param>
    /// <param name="callback">Call when completed.</param>
    public void Login(string username, string password,string otp, GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        pars.Add("username", username);
        pars.Add("password", Md5Sum(password));
        pars.Add("otp", otp);
        StartCoroutine(Post((data) =>
        {
            object r=null;
            Dictionary<string, object> result = GetObjectData((string)data);
            if (result["error"].ToString() == "0")
            {
                user = new GrdUser();
                user.username = username;
                user.address = result["address"].ToString();
                user.email = result["email"].ToString();
                user.balance = decimal.Parse(result["balance"].ToString());
                user.isRequiredOtp = result["isRequiredOtp"].ToString() == "1";
                token = result["token"].ToString();
                r=user;
            }
            else{
                r=result["message"];
            }
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), r);
            }
        }, "login", pars));
    }
    private static void GetImageSize(byte[] imageData, out int width, out int height)
    {
        width = ReadInt(imageData, 3 + 15);
        height = ReadInt(imageData, 3 + 15 + 2 + 2);
    }

    private static int ReadInt(byte[] imageData, int offset)
    {
        return (imageData[offset] << 8) | imageData[offset + 1];
    }
    public void GetAddressQRCode(string address, GrdEventHandler callback)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("address", address);
        StartCoroutine(Get((responseText)=>{
            responseText = responseText.Replace("data:image/image/png;base64,", "");
            byte[] array = System.Convert.FromBase64String(responseText);
            int width, height;
            int error = 0;
            Texture2D texture=null;
            try
            {
                GetImageSize(array, out width, out height);
                texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
                texture.hideFlags = HideFlags.HideAndDontSave;
                texture.filterMode = FilterMode.Point;
                texture.LoadImage(array);
            }
            catch
            {
                error = 1;
            }
            if (callback != null)
            {
                callback(error, texture);
            }
        }, "qrcodeaddress",dic));
    }

    /// <summary>
    /// Call the server script to do logic game on server
    /// </summary>
    /// <param name="scriptName">The name of the script defined on server</param>
    /// <param name="functionName">The name of function you want to call. If the script have return statement in global scope, the functionName can be empty</param>
    /// <param name="parameters">The parameters to pass to the function</param>
    /// <param name="callback">Call when server response result.</param>
    public void CallServerScript(string scriptName,string functionName,object[]parameters, GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        pars.Add("vars", MiniJSON.Json.Serialize(parameters));
        pars.Add("fn", functionName);
        pars.Add("script", scriptName);
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            int error = int.Parse(result["error"].ToString());
            object rdata=null;
            if(error!=0){
                rdata=result["message"];
            }
            else{
                rdata=result["result"];
            }
            if (callback != null)
            {
                callback(error, rdata);
            }
        }, "callserverscript", pars));
    }
  
    private static string FormatNumber(decimal number)
    {
        char[] array= number.ToString().ToCharArray();
        bool isDecimal = false;
        for (int i = array.Length - 1; i > 0; i--)
        {
            if (!isDecimal)
            {
                if (!char.IsDigit(array[i]))
                {
                    array[i] = '.';
                    isDecimal = true;
                }
            }
            else
            {
                if (!char.IsDigit(array[i]))
                {
                    array[i] = ' ';
                    isDecimal = true;
                }
            }
        }
        string result = new string(array);
        result = result.Replace(" ","");
        return result;
    }
    /// <summary>
    /// Get the newest user balance from server update to user object.
    /// </summary>
    /// <param name="callback"></param>
    public void UpdateBalance(GrdEventHandler callback)
    {
        StartCoroutine(Get((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            object r = null;
            if (result["error"].ToString() == "0")
            {
                if (result.ContainsKey("balance"))
                {
                    User.balance = decimal.Parse(result["balance"].ToString());
                }
                r = data;
            }
            else
            {
                r = result["message"].ToString();
            }
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), r);
            }
        }, "accountbalance"));
    }
    /// <summary>
    /// Transfer user money to another wallet
    /// </summary>
    /// <param name="toAddress">Address of the wallet to transfer to</param>
    /// <param name="money">The amount of money to tranfer</param>
    /// <param name="callback">Call when the transfer finished!</param>
    public void Transfer(string toAddress,decimal money,string otp, GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        pars.Add("to", toAddress);
        pars.Add("value", FormatNumber(money));
        pars.Add("otp", otp);
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            if (result["error"].ToString() == "0")
            {
                if (!result.ContainsKey("balance"))
                {
                    User.balance -= money;
                }
                else
                {
                    User.balance = decimal.Parse(result["balance"].ToString());
                }
            }
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), result["message"].ToString());
            }
        }, "transfer", pars));
    }
    /// <summary>
    /// Use when user want to turn on the 2 steps verification.
    /// 
    /// </summary>
    /// <param name="callback">Call when the request is completed and return the result</param>
    public void RequestEnableOtp(GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            if (callback != null)
            {
                callback(int.Parse(result["error"].ToString()), result["message"].ToString());
            }
        }, "requestotp", pars));
    }
    /// <summary>
    /// Allow user enable or disable the 2 steps verification security options
    /// </summary>
    /// <param name="otp"></param>
    /// <param name="enabled"></param>
    /// <param name="callback"></param>
    public void EnableOtp(string otp,bool enabled,GrdEventHandler callback)
    {
        Dictionary<string, string> pars = new Dictionary<string, string>();
        pars.Add("otp", otp);
        StartCoroutine(Post((data) =>
        {
            Dictionary<string, object> result = GetObjectData((string)data);
            int error=int.Parse(result["error"].ToString());
            if (error==0)
            {
                User.isRequiredOtp = enabled;
            }
            if (callback != null)
            {
                callback(error, result["message"].ToString());
            }
        }, "enableotp", pars));
    }

}
public delegate void GrdEventHandler(int error,object data);
public class GrdUser
{
    public string username;
    public string email;
    public string address;
    public decimal balance;
    public bool isRequiredOtp;
}
