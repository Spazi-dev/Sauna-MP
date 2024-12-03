using TMPro;
using UnityEngine;
using PaziUtils;

public class PlayerCharacterSheet : MonoBehaviour
{
	// Local representation and manager of player and character information

	[Header("Changed from scripts:")]
	[SerializeField] string playerName;
	[SerializeField] int minNameLength = 1;
	[SerializeField] int maxNameLength = 13;
	[SerializeField] Color32 characterColor0;
	[Header("References:")]
	[SerializeField] FlexibleColorPicker colorPicker;
	[SerializeField] SkinnedMeshRenderer playerCharacterMesh; // gotta maybe get these references dynamically for sending to actual player character or only ever use them for mannequin
	[SerializeField] TMP_Text playerNameTag; // gotta maybe get these references dynamically for sending to actual player character or only ever use them for mannequin
	[SerializeField] TMP_InputField playerNameField; // this should be replaced by name field script probably
													 //Material characterMaterial;
	[Header("Editable variables:")]
	[SerializeField] string[] defaultNames;
	void Start()
	{
		if (PlayerPrefs.GetInt("CharacterCreated", 0) == 0) //Character not created previously, set to random
		{
			//Color randomColor = Color.HSVToRGB(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(.333f, 1f));
			Color randomColor = Colorful.RandomHSV(0f, 1f, 0f, 1f, 0.333f, 1f);
			characterColor0 = randomColor;
			SetColor(characterColor0);
			colorPicker.color = characterColor0;

			SetNameTag(RandomName());
		}
		else
		{
			//colorPicker.color = characterColor0;
			//characterMaterial = playerCharacterMesh.material;
		}
	}

	string RandomName()
	{
		playerName = defaultNames[Random.Range(0, defaultNames.Length)];
		return playerName;
	}

	public void SetColor(Color col)
	{
		characterColor0 = col;
		//characterMaterial.SetColor("_BaseColor", characterColor0);
		playerCharacterMesh.material.SetColor("_BaseColor", characterColor0);
	}

	public void SetNameTag(string name)
	{

		if (name.Length > maxNameLength)
		{
			name = name[..maxNameLength]; //range operator
		}

		if (name.Length <= 0)
		{
			playerName = RandomName();
			playerNameTag.text = playerName;
		}
		else
		{
			playerName = name;
			playerNameTag.text = playerName;
			playerNameField.text = playerName;
		}
	}
}
