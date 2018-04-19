using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class Wallet : MonoBehaviour {

    public Text address;
    public Image qrImage;
    public InputField addressInput;
    public InputField amount;
    public Text message;
	// Use this for initialization
	void Start () {
        address.text = GrdManager.Instance.User.address;
		Texture2D barcode = generateQR("gamereward:" + GrdManager.Instance.User.address);
		qrImage.sprite = Sprite.Create(barcode, new Rect(0, 0, barcode.width, barcode.height), new Vector2(0, 0), 1);

	}

	public Texture2D generateQR(string text)
	{
		var encoded = new Texture2D(256, 256);
		var color32 = Encode(text, encoded.width, encoded.height);
		encoded.SetPixels32(color32);
		encoded.Apply();
		return encoded;
	}

	private static Color32[] Encode(string textForEncoding,
	int width, int height)
	{
		var writer = new BarcodeWriter
		{
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions
			{
				Height = height,
				Width = width
			}
		};
		return writer.Write(textForEncoding);
	}

    public void OnClose(){
        gameObject.SetActive(false);
    }

    public void Withdraw(){
        if(addressInput.text!=null&& addressInput.text.StartsWith("0x", System.StringComparison.Ordinal)&&amount.text!=null&&decimal.Parse(amount.text)>0)
        {
            GrdManager.Instance.Transfer(addressInput.text,decimal.Parse(amount.text),"",(error, data) => {
                if(error==0){
                    message.text = "Success!";
                }else{
                    message.text = data.ToString();
                }
            });
        }else{
            message.text = "Wrong address or amount!";
        }
    }
}
