using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour
{


    void Start()
    {
        Time.timeScale = 0;
        Screen.lockCursor = false;
    }

    void DidLockCursor()
    {
        Debug.Log("Locking cursor");

        // Disable the button
        //guiTexture.enabled = false;
    }

    // Called when the cursor is being unlocked
    // or by a script calling Screen.lockCursor = false;
    void DidUnlockCursor()
    {
        Debug.Log("Unlocking cursor");
        Time.timeScale = 0;
        // Show the button again
        // guiTexture.enabled = true;
    }

    private bool wasLocked = false;

    void Update()
    {
        // In standalone player we have to provide our own key
        // input for unlocking the cursor
        if (Input.GetKeyDown("escape"))
            Screen.lockCursor = false;

        // Did we lose cursor locking?
        // eg. because the user pressed escape
        // or because he switched to another application
        // or because some script set Screen.lockCursor = false;
        if (!Screen.lockCursor && wasLocked)
        {
            wasLocked = false;
            DidUnlockCursor();
        }
        // Did we gain cursor locking?
        else if (Screen.lockCursor && !wasLocked)
        {
            wasLocked = true;
            DidLockCursor();
        }
    }

    void OnGUI()
    {
        if (Screen.lockCursor == false)
        {
            if (GUI.Button(new Rect(Screen.width - 120, 20, 100, 30), "Lock Cursor"))
            {
                Screen.lockCursor = true;
                Time.timeScale = 1;
            }
        }
    }
}