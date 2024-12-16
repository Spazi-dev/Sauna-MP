using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

	public static GameStateManager Singleton { get; private set; }
	[SerializeField] public GlobalRuleSet activeGlobalRules;
	[SerializeField] GameObject StartMenuParent;
	[SerializeField] GameObject SessionWidgetParent;
	[SerializeField] GameObject NetworkedPointLight; // temporarily here because the proximity check test light disrupts the appearance of the character creation screen
	public CosmeticItemCatalog CosmeticItemCatalog;
	public CharacterSheet localCharacterSheet;
	PlayerCharacterCreator playerCharacterCreator;
	void Awake()
	{
		if (Singleton == null)
		{
			Singleton = this;
			DontDestroyOnLoad(gameObject);
		}

	}
	void Start()
	{
		playerCharacterCreator = StartMenuParent.GetComponentInChildren<PlayerCharacterCreator>();

		NetworkManager.Singleton.OnClientStarted += StartEvent;
		NetworkManager.Singleton.OnClientStopped += QuitEvent;

	}

	void StartEvent()
	{
		//print($"<color=#DD0088>Client starteddd </color>");
		playerCharacterCreator.SaveCharacterSheetData();
		StartMenuParent.SetActive(false);
		//SessionWidgetParent.SetActive(true);
		NetworkedPointLight.SetActive(true);
	}
	void QuitEvent(bool host)
	{
		//print($"<color=#FF00AA>Client quitted, hostmode: {host}</color>");
		StartMenuParent.SetActive(true);
		//SessionWidgetParent.SetActive(false);
		NetworkedPointLight.SetActive(false);
		playerCharacterCreator.Invoke("RemakeCharacter", 0);
	}


}
