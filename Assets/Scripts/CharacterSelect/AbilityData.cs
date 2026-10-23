using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea(2, 5)] public string description;
    public Sprite icon;
}