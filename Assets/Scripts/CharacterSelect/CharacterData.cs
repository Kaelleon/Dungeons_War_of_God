using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Info")]
    public string characterName;
    public string characterClass;
    public Sprite portrait;   // Icono pequeño para la grilla
    public Sprite fullBody;   // Imagen grande para el centro

    [Header("Stats")]
    public int maxHealth = 100;
    [Range(0f, 100f)] public float defensePercent;
    public float moveSpeedMultiplier = 1f;
    public int attackPoints = 10;
    public float attackSpeedMultiplier = 1f;
    public float evasionMultiplier = 1f;

    [Header("Abilities")]
    public AbilityData[] abilities = new AbilityData[3];

    [Header("In-Game")]
    public Sprite gameSprite;                            // sprite del campeón en la arena (si no usas animaciones)
    public RuntimeAnimatorController animatorController; // opcional: animaciones de este dios
    public GameObject godPrefab;                         // opcional: efectos o habilidades propias del dios
}