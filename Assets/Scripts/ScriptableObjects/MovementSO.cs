using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu()]
public class MovementSO : ScriptableObject
{
    public string moveName;
    public MovementStateMachine.State moveState;
    public float moveTimeLength;
    public float movePower;
    public float healthScaling;
    public float defenseScaling;
    public float strengthScaling;
    public float agilityScaling;
    public float dexterityScaling;
    public float luckScaling;
    public bool hasDecay;
    public MovementStateMachine.State decayState;
    public float decayTimeLength;
}