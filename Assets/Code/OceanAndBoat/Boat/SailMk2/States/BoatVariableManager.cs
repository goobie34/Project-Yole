using UnityEngine;

// --- Purpous:
// The single source of truth for granular boat state changes,
// sutch as rasing and lowering the sail, or enabeing and disabeling movement sources.
// The manager works a bit like a fuzzy state machine (i think),
// However, the variables are more like flags than states,

// --- Function:
// the "Setters" serve as input for a specific variable, while the getters
// take other variables into account, for example the sail force getter takes both the
// master sail force variable into account, as well as how much the sail is raised

// --- Usage:
// Most variables should remain between 0 and 1. There is risk for race conditions,
// if two systems falsley assume they are the only ones modifying a setter,
// A variable may only use it's own Set variable. For variables other than itself, use the get variable

public class BoatVariableManager : MonoBehaviour
{


    [Header("Generic Variables")]
    // master variable for player controll
    public FuzzyFlagSetter Set_ControlBoat;
    public float Get_ControlBoat => Set_ControlBoat.Get()
        * Get_FreeMovement
        * (1 - Get_Stunned)
        * (1 - Get_Docked);



    // if the bloat is free to move, or locked in place
    public FuzzyFlagSetter Set_FreeMovement;
    public float Get_FreeMovement => Set_FreeMovement.Get()
        * (1 - Get_Docked);

    

    // if the boat is pushed by waves
    public FuzzyFlagSetter Set_MovedByWaves;
    public float Get_MovedByWaves => Set_MovedByWaves.Get()
        * Get_FreeMovement;

    
    // if stunned
    public FuzzyFlagSetter Set_Stunned;
    public float Get_Stunned => Set_Stunned.Get();


    public FuzzyFlagSetter Set_Docked;
    public float Get_Docked => Set_Docked.Get();


    [Header("Sail Variables")]
    // if the sail is rigged or closed
    public FuzzyFlagSetter Set_SailRaised;
    public float Get_SailRaised => Set_SailRaised.Get();


    // if the boat is pulled allong by the wind
    public FuzzyFlagSetter Set_SailForce;
    public float Get_SailForce => Set_SailForce.Get()
        * Get_FreeMovement
        * Get_SailRaised
        * (1 - Get_Stunned);


    // if the mainsail rope is free to change, or locked in the center
    public FuzzyFlagSetter Set_FreeMainsailRope;
    public float Get_FreeMainsailRope => Set_FreeMainsailRope.Get()
        * Get_SailRaised;

    
    // if the player can controll the mainsail rope.
    public FuzzyFlagSetter Set_ControlMainsailRope;
    public float Get_ControlMainsailRope => Set_ControlMainsailRope.Get()
        * Get_FreeMainsailRope 
        * Get_SailRaised
        * Get_ControlBoat;



    [Header("Rowing Variables")]
    // if oars are out and in the water
    public FuzzyFlagSetter Set_OarsRaised;
    public float Get_OarsRaised => Set_OarsRaised.Get();


    // if player has controll over oar movement
    public FuzzyFlagSetter Set_ControlOarForce;
    public float Get_ControlOarForce => Set_ControlOarForce.Get()
        * Get_OarsRaised
        * Get_FreeMovement;

}


[System.Serializable]
public struct FuzzyFlagSetter
{
    [SerializeField] private float value;

    public FuzzyFlagSetter(float value) => this.value = value;

    public float Get() => value;

    public void Set(float value)
    {
        this.value = value;
    }


}