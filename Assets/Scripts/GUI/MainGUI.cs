using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGUI : MonoBehaviour
{
    class QuickAndDirtyLayer { public BarSprite bs; public TextMesh text; }

    private Dictionary<int, QuickAndDirtyLayer> playerGUI;
    [SerializeField]
    private GameObject
        BarSpritePrefab;
    [SerializeField]
    private GameObject
        TextPrefab;
    
    void Awake()
    {
        playerGUI = new Dictionary<int, QuickAndDirtyLayer>();
    }

    void Update()
    {
    
        var players = CharacterManager.Instance.GetPlayers();

        foreach (var p in players)
        {
            QuickAndDirtyLayer qadl;
            
            if (playerGUI.TryGetValue(p.Key, out qadl))
            {

                qadl.bs.Current = p.Value.Lifes;
                qadl.text.text = "" + CharacterManager.Instance.GetPoints(p.Key);

                var mat = CharacterManager.Instance.GetAssignMaterial(p.Key);

                if (mat != null)
                {
                    (qadl.bs.renderer as SpriteRenderer).color = mat.color;
                }

            }
        }

    }

    public void AddPlayer(int key, Player player)
    {
        if (playerGUI.ContainsKey(key))
            return;

        bool left = playerGUI.Count % 2 == 0;
        bool top = !(playerGUI.Count < 2);

        Vector3 position = new Vector3();

        if (left)
        {
            position.x = -camera.orthographicSize * 0.6f * camera.aspect;
        } else
        {
            position.x = camera.orthographicSize * 0.6f * camera.aspect;
        }
        
        if (top)
        {
            position.y = -camera.orthographicSize * 0.9f;
        } else
        {
   
            position.y = camera.orthographicSize * 0.9f;
        }

        position.z = 1;
        
        var barGO = Instantiate(BarSpritePrefab) as GameObject;

        barGO.transform.parent = transform;
        barGO.transform.localPosition = position;

        var bar = barGO.GetComponent<BarSprite>();
        bar.Max = Hub.Instance.startLifepoints;

        bar.align = TextAlignment.Center;// left ? TextAlignment.Left : TextAlignment.Right;
        bar.ResetBar();

        var pointsGO = Instantiate(TextPrefab) as GameObject;
        pointsGO.transform.parent = transform;
        position.y = position.y * 0.9f;
        pointsGO.transform.localPosition = position;

        var points = pointsGO.GetComponent<TextMesh>();

        playerGUI.Add(key, new QuickAndDirtyLayer {bs = bar, text = points});

    }

    public void PlayerKilled(int key)
    {
        QuickAndDirtyLayer qadl;

        if (playerGUI.TryGetValue(key, out qadl))
        {
            qadl.bs.Current = 0;
        }
    }

    public void RemovePlayer(int key, Player player)
    {
        if (!playerGUI.ContainsKey(key))
            return;

        Destroy(playerGUI [key].bs.gameObject);
        playerGUI.Remove(key);
    }
}
