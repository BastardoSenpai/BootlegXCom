using UnityEngine;
using System.Collections.Generic;

public enum WeaponType
{
    Pistol,
    AssaultRifle,
    SniperRifle,
    Shotgun,
    HeavyWeapon,
    SMG,
    LMG,
    RocketLauncher
}

[System.Serializable]
public class WeaponAttachment
{
    public string name;
    public int damageBonus;
    public int accuracyBonus;
    public int critChanceBonus;
    public int ammoCapacityBonus;
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "XCOM/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public WeaponType type;
    public int baseDamage;
    public int baseAccuracy;
    public int baseCriticalChance;
    public int baseAmmoCapacity;
    public int range;
    public int actionPointCost = 1;
    public bool isAoE = false;
    public float aoERadius = 0f;

    public List<WeaponAttachment> attachments = new List<WeaponAttachment>();

    public int GetDamage()
    {
        int totalDamage = baseDamage;
        foreach (var attachment in attachments)
        {
            totalDamage += attachment.damageBonus;
        }
        return totalDamage;
    }

    public int GetAccuracy()
    {
        int totalAccuracy = baseAccuracy;
        foreach (var attachment in attachments)
        {
            totalAccuracy += attachment.accuracyBonus;
        }
        return totalAccuracy;
    }

    public int GetCriticalChance()
    {
        int totalCritChance = baseCriticalChance;
        foreach (var attachment in attachments)
        {
            totalCritChance += attachment.critChanceBonus;
        }
        return totalCritChance;
    }

    public int GetAmmoCapacity()
    {
        int totalAmmoCapacity = baseAmmoCapacity;
        foreach (var attachment in attachments)
        {
            totalAmmoCapacity += attachment.ammoCapacityBonus;
        }
        return totalAmmoCapacity;
    }

    public void AddAttachment(WeaponAttachment attachment)
    {
        attachments.Add(attachment);
    }

    public void RemoveAttachment(WeaponAttachment attachment)
    {
        attachments.Remove(attachment);
    }
}