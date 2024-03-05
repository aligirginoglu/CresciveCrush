using UnityEngine;

[CreateAssetMenu(fileName = "Tutorials", menuName = "Tutorial")]
public class TutorialSteps : ScriptableObject
{
    [Header("Tutorail Strings")]
    public string SpeachString;

    public string InfoString;

    [Header("Tutorail Bools")]
    public bool SpeachActive;
}