using UnityEngine;
using System.Collections;

public class RotateCrossHair : MonoBehaviour {

    void Update()
    {
        transform.Rotate(Vector3.forward * GameObject.Find("Hub").GetComponent<Hub>().RotateSpeed, Space.Self);
    }
}
