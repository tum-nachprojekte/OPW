using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;

public class CharacterManager : MonoBehaviour
{

    //    private abstract class Player : MonoBehaviour
    //    {
    //        public void Move(object stuff);
    //
    //        public void Aim(object stuff);
    //    };

    private static CharacterManager instance;

    public static CharacterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("_CharacterManager").GetComponent<CharacterManager>();
            }

            return instance;
        }
    }

    [SerializeField]
    private GameObject
        playerPrefab;
    [SerializeField]
    private Transform[]
        spawns;
    [SerializeField]
    private List<Material>
        playerColors;
    private Dictionary<int, Player> players;
    private Dictionary<int, Material> assignedColors;
    private Dictionary<int, int> points;

    private void Awake()
    {
        instance = this;

        players = new Dictionary<int, Player>();
        assignedColors = new Dictionary<int, Material>();
        points = new Dictionary<int, int>();

        InputManager.EnableXInput = true;
        InputManager.Setup();
        InputManager.OnDeviceDetached += DeviceDeatached;
        // InputManager.AttachDevice(new UnityInputDevice(new FPSProfile()));

    }

    public void KillPlayer(GameObject playerGO)
    {
        var player = playerGO.GetComponent<Player>();

        KillPlayer(player);
    }

    public void KillPlayer(Player player)
    {
        if (player != null)
        {
            var matches = players.Where(p => p.Value == player).ToList();
            if (matches.Count > 0)
            {
                var id = matches.Select(e => e.Key).First();
                GetMainGUI().PlayerKilled(id);

                RemovePlayer(id);
            }
        }
    }

    public Dictionary<int, Player> GetPlayers()
    {
        return players;
        //        return players.Select(s => s.Value).ToArray();
    }

    public Material GetAssignMaterial(int id)
    {
        if (assignedColors.ContainsKey(id))
        {
            return assignedColors[id];
        }

        return null;
    }

    public int GetPoints(int key)
    {
        try
        {
            return points[key];
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    private CameraScript GetCurrentCamera()
    {
        return Camera.main.GetComponent<CameraScript>();
    }

    private MainGUI GetMainGUI()
    {
        return Camera.allCameras.Where(c => c.name.Contains("GUI")).First().GetComponent<MainGUI>();
    }

    void RemovePlayer(int key)
    {
        Player player = null;

        if (!players.TryGetValue(key, out player))
            return;

        GetCurrentCamera().RemovePlayer(player.transform);

        Destroy(player.gameObject);

        players.Remove(key);
    }

    void DeviceDeatached(InputDevice device)
    {
        var player = players.Where(p => p.Key == device.SortOrder).Select(p => p.Value).FirstOrDefault();

        if (player != null)
        {
            var mainGui = GetMainGUI();

            mainGui.RemovePlayer(device.SortOrder, player);
        }

        RemovePlayer(device.SortOrder);

        if (assignedColors.ContainsKey(device.SortOrder))
        {
            assignedColors.Remove(device.SortOrder);
        }

        if (points.ContainsKey(device.SortOrder))
        {
            points.Remove(device.SortOrder);
        }
    }
    public void AddPoints(GameObject playerGO)
    {
        if (playerGO == null) return;
        var player = playerGO.GetComponent<Player>();

        var playerObj = from p in players
                        where p.Value == player
                        select p;

        if (playerObj.Count() != 0)
        {
            var id = playerObj.Select(p => p.Key).First();

            if (points.ContainsKey(id))
            {
                points[id] += 1;

                if (points[id] > Hub.Instance.NumPointsToWin)
                {
                    Hub.Instance.points = points;
                    Application.LoadLevel(2);
                }
            }

        }
    }
    void AddNewPlayer(InputDevice device)
    {
        Vector3 spawn = Vector3.zero;

        if (spawns.Length > 0)
        {
            spawn = spawns[Random.Range(0, spawns.Length)].position;
        }

        var playerGO = Instantiate(playerPrefab, spawn, Quaternion.identity) as GameObject;

        var player = playerGO.GetComponent<Player>();
        players[device.SortOrder] = player;

        GetCurrentCamera().SetPlayer(player.transform);

        var mainGui = GetMainGUI();

        mainGui.AddPlayer(device.SortOrder, player);

        if (!assignedColors.ContainsKey(device.SortOrder))
        {
            var mat = playerColors[assignedColors.Count % playerColors.Count];
            assignedColors.Add(device.SortOrder, mat);
        }

        if (!points.ContainsKey(device.SortOrder))
        {
            points.Add(device.SortOrder, 0);
        }

        player.SetMaterial(assignedColors[device.SortOrder]);
    }

    void FixedUpdate()
    {
        InputManager.Update();

        foreach (var device in InputManager.Devices)
        {
            var id = device.SortOrder;

            Player player = null;

            if (players.TryGetValue(id, out player))
            {
                var moveAmount = device.LeftStickX;

                if (moveAmount > 0)
                {
                    player.moveRight(moveAmount);
                }
                else
                {
                    player.moveLeft(-moveAmount);
                }

                player.aim(device.RightStickVector);

                if (device.LeftBumper.WasPressed)
                {
                    player.jump();
                }

                if (device.LeftTrigger.WasPressed)
                {
                    player.jump();
                }
                else if (device.LeftTrigger.IsPressed)
                {
                    player.jetpack(device.LeftTrigger.Value);
                }
                else
                {
                    player.jetpackStop();
                }

                if (device.RightBumper.IsPressed)
                {
                    player.shoot();
                }

                if (device.RightTrigger.IsPressed)
                {
                    player.shootMega();
                }
                else if (device.RightTrigger.WasReleased)
                {
                    player.releaseShoot();
                }

                player.SetMaterial(assignedColors[device.SortOrder]);
            }
            else
            {

                var startButton = device.Buttons.Where(b => b.Handle.ToLower().Contains("start")).FirstOrDefault();

                if (startButton == null || startButton.WasPressed)
                {
                    AddNewPlayer(device);
                }
            }
        }
    }

}
