using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    //Particle Prefab here
    public GameObject explosion;
    public GameObject owner;
    public Hub.PorjectileType Type;
    public Sprite[] Sprites;
    public bool released = false;
    private float ind = 0;
    float angle;
    void Start()
    {
        // transform.LookAt(rigidbody2D.velocity, Vector2.up);
        angle = Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x) * Mathf.Rad2Deg;
        if (Type == Hub.PorjectileType.normal)
            angle += 180;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }
    // Remove Projectile if out of bounds
    void Update()
    {
        int MarginX = GameObject.Find("Hub").GetComponent<Hub>().MarginX;
        int MarginY = GameObject.Find("Hub").GetComponent<Hub>().MarginY;
        if (transform.position.x <= -MarginX || transform.position.y <= -MarginY || transform.position.x >= MarginX || transform.position.y >= 80f)
            Destroy(this.gameObject);
        SpriteRenderer rend = this.GetComponent<SpriteRenderer>();
        if (Type == Hub.PorjectileType.normal)
            ind = (ind + 1f) % Sprites.Length;
        else
        {
            if (released)
            {
                if (ind < Sprites.Length - 1) ind += 0.5f;
            }
        }
        rend.sprite = Sprites[(int)ind];

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null && player.gameObject != owner)
        {
            GameObject.Instantiate(explosion, transform.position, Quaternion.Euler(0.0f, 0.0f, 90.0f+angle));
            Destroy(this.gameObject);
        }
        if (player == null)
        {
            GameObject.Instantiate(explosion, transform.position, Quaternion.Euler(0.0f, 0.0f, 90.0f + angle));
            Destroy(this.gameObject);
        }
        // Spawn fancy Stuff here
    }
    public void Init(GameObject owner, Hub.PorjectileType Type)
    {
        this.owner = owner;
        this.Type = Type;
    }
    public void InitAngle()
    {
        angle = Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x) * Mathf.Rad2Deg;
        if (Type == Hub.PorjectileType.normal)
            angle += 180;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
