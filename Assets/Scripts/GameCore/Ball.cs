using UnityEngine;

[CreateAssetMenu(fileName = "Balls", menuName = "Ball")]
public class Ball : ScriptableObject
{
    public BallType Type;
    public GameObject Prefab;
    public GameObject Model;
    public GameObject ExplotionsEffect;
    public Color ExplotionColor;
}

public enum BallType
{
    Yellow,
    Blue,
    Red,
    White,
    Green,
    Purple,
    Orange,
    Candy
}