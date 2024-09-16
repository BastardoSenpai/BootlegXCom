using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "XCOM/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int minDamage;
    public int maxDamage;
    public int criticalChance;
    public int criticalDamageBonus;
    public int ammoCapacity;
    public int range;
    public float accuracyModifier;
    public WeaponType type;
}

public enum WeaponType
{
    Pistol,
    AssaultRifle,
    SniperRifle,
    Shotgun,
    HeavyWeapon
}