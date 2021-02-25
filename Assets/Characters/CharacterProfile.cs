using BobJeltes.Attributes;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character Profile", menuName = "Ranchy Rats/Character Profile")]
public class CharacterProfile : ScriptableObject
{
    [Tooltip(
        "Horizontal axis = energy\n" +
        "Vertical axis = level")]
    public AnimationCurve LevelEnergyCurve;
    public float Health = 2;
    public float Speed = 1;
    public float AttackFrequency = 1;
    public float Size = 1;
    [Space]
    public float testEnergy = 0;
    [ReadOnly]
    [SerializeField]
    private int testLevel;

    private void OnValidate()
    {
        TestLevel = Mathf.RoundToInt(LevelEnergyCurve.Evaluate(testEnergy));
    }

    public int TestLevel
    {
        get
        {
            return Mathf.RoundToInt(LevelEnergyCurve.Evaluate(testEnergy));
        }
        set
        {
            testLevel = value;
        }
    } 

    //public float GetHealth(float energy)
    //{
    //    LevelCurve.Evaluate(energy);
    //}
}
