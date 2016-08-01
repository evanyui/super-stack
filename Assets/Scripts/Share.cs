using UnityEngine;
using System.Collections;

public class Share : MonoBehaviour {
	private const string TITLE   = "Challange Me";
	private const string MESSAGE = "Can you beat me?";

	public void shareScreenShot() {
		StartCoroutine (FrispGames.Social.ScreenshotSharer.Instance().PostScreenshot(TITLE, MESSAGE));
	}

	public void rate()
	{
		print("Please Support Us" + Application.bundleIdentifier);
		Application.OpenURL("http://play.google.com/store/apps/details?id=com.evanyui.superstack" + Application.bundleIdentifier);

	}
}