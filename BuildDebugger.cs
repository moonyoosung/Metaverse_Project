using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildDebugger : MonoBehaviour
{
    public Text text;
    string myLog = "*begin log";
    string filename = "";
    bool doShow = true;
    int kChars = 700;
    private void Awake() { }
    void OnEnable()
    {
#if UNITY_EDITOR
#else
        Application.logMessageReceived += Log;
#endif
    }
    void OnDisable()
    {
#if UNITY_EDITOR
#else
        Application.logMessageReceived -= Log;
#endif
    }
    void Update() { if (Input.GetKeyDown(KeyCode.Space)) { doShow = !doShow; } }

    public void Log(string logString, string stackTrace, LogType type)
    {

        // for onscreen...
        myLog = type.ToString() + "\n" + myLog + "\n" + logString + "\n"+stackTrace + "\n";
        
        if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }
        if (text != null)
        {
            text.text = myLog;
        }
        // for the file ...
        if (filename == "")
        {
            string d = System.Environment.GetFolderPath(
               System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
#if UNITY_ANDROID
            d = Application.persistentDataPath + "/Your_Logs";
#endif
            System.IO.Directory.CreateDirectory(d);
            string r = Random.Range(1000, 9999).ToString();
            filename = d + "/log-" + r + ".txt";
        }
        try { System.IO.File.AppendAllText(filename, type.ToString() + "\n"+logString + "\n" + stackTrace+ "\n"); }
        catch { }
    }

    void OnGUI()
    {
#if UNITY_EDITOR
#else
        if (!doShow) { return; }
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
           new Vector3(Screen.width / 900.0f, Screen.height / 600.0f, 1.0f));
        GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
#endif
    }
}
