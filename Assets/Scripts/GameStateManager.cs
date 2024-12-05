using System;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

	public static GameStateManager Singleton { get; private set; }
	[SerializeField] public GlobalRuleSet activeGlobalRules;
	[SerializeField] GameObject StartMenuParent;
	string menuSceneName;
	public CharacterSheet localCharacterSheet;
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

		NetworkManager.Singleton.OnClientStarted += StartEvent;
		NetworkManager.Singleton.OnClientStopped += QuitEvent;
	}

	void StartEvent()
	{
		print($"<color=#DD0088>Client starteddd </color>");
		StartMenuParent.SetActive(false);
	}
	void QuitEvent(bool host)
	{
		print($"<color=#FF00AA>Client quitted, hostmode: {host}</color>");
		StartMenuParent.SetActive(true);
	}


}
