using TMPro;
using UnityEngine;
using PaziUtils;
using System.Collections.Generic;

public struct CharacterSheet
{
	public string PlayerName;
	public Color32 CharacterColor0; // color of the body
	public Color32 CharacterColor1; // color of the first cosmetic item (hat)
	public int CharacterItem1; // in character sheet 0 is no hat, but in ItemCatalog it's the first hat of the catalog

}
public class PlayerCharacterCreator : MonoBehaviour
{
	// Local representation and manager of player and character information

	//[Header("Changed from scripts:")]
	//[SerializeField] string playerName;
	//[SerializeField] Color32 characterColor0;
	[Header("References:")]
	[SerializeField] FlexibleColorPicker colorPicker0;
	[SerializeField] FlexibleColorPicker colorPicker1;
	[SerializeField] TMP_Dropdown cosmeticItemDropdown;
	[SerializeField] Transform mannequinRigRoot;
	[SerializeField] SkinnedMeshRenderer mannequinCharacterMesh; // gotta maybe get these references dynamically for sending to actual player character or only ever use them for mannequin
	[SerializeField] TMP_Text mannequinNameTag; // gotta maybe get these references dynamically for sending to actual player character or only ever use them for mannequin
	[SerializeField] TMP_InputField playerNameField; // this should be replaced by name field script probably

	[Header("Editable variables:")]

	[SerializeField] string[] defaultNames;
	CharacterSheet editedCharacterSheet;
	CosmeticItem currentHat;
	List<string> cosmeticItemList;
	void Start()
	{

		if (PlayerPrefs.GetInt("CharacterCreated", 0) == 0) //Character not created previously, set to random
		{
			//Color randomColor = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(.333f, 1f));
			Color randomColor = Colorful.RandomHSV(0f, 1f, 0f, 1f, 0.333f, 1f);
			colorPicker0.color = randomColor;

			SetColor0(randomColor);

			randomColor = Colorful.RandomHSV(0f, 1f, 0f, 1f, 0.333f, 1f);
			colorPicker1.color = randomColor;

			SetColor1(randomColor);

			SetNameTag(RandomName());

			//SaveCharacterSheet(); 
		}
		else // Character supposedly saved to playerprefs
		{

			//colorPicker.color = characterColor0;
			//characterMaterial = playerCharacterMesh.material;
		}

		// Initializing hat list
		cosmeticItemList = new List<string>();
		for (int i = 0; i < GameStateManager.Singleton.CosmeticItemCatalog.CosmeticItems.Length; i++)
		{
			cosmeticItemList.Add(GameStateManager.Singleton.CosmeticItemCatalog.CosmeticItems[i].ItemName);
		}
		cosmeticItemDropdown.AddOptions(cosmeticItemList);
	}

	string RandomName()
	{
		string randomName = defaultNames[Random.Range(0, defaultNames.Length)];
		return randomName;
	}

	public void SelectHat(int choice)
	{
		if (choice == 0)
		{
			Debug.Log($"<color=#GG7700>Selected no hat </color>");
			if (currentHat != null)
				Destroy(currentHat.gameObject);
		}
		else
		{
			if (currentHat != null)
				Destroy(currentHat.gameObject);

			editedCharacterSheet.CharacterItem1 = choice; // in character sheet 0 is no hat, but in ItemCatalog it's the first hat of the catalog


			currentHat = Instantiate(
				GameStateManager.Singleton.CosmeticItemCatalog.CosmeticItems[choice - 1],
				mannequinRigRoot.FindRecursive(GameStateManager.Singleton.CosmeticItemCatalog.CosmeticItems[choice - 1].CharacterBone)
				);
			currentHat.SetItemColor(editedCharacterSheet.CharacterColor1);

		}

		SaveCharacterSheet();
	}

	public void SetColor0(Color col)
	{
		editedCharacterSheet.CharacterColor0 = col;
		mannequinCharacterMesh.material.SetColor("_BaseColor", col);

		SaveCharacterSheet(); // this could be called with a delegate only when joining and to avoid duplication and writing to prefs a lot 
	}
	public void SetColor1(Color col)
	{
		editedCharacterSheet.CharacterColor1 = col;
		if (currentHat != null)
			currentHat.SetItemColor(col);

		SaveCharacterSheet(); // this could be called with a delegate only when joining and to avoid duplication and writing to prefs a lot 
	}

	public void SetNameTag(string name)
	{
		string randomFallbackName;
		if (name.Length > GameStateManager.Singleton.activeGlobalRules.MaxNameLength)
		{
			name = name[..GameStateManager.Singleton.activeGlobalRules.MaxNameLength]; //range operator
		}

		if (name.Length <= 0)
		{
			randomFallbackName = RandomName();
			editedCharacterSheet.PlayerName = randomFallbackName;
			mannequinNameTag.text = randomFallbackName;
		}
		else
		{
			editedCharacterSheet.PlayerName = name;
			playerNameField.text = name;
			mannequinNameTag.text = name;
		}

		SaveCharacterSheet(); // this could be called with a delegate only when joining and to avoid duplication and writing to prefs a lot 
	}

	void SaveCharacterSheet()
	{
		GameStateManager.Singleton.localCharacterSheet = editedCharacterSheet;
		//Save to playerprefs here too probably
	}
}
