using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoldierCustomization
{
    public Color skinColor = Color.white;
    public Color hairColor = Color.black;
    public Color eyeColor = Color.blue;
    public Color armorPrimaryColor = Color.gray;
    public Color armorSecondaryColor = Color.white;

    public int hairStyleIndex = 0;
    public int faceIndex = 0;
    public int armorPatternIndex = 0;

    public List<string> accessories = new List<string>();

    public static SoldierCustomization GenerateRandomCustomization()
    {
        SoldierCustomization customization = new SoldierCustomization();

        customization.skinColor = Random.ColorHSV(0f, 1f, 0.4f, 0.8f, 0.7f, 1f);
        customization.hairColor = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.3f, 1f);
        customization.eyeColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        customization.armorPrimaryColor = Random.ColorHSV(0f, 1f, 0.2f, 0.8f, 0.2f, 0.8f);
        customization.armorSecondaryColor = Random.ColorHSV(0f, 1f, 0.2f, 0.8f, 0.2f, 0.8f);

        customization.hairStyleIndex = Random.Range(0, 10); // Assuming 10 hair styles
        customization.faceIndex = Random.Range(0, 10); // Assuming 10 face options
        customization.armorPatternIndex = Random.Range(0, 5); // Assuming 5 armor patterns

        // Randomly add accessories
        string[] possibleAccessories = { "Helmet", "Shoulder Pads", "Knee Pads", "Backpack", "Goggles" };
        int accessoryCount = Random.Range(0, 3);
        for (int i = 0; i < accessoryCount; i++)
        {
            string accessory = possibleAccessories[Random.Range(0, possibleAccessories.Length)];
            if (!customization.accessories.Contains(accessory))
            {
                customization.accessories.Add(accessory);
            }
        }

        return customization;
    }
}