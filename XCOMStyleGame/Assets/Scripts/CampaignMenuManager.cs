using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CampaignMenuManager : MonoBehaviour
{
    public CampaignManager campaignManager;
    public GameObject soldierListItemPrefab;
    public Transform soldierListContent;
    public GameObject soldierDetailsPanel;
    public GameObject customizationPanel;
    public GameObject inventoryPanel;

    [Header("Soldier Details")]
    public TMP_Text soldierNameText;
    public TMP_Text soldierClassText;
    public TMP_Text soldierLevelText;
    public TMP_Text soldierStatsText;

    [Header("Customization")]
    public Image armorColorImage;
    public TMP_Dropdown faceDropdown;
    public TMP_Dropdown hairDropdown;
    public Image hairColorImage;

    [Header("Inventory")]
    public Transform inventoryContent;
    public GameObject inventoryItemPrefab;

    private PersistentSoldier selectedSoldier;

    void Start()
    {
        campaignManager = CampaignManager.Instance;
        LoadSoldierList();
    }

    void LoadSoldierList()
    {
        foreach (Transform child in soldierListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (PersistentSoldier soldier in campaignManager.currentCampaign.soldiers)
        {
            GameObject listItem = Instantiate(soldierListItemPrefab, soldierListContent);
            listItem.GetComponentInChildren<TMP_Text>().text = soldier.name;
            listItem.GetComponent<Button>().onClick.AddListener(() => SelectSoldier(soldier));
        }
    }

    void SelectSoldier(PersistentSoldier soldier)
    {
        selectedSoldier = soldier;
        UpdateSoldierDetails();
        soldierDetailsPanel.SetActive(true);
    }

    void UpdateSoldierDetails()
    {
        soldierNameText.text = selectedSoldier.name;
        soldierClassText.text = selectedSoldier.classType.ToString();
        soldierLevelText.text = "Level: " + selectedSoldier.level;
        soldierStatsText.text = $"Health: {selectedSoldier.stats["maxHealth"]}\n" +
                                $"Accuracy: {selectedSoldier.stats["accuracy"]}\n" +
                                $"Mobility: {selectedSoldier.stats["mobility"]}";

        UpdateCustomizationPanel();
        UpdateInventoryPanel();
    }

    void UpdateCustomizationPanel()
    {
        armorColorImage.color = selectedSoldier.customization.armorColor;
        // Update face and hair dropdowns based on available options
        hairColorImage.color = selectedSoldier.customization.hairColor;
    }

    public void OnArmorColorChanged(Color newColor)
    {
        selectedSoldier.customization.armorColor = newColor;
        armorColorImage.color = newColor;
    }

    public void OnFaceChanged(int index)
    {
        selectedSoldier.customization.faceIndex = index;
    }

    public void OnHairChanged(int index)
    {
        selectedSoldier.customization.hairIndex = index;
    }

    public void OnHairColorChanged(Color newColor)
    {
        selectedSoldier.customization.hairColor = newColor;
        hairColorImage.color = newColor;
    }

    void UpdateInventoryPanel()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        foreach (string weaponName in selectedSoldier.inventory)
        {
            GameObject inventoryItem = Instantiate(inventoryItemPrefab, inventoryContent);
            inventoryItem.GetComponentInChildren<TMP_Text>().text = weaponName;
            inventoryItem.GetComponent<Button>().onClick.AddListener(() => EquipWeapon(weaponName));
        }
    }

    void EquipWeapon(string weaponName)
    {
        // In a real implementation, you'd want to have a way to reference the actual Weapon scriptable object
        // For now, we'll just update the first item in the inventory as the equipped weapon
        if (selectedSoldier.inventory.Count > 0)
        {
            selectedSoldier.inventory.Remove(weaponName);
            selectedSoldier.inventory.Insert(0, weaponName);
        }
        UpdateInventoryPanel();
    }

    public void OnStartMissionClicked()
    {
        campaignManager.SaveCampaign();
        // Load the mission scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MissionScene");
    }
}