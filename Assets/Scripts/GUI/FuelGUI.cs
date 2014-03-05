using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class FuelGUI : MonoBehaviour
{


    private Player player;
    [SerializeField]
    private BarSprite
        fuelBar;

    void Start()
    {
        player = GetComponent<Player>();
        fuelBar.Max = 100f;
    }
    
    // Update is called once per frame
    void Update()
    {
        fuelBar.Current = player.Fuel;
    }
}
