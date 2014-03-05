using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private float height2 = Screen.height / 2;
    private float width2 = Screen.width / 2;
    void OnGUI()
    {
        if (GUI.Button(new Rect(width2 - 40, height2 - 15, 80, 30), "Play"))
        {
            Application.LoadLevel(1);
        }
        if (GUI.Button(new Rect(width2 - 40, height2 + 30, 80, 30), "Exit"))
        {
            Application.Quit();
        }
    }
}