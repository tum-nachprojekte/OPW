using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraScript : MonoBehaviour
{

    private List<Transform> Players;
    private float MinDistance;
    private float FixLerp = 0.2f;

    void Start()
    {
        Players = new List<Transform>(4);
        MinDistance = -GameObject.Find("Hub").GetComponent<Hub>().MinDistance;
    }
    // Checks if player is to close to the edge
    void ApplyCameraCorrection()
    {
        if (Players.Count == 0) return;
        Vector2 Min = new Vector2(100f, 100f);
        Vector2 Max = new Vector2(-100f, -100f);
        Vector2 Mid = new Vector3(0, 0);
        foreach (Transform p in Players)
        {
            Min.y = Mathf.Min(p.position.y, Min.y);
            Max.y = Mathf.Max(p.position.y, Max.y);
            Min.x = Mathf.Min(p.position.x, Min.x);
            Max.x = Mathf.Max(p.position.x, Max.x);
            Mid += new Vector2(p.position.x, p.position.y);
        }
        Mid /= Players.Count * 1.0f;
        float dimX = (Max.x - Min.x) * GameObject.Find("Hub").GetComponent<Hub>().PercentToEdge / camera.aspect;
        float dimY = (Max.y - Min.y) * GameObject.Find("Hub").GetComponent<Hub>().PercentToEdge;
        float hypo = Mathf.Max((dimX * 0.5f) / Mathf.Sin(camera.fieldOfView * 0.5f * Mathf.Deg2Rad),
                               (dimY * 0.5f) / Mathf.Sin(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        float dist = -1f * Mathf.Sqrt(hypo * hypo - Mathf.Pow(Mathf.Max(dimY * 0.5f, dimX * 0.5f), 2));
        // Quick Fix !! Inline if Blub!!
        camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3(Mid.x, Mid.y, dist < MinDistance ? dist : MinDistance), FixLerp);
        FixLerp = Mathf.Lerp(FixLerp, 0.2f, Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyCameraCorrection();
    }
    // Setter and Getter for the Players
    public void SetPlayer(Transform player)
    {
        Players.Add(player);
    }
    public void RemovePlayer(Transform player)
    {
        Players.Remove(player);
        FixLerp = Time.deltaTime;
    }

    public Vector2[] GetConnectingVectors(Transform player)
    {
        return (from p in Players
                where p.transform != player
                select (Vector2)(p.transform.position - player.position)).ToArray<Vector2>();
    }
}

