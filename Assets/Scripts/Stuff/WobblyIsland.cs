using UnityEngine;
using System.Collections;

public class WobblyIsland : MonoBehaviour
{
    private float rand;
    private float randA;
    private Vector3 Init;
    void Start()
    {
        Init = transform.position;
        rand = (Random.insideUnitCircle.x + 0.5f) * 2.0f ;
        randA = Random.insideUnitCircle.x * 20f;
    }
    // Update is called once per frame
    void Update() {
        this.transform.position = Vector3.Slerp(transform.position, Init + new Vector3(0 , randA * Mathf.Sin((Time.time % 360) * Mathf.Deg2Rad / rand), 0), Time.deltaTime);
	}
}
