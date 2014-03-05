using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    private Hub instance;
    public GameObject CrosshairPrefab;
    public GameObject JetstreamPrefab;
    public GameObject Projectile;
    public GameObject ProjectileMega;
    public GameObject WeaponHand;
    public GameObject CharacterModel;
    public GameObject LegLeft;
    public GameObject LegRight;
    // public GameObject map;
    public Renderer[] rendererObject;
    public GameObject fallingOutOfArena;
    public GameObject explosionByShot;
    bool jumping = false;
    private GameObject Crosshair;
    private ParticleSystem Jetstream;
    private int m_lifes;
    private float m_fuel;
    private float jetpackDelay; //Zeit, die seit des drücken des Jump-Knopfes 
    private float m_fuelRegenRate;
    private float m_fuelWastageRate;
    public int Lifes { get { return m_lifes; } }
    public float Fuel { get { return m_fuel; } }
    private bool OnGround = false;
    private Vector2 left = new Vector2(-0.25f, -1f);
    private Vector2 right = new Vector2(0.25f, -1f);
    private float deltaTimeshoot = 0.0f;
    private float LoadUp = 0.0f;
    private CameraScript Camera;
    private GameObject projMega;
    private bool released = true;

    // Use this for initialization
    void Start()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        instance = GameObject.Find("Hub").GetComponent<Hub>();
        Crosshair = (GameObject)GameObject.Instantiate(CrosshairPrefab, this.transform.position + new Vector3(instance.CrosshairDistance, 0.0f, 0.0f), Quaternion.identity);
        Crosshair.transform.parent = transform;

        (Crosshair.renderer as SpriteRenderer).color = rendererObject[0].material.color;

        Jetstream = JetstreamPrefab.GetComponent<ParticleSystem>();
        Jetstream.transform.parent = transform;
        Jetstream.Pause();

        rigidbody2D.gravityScale = instance.gravityMultiplier;
        m_fuel = instance.startJetpackFuel;
        m_fuelRegenRate = instance.fuelRegenRate;
        m_fuelWastageRate = instance.fuelWastage;
        m_lifes = instance.startLifepoints;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HitGround();
        if (OnGround)
        {
            rigidbody2D.velocity.Scale(instance.dampingFactor);
            jumping = false;
            if (m_fuel < 100.0f)
                m_fuel += m_fuelRegenRate;
        }
        if (transform.position.y <= -instance.MarginY || transform.position.y >= instance.MarginY || transform.position.x <= -instance.MarginX || transform.position.x >= instance.MarginX)
        {
            KillMePlease(null);
        }
  
        if(OnGround && Mathf.Abs(rigidbody2D.velocity.x) > 0.1f) {
            LegLeft.transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * Hub.Instance.WalkSpeed) * Hub.Instance.WalkAngle,0,0);
            LegRight.transform.localRotation = Quaternion.Euler(-Mathf.Sin(Time.time * Hub.Instance.WalkSpeed)  * Hub.Instance.WalkAngle,0,0);
        }
        else {
            LegLeft.transform.localRotation = Quaternion.identity;
            LegRight.transform.localRotation = Quaternion.identity;
        }
    }
    
    void KillMePlease(GameObject other)
    {
        m_lifes = 0;
        m_fuel = 0f;
        if (projMega != null && !projMega.GetComponent<Projectile>().released)
            Destroy(projMega.gameObject);
        if (other == null)
        {
            if (transform.position.y > 5)
            {
                GameObject.Instantiate(fallingOutOfArena, transform.position, Quaternion.Euler(0.0f, 0.0f, 180.0f));
            }
            else
            {
                GameObject.Instantiate(fallingOutOfArena, transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
            }
        }
        else
        {
            GameObject.Instantiate(explosionByShot, transform.position, other.transform.rotation);
        }
        GameObject.Find("_CharacterManager").SendMessage("KillPlayer", gameObject);
        GameObject.Find("_CharacterManager").GetComponent<CharacterManager>().AddPoints(other);
    }

    void AddVelocity(Vector2 velocity)
    {
        rigidbody2D.velocity += velocity;
    }

    void SetXVelocity(float newXVel)
    {
        rigidbody2D.velocity = new Vector2(newXVel, rigidbody2D.velocity.y);
    }

    public void moveLeft(float amount)
    {
        SetXVelocity(-amount * instance.movementMultiplier);
    }

    public void moveRight(float amount)
    {
        SetXVelocity(amount * instance.movementMultiplier);
    }

    public void jump()
    {
        if (!jumping && OnGround)
        {
            AddVelocity(new Vector2(0, instance.jumpHight));
            jumping = true;
            jetpackDelay = Time.time;
        }
    }

    public void jetpack(float injectionPressure)
    {
        if (Time.time - jetpackDelay > instance.jetpackDelay)
        {
            if (m_fuel > m_fuelWastageRate)
            {
                m_fuel -= m_fuelWastageRate;
                AddVelocity(new Vector2(0, injectionPressure * instance.jetpackMultiplicator));
                Jetstream.Play();
            }
            else
                Jetstream.Stop();
        }
    }

    public void jetpackStop()
    {
        Jetstream.Stop();
    }


    public void aim(Vector2 directionChange)
    {
        // AimAssist vom letzten Frame aber egal
        Vector3 dir = AimAssistance();
        if (directionChange.magnitude < instance.crosshairThreshold)
            return;
        directionChange.Normalize();
        directionChange *= instance.CrosshairDistance;
        Vector3 temp = directionChange;
        temp = (temp + Crosshair.transform.position * instance.interpolationCoefficient
            - transform.position * (1 - instance.interpolationCoefficient) + dir);
        temp.Normalize();
        temp *= instance.CrosshairDistance;
        temp += transform.position;
        Crosshair.transform.position = temp;

        WeaponHand.transform.rotation = Quaternion.Euler(0, 0, (180 - Mathf.Atan2(directionChange.x, directionChange.y) * Mathf.Rad2Deg));

        if (Crosshair.transform.position.x - transform.position.x > 0)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            CharacterModel.transform.localRotation = Quaternion.Euler(270, 135, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            CharacterModel.transform.localRotation = Quaternion.Euler(270, 45, 0);
        }

        aimCannon();
    }
    void aimCannon() { }

    public void shoot()
    {
        if (deltaTimeshoot >= instance.ProjDelay)
        {
            Vector3 startPos = transform.position + 0.42f * (Crosshair.transform.position - transform.position);
            GameObject proj = (GameObject)Instantiate(Projectile, startPos, Quaternion.identity);
            proj.GetComponent<Projectile>().Init(this.gameObject, Hub.PorjectileType.normal);
            ((GameObject)proj).transform.rigidbody2D.velocity = Vector3.Normalize(Crosshair.transform.position - transform.position) * instance.ProjSpeed;
            deltaTimeshoot = 0.0f;
        }
        else
        {
            deltaTimeshoot += Time.deltaTime;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Projectile proj = other.gameObject.GetComponent<Projectile>();
        if (proj != null && proj.owner != this.gameObject)
        {
            if (m_lifes > 0)
            {
                if (proj.Type == Hub.PorjectileType.normal)
                    m_lifes--;
                else
                    KillMePlease(proj.owner);
            }
            else
                KillMePlease(proj.owner);
            Vector2 setback = transform.position - other.gameObject.transform.position;
            setback.Normalize();
            setback *= 10;
            AddVelocity(setback);
        }
    }

    void HitGround()
    {
        RaycastHit2D HitLeft = Physics2D.Raycast(transform.position, left, 2 * instance.MarginY, 1);
        RaycastHit2D HitRight = Physics2D.Raycast(transform.position, right, 2 * instance.MarginY, 1);
        float distance = Mathf.Min(Mathf.Abs(HitRight.point.y - transform.position.y), Mathf.Abs(HitLeft.point.y - transform.position.y));
        OnGround = distance <= instance.PlayerSize ? true : false;
    }

    public void shootMega()
    {
        LoadUp += Time.deltaTime;
        if ((LoadUp += Time.deltaTime) >= instance.LoadUpLimit && released)
        {
            Vector3 startPos = transform.position  + 0.42f * (Crosshair.transform.position - transform.position);
            projMega = (GameObject)Instantiate(ProjectileMega, startPos, Quaternion.identity);
            projMega.GetComponent<Projectile>().Init(this.gameObject, Hub.PorjectileType.ultra);
            projMega.GetComponent<Projectile>().collider2D.enabled = false;
            released = false;
        }
        if (projMega == null) return;
        if (!projMega.GetComponent<Projectile>().released)
            projMega.GetComponent<Projectile>().transform.position = transform.position + 0.42f * (Crosshair.transform.position - transform.position);

    }
    public void releaseShoot()
    {
        released = true;
        if ((LoadUp += Time.deltaTime) >= instance.LoadUpLimit && released)
        {
            projMega.GetComponent<Projectile>().collider2D.enabled = true;
            projMega.GetComponent<Projectile>().released = true;
            projMega.GetComponent<Projectile>().Init(this.gameObject, Hub.PorjectileType.ultra);
            ((GameObject)projMega).transform.rigidbody2D.velocity = Vector3.Normalize(Crosshair.transform.position - transform.position) * instance.ProjSpeed;
            projMega.GetComponent<Projectile>().InitAngle();
        }
        LoadUp = 0.0f;
    }

    public void SetMaterial(Material material)
    {
        foreach (var r in rendererObject)
            r.material = material;
    }

    private Vector2 AimAssistance()
    {
        Vector2[] vectors = Camera.GetConnectingVectors(this.transform);
        Vector2 aimDir = Crosshair.transform.position - transform.position;
        Vector2 res = new Vector2(0, 0);
        float minAngle = 90;
        foreach (Vector2 v in vectors)
        {
            float angle = Vector2.Angle(aimDir, v);
            if (Mathf.Min(angle, minAngle) <= minAngle && angle <= instance.AimAssistLevel)
            {
                minAngle = angle;
                res = v;
            }
        }
        res.Normalize();
        return res;
    }

}
