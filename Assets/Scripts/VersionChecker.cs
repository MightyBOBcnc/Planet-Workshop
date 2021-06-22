using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    public static string path = "https://raw.githubusercontent.com/carlpilot/Planet-Workshop/main/version.txt";

    public static string CurrentVersion = "1";

    public GameObject newVersionNotification;
    public Text newVersionMessage;
    public Text versionDisplay;

    private void Start () {
        StartCoroutine (WwwRequestVersion ());
    }

    IEnumerator WwwRequestVersion () {
        WWW www = new WWW (path);

        yield return www;// new WaitUntil (() => www.isDone);

        string[] lines = www.text.Split (new char[] { '\n' }, 2);

        if(lines[0] != CurrentVersion) {
            Debug.Log ("Update needed: version available: (" + lines[0] + ") vs current version: (" + CurrentVersion + ")");
            newVersionNotification.SetActive (true);
            newVersionMessage.text = lines[1];
            versionDisplay.text = string.Format ("Current Version: {0}\tNew Version: {1}", CurrentVersion, lines[0]);
        } else {
            Debug.Log ("Up to date");
        }
    }
}
