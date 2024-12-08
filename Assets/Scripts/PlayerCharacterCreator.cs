using TMPro;
using UnityEngine;
using PaziUtils;

public struct CharacterSheet
{
	public string PlayerName;
	public Color32 CharacterColor0;
}
public class PlayerCharacterCreator : MonoBehaviour
{
	// Local representation and manager of player and character information

	//[Header("Changed from scripts:")]
	//[SerializeField] string playerName;
	//[SerializeField] Color32 characterColor0;
	[Header("References:")]
	[SerializeField] FlexibleColorPicker colorPicker;
	[SerializeField] SkinnedMeshRenderer playerCharacterMesh; // gotta maybe get these references dynamically for sending to actual player character or only ever use them for mannequin
	[SerializeField] TMP_Text playerNameTag; // gotta maybe get these references dynamically for sending to actual player character or only ever use them for mannequin
	[SerializeField] TMP_InputField playerNameField; // this should be replaced by name field script probably
													 //Material characterMaterial;
	[Header("Editable variables:")]
	[SerializeField] string[] defaultNames;
	CharacterSheet editedCharacterSheet;
	void Start()
	{

		if (PlayerPrefs.GetInt("CharacterCreated", 0) == 0) //Character not created previously, set to random
		{
			//Color randomColor = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(.333f, 1f));
			Color randomColor = Colorful.RandomHSV(0f, 1f, 0f, 1f, 0.333f, 1f);
			colorPicker.color = randomColor;

			SetColor(randomColor);
			SetNameTag(RandomName());

			SaveCharacterSheet();
		}
		else // Character supposedly saved to playerprefs
		{

			//colorPicker.color = characterColor0;
			//characterMaterial = playerCharacterMesh.material;
		}
	}

	string RandomName()
	{
		string randomName = defaultNames[Random.Range(0, defaultNames.Length)];
		return randomName;
	}

	public void SetColor(Color col)
	{
		editedCharacterSheet.CharacterColor0 = col;
		playerCharacterMesh.material.SetColor("_BaseColor", col);

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
			playerNameTag.text = randomFallbackName;
		}
		else
		{
			editedCharacterSheet.PlayerName = name;
			playerNameField.text = name;
			playerNameTag.text = name;
		}

		SaveCharacterSheet(); // this could be called with a delegate only when joining and to avoid duplication and writing to prefs a lot 
	}

	void SaveCharacterSheet()
	{
		GameStateManager.Singleton.localCharacterSheet = editedCharacterSheet;
		//Save to playerprefs here too probably
	}
}
