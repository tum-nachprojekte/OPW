using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hub : MonoBehaviour
{
    // Margins ( Apply on Player and Bullets )
    public int MarginX = 50;                                    // Defines the Size of the field in x-Direction
    public int MarginY = 20;                                    // Defines the Size of the field in y-Direction
    //for the players
    public int startLifepoints = 20;                            // "HP"
    public float grounddetector = 0.5f;                         // Minimal distance to ground to be detected as standing
    public float interpolationCoefficient = 0.5f;               // Interpolation for change of crosshair 0 = Nur neuer Wert, 1 = nur alter Wert | Lineare Interpolation
    public float CrosshairDistance = 1f;                        // distance from the crosshair to its player
    public float crosshairThreshold = 0.4f;                     // threshold before movement of crosshair gets detected
    public Vector2 dampingFactor = new Vector2(0.6f, 1.0f);     // damping of movement every FixedUpdate
    public float movementMultiplier = 6.0f;                     // Multiplier for left/right movement 
    public float PlayerSize = 1.0f;                             // Size Center to Ground ( Dfor the OnGround Method ) TWEAK HERE !!!!
    public float LoadUpLimit = 1.0f;                            // How Long you need to Load up SuperShot
    public float AimAssistLevel = 15;                           // Degrees for your "Aimbot"
    //For Jumps
    public float jumpHight = 8;                                 // Power of Jump
	public float gravityMultiplier = 2.0f;                      // stronger gravity, palyer falls faster, looks better
    //Jetpack
    public float startJetpackFuel = 0.0f;                     // Jetpack Fuell
    public float jetpackDelay = 0.2f;                           // Zeit zwischen Srpung und start des Jetpacks in Sekunden
    public float jetpackMultiplicator = 0.8f;                   // natural velocity of jetpack upstream
    public float fuelRegenRate = 8f;                            // percent per update which the fuel of jetpack regenerates
    public float fuelWastage = 6;                               // percent per update which the fuel of jetpack loses whiel active
    public float WalkSpeed = 15;
    public float WalkAngle = 60;
    //projectiles
    public int ProjSpeed = 12;                                  // speed of projectiles
    public float ProjDelay = 0.125f;                            // 1/frequency of shoot, in seconds
    public float megaShootThreshold = 0.9f;
    public enum PorjectileType { normal, ultra };
	//Camera
    public float MinDistance = 15f;                             // Defines the Distance the Camera will have
    public float PercentToEdge = 1.5f;                          // Defines the Space is between the Frustum Edges and  the Players
    // BarLerp
    public float BarLerpFactor = 5f;                            // Lerp of Fuelbar for each player

    // Rotate Crosshair
    public float RotateSpeed = 2.0f;                            // Defines the Rotationspeed ( Vector3.forward * RotateSpeed )

    // WobblyIsland
    public float WobblyCoefficient = 5.0f;

    // CharacterManager
    public int NumPointsToWin = 5;                              // After how much kills you win

    //points
    public Dictionary<int, int> points;                                // Points for Score
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    private static Hub instance;
    public static Hub Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("Hub").GetComponent<Hub>();
            }
            return instance;
        }
    }
}
