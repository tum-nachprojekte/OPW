using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TransferData :MonoBehaviour{

    public Dictionary<int, int> points;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
