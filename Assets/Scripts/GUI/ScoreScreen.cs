using UnityEngine;
using System.Collections;
using System.Linq;

public class ScoreScreen : MonoBehaviour {

    private float height2 = Screen.height / 2;
    private float width2 = Screen.width / 2;
    void OnGUI()
    {
        int Offset = 0;
        var blub = from pl in Hub.Instance.points
                       orderby pl.Value descending
                       select pl;
                       
        foreach (var pl in blub)
        {
            GUI.Label(new Rect(width2 - 40, height2 - 15 + Offset, 80, 40), "Player: " + pl.Key + " Score: " + pl.Value);
            Offset += 50;
        }
        if (GUI.Button(new Rect(width2 - 40, height2 - 15 + Offset, 80, 30), "Main Menu"))
        {
            Application.LoadLevel(0);
        }

    }
}
