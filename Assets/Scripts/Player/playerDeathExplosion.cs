using UnityEngine;
using System.Collections;

public class playerDeathExplosion : MonoBehaviour {

    public GameObject owner;
    public Sprite[] Sprites;
    public float timeForLoop = 0.5f;
    private float ind = 0;

    void Start()
    {
        

    }
    // Remove Projectile if out of bounds
    void Update()
    {
        SpriteRenderer rend = this.GetComponent<SpriteRenderer>();
            ind = (ind + (timeForLoop/(float)Sprites.Length));
        if (((int)ind )>= Sprites.Length) {
            Destroy(gameObject);
            return;
        }
        rend.sprite = Sprites[(int)ind];


    }
}
