using UnityEngine;
# if PLATFORM_ANDROID
using UnityEngine.Android;
# endif

public class PermissionsRationaleDialog : MonoBehaviour
{
    public PermissionManager permissionManager;
    const int kDialogWidth = 600;
    const int kDialogHeight = 200;
    private bool windowOpen = true;

    public void OnGUI()
    {
        if (windowOpen)
        {
            Rect rect = new Rect((Screen.width / 2) - (kDialogWidth / 2), (Screen.height / 2) - (kDialogHeight / 2), kDialogWidth, kDialogHeight);
            GUI.ModalWindow(0, rect, DoMyWindow, "Permissions Request Dialog");
        }
    }

    void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(20, 40, kDialogWidth - 20, kDialogHeight - 50), "Please let me use the microphone.");
        GUI.Button(new Rect(20, kDialogHeight - 30, 200, 40), "No");
        if (GUI.Button(new Rect(kDialogWidth - 110, kDialogHeight - 30, 200, 40), "Yes"))
        {
#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.Microphone);
#endif
            windowOpen = false;
        }
    }
}