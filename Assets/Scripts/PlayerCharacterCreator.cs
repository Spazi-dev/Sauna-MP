
using System.Collections;
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
							   //public bool Created;

	public CharacterSheet(string playerName, Color32 characterColor0, Color32 characterColor1, int characterItem1)
	{
		this.PlayerName = playerName;
		this.CharacterColor0 = characterColor0;
		this.CharacterColor1 = characterColor1;
		this.CharacterItem1 = characterItem1;
		//this.Created = created;
	}

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

		// Initializing hat list
		cosmeticItemList = new List<string>();
		for (int i = 0; i < GameStateManager.Singleton.CosmeticItemCatalog.CosmeticItems.Length; i++)
		{
			cosmeticItemList.Add(GameStateManager.Singleton.CosmeticItemCatalog.CosmeticItems[i].ItemName);
		}
		cosmeticItemDropdown.AddOptions(cosmeticItemList);

		//Debug.Log($"<color=#8800DD>In start: colorPicker0.color = {colorPicker0.color}, editedCharacterSheet.CharacterColor0 = {editedCharacterSheet.CharacterColor0} </color>");

		if (PlayerPrefs.GetInt("CharacterCreated", 0) == 0) //Character not created previously, set to random
															//if (!GameStateManager.Singleton.localCharacterSheet.Created) //Character not created previously, set to random
		{
			//Debug.Log($"<color=#88DDDD>Character sheet not created, randomizing </color>");
			//Color randomColor = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(.333f, 1f));
			Color randomColor = Colorful.RandomHSV(0f, 1f, 0f, 1f, 0.333f, 1f);
			colorPicker0.color = randomColor;

			SetColor0(randomColor);

			randomColor = Colorful.RandomHSV(0f, 1f, 0f, 1f, 0.333f, 1f);
			colorPicker1.color = randomColor;

			SetColor1(randomColor);

			SetNameTag(RandomName());

			SaveCharacterSheetData();

			//UpdateLocalCharacterSheet();
		}
		else // Character saved to playerprefs or otherwise
		{
			RemakeCharacter();
		}

		//GameStateManager.Singleton.localCharacterSheet.Created = true;
		//PlayerPrefs.SetInt("CharacterCreated", 1);

	}

	public void RemakeCharacter()
	{
		//Debug.Log($"<color=#DDDD88>Character sheet created, assuming an existing one after a delay</color>");

		GameStateManager.Singleton.localCharacterSheet = LoadCharacterSheet();
		editedCharacterSheet = GameStateManager.Singleton.localCharacterSheet;

		colorPicker0.color = editedCharacterSheet.CharacterColor0;
		colorPicker1.color = editedCharacterSheet.CharacterColor1;
		cosmeticItemDropdown.SetValueWithoutNotify(editedCharacterSheet.CharacterItem1); //print($"<color=#DDGG88>hat dropdown set with choice {editedCharacterSheet.CharacterItem1}</color>");

		SetColor0(editedCharacterSheet.CharacterColor0);
		SelectHat(editedCharacterSheet.CharacterItem1); //print($"<color=#DDGG88>hat remade with choice {editedCharacterSheet.CharacterItem1}</color>");
		SetColor1(editedCharacterSheet.CharacterColor1);

		SetNameTag(editedCharacterSheet.PlayerName);

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
			//Debug.Log($"<color=#GG7700>Selected no hat </color>");
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

		UpdateLocalCharacterSheet();
	}

	public void SetColor0(Color col)
	{
		editedCharacterSheet.CharacterColor0 = col;
		mannequinCharacterMesh.material.SetColor("_BaseColor", col);

		UpdateLocalCharacterSheet();
	}
	public void SetColor1(Color col)
	{
		editedCharacterSheet.CharacterColor1 = col;
		if (currentHat != null)
			currentHat.SetItemColor(col);

		UpdateLocalCharacterSheet();
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

		UpdateLocalCharacterSheet();
	}
	public void UpdateLocalCharacterSheet()
	{
		GameStateManager.Singleton.localCharacterSheet = editedCharacterSheet;
	}
	public void SaveCharacterSheetData()
	{

		PlayerPrefs.SetString("PlayerName", editedCharacterSheet.PlayerName);

		//print($"<color=#GG9900>Saving 0R: {editedCharacterSheet.CharacterColor0.r} </color>");
		PlayerPrefs.SetInt("CharacterColor0R", editedCharacterSheet.CharacterColor0.r);
		PlayerPrefs.SetInt("CharacterColor0G", editedCharacterSheet.CharacterColor0.g);
		PlayerPrefs.SetInt("CharacterColor0B", editedCharacterSheet.CharacterColor0.b);

		PlayerPrefs.SetInt("CharacterColor1R", editedCharacterSheet.CharacterColor1.r);
		PlayerPrefs.SetInt("CharacterColor1G", editedCharacterSheet.CharacterColor1.g);
		PlayerPrefs.SetInt("CharacterColor1B", editedCharacterSheet.CharacterColor1.b);

		PlayerPrefs.SetInt("CharacterItem1", editedCharacterSheet.CharacterItem1);

		PlayerPrefs.SetInt("CharacterCreated", 1);
	}
	public CharacterSheet LoadCharacterSheet()
	{

		CharacterSheet loadedCharacterSheet;

		string loadedPlayerName = PlayerPrefs.GetString("PlayerName");

		Color32 loadedCharacterColor0 = new Color32(
			(byte)PlayerPrefs.GetInt("CharacterColor0R"),
			(byte)PlayerPrefs.GetInt("CharacterColor0G"),
			(byte)PlayerPrefs.GetInt("CharacterColor0B"),
			255
			);

		Color loadedCharacterColor1 = new Color32(
			(byte)PlayerPrefs.GetInt("CharacterColor1R"),
			(byte)PlayerPrefs.GetInt("CharacterColor1G"),
			(byte)PlayerPrefs.GetInt("CharacterColor1B"),
			255
			);

		int loadedCharacterItem1 = PlayerPrefs.GetInt("CharacterItem1");

		loadedCharacterSheet = new CharacterSheet(loadedPlayerName, loadedCharacterColor0, loadedCharacterColor1, loadedCharacterItem1);

		return loadedCharacterSheet;
	}

	[ContextMenu("Remove character data from playerprefs")]
	void RemoveCharacterData()
	{
		PlayerPrefs.DeleteKey("PlayerName");
		PlayerPrefs.DeleteKey("CharacterColor0R");
		PlayerPrefs.DeleteKey("CharacterColor0G");
		PlayerPrefs.DeleteKey("CharacterColor0B");
		PlayerPrefs.DeleteKey("CharacterColor1R");
		PlayerPrefs.DeleteKey("CharacterColor1G");
		PlayerPrefs.DeleteKey("CharacterColor1B");
		PlayerPrefs.DeleteKey("CharacterItem1");
		PlayerPrefs.DeleteKey("CharacterCreated");
	}
	[ContextMenu("Clear playerprefs")]
	void RemoveAllData()
	{
		PlayerPrefs.DeleteAll();
	}
}
