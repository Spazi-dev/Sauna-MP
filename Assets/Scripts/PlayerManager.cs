using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;
using TMPro;
using Unity.Netcode;
//using Unity.Netcode.Samples.MultiplayerUseCases.Common;

/// <summary>
/// A generic player manager that manages the lifecycle of a player
/// </summary>
/// 
struct SyncableCharacterData : INetworkSerializable
{
	public FixedString64Bytes Playername; //value-type version of string with fixed allocation. Strings should be avoided in general when dealing with netcode. Fixed strings are a "less bad" option.
	public Color32 CharacterColor0;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Playername);
		serializer.SerializeValue(ref CharacterColor0);
	}
}
public class PlayerManager : NetworkBehaviour
{
	/// <summary>
	/// The localplayer instance
	/// </summary>
	/// <remarks> You could use <c>NetworkManager.Singleton.LocalClient.PlayerObject</c> if you don't want to maintain this flag,
	/// but keep in mind that you'll also have to check that the NetworkManager is available and that a local client is running</remarks>
	public static PlayerManager s_LocalPlayer;

	[SerializeField]
	PlayerInput inputManager;
	[SerializeField] GameObject playerNameTag;
	[SerializeField] TMP_Text playerNameText;
	[SerializeField] SkinnedMeshRenderer playerCharacterMesh;
	[SerializeField] GameObject playerVCams;

	[SerializeField, Tooltip("The seconds that will elapse between data changes")]

	/// <summary>
	/// The NetworkVariable holding the custom data to synchronize.
	/// </summary>
	NetworkVariable<SyncableCharacterData> m_SyncedCharacterData = new NetworkVariable<SyncableCharacterData>(writePerm: NetworkVariableWritePermission.Owner); //you can adjust who can write to it with parameters

	public override void OnNetworkSpawn()
	{
		/* When an Network Object is spawned, you usally want to setup some if its components
		 * so that they behave differently depending on whether this object is owned by the local player or by other clients. */
		base.OnNetworkSpawn();

		OnCharacterDataChanged(m_SyncedCharacterData.Value, m_SyncedCharacterData.Value);
		m_SyncedCharacterData.OnValueChanged += OnCharacterDataChanged;

		if (!IsServer)
		{
		}

		if (IsOwner)
		{
			//OnCharacterDataChanged(m_SyncedCharacterData.Value, m_SyncedCharacterData.Value);
			//m_SyncedCharacterData.OnValueChanged += OnCharacterDataChanged;
			InitializeSyncedCharacterData();

			//print($"<color=#DD8800>Owner spawned and its data synced </color>");

			OnLocalPlayerSpawned();
			return;
		}

		OnNonLocalPlayerSpawned();

		/* if (IsClient)
		{

			//OnCharacterDataChanged(m_SyncedCharacterData.Value, m_SyncedCharacterData.Value);
			//m_SyncedCharacterData.OnValueChanged += OnCharacterDataChanged;
			print($"<color=#0088DD>Client spawned and its data synced </color>");
		} */

	}

	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
		if (!IsServer)
		{
			m_SyncedCharacterData.OnValueChanged -= OnCharacterDataChanged;
		}

		if (IsOwner)
		{
			OnLocalPlayerDeSpawned();
			return;
		}
		OnNonLocalPlayerDeSpawned();
	}

	void OnNonLocalPlayerSpawned()
	{
		//you don't want other players to be able to control your player
		if (inputManager)
		{
			inputManager.enabled = false;
		}

		playerVCams.SetActive(false);
	}

	void OnLocalPlayerSpawned()
	{
		/* you want only the local player to be identified as such, and to have its input-related components enabled.
		 * The same concept usually applies for cameras, UI, etc...*/
		s_LocalPlayer = this;
		if (inputManager)
		{
			inputManager.enabled = IsOwner;
		}

		//playerNameTag.SetActive(false); //The player should not see their own nametag?
	}

	void OnLocalPlayerDeSpawned()
	{
		s_LocalPlayer = null;
	}

	void OnNonLocalPlayerDeSpawned() { }

	/* void Update()
	{
		//Debug.Log(GameStateManager.Singleton.localCharacterSheet.PlayerName.ToString());

		if (!IsSpawned)
		{
			//the player disconnected
			return;
		}

		if (IsOwner)
		{

		}

	} */

	void InitializeSyncedCharacterData()
	{

		m_SyncedCharacterData.Value = new SyncableCharacterData
		{
			Playername = GameStateManager.Singleton.localCharacterSheet.PlayerName,
			CharacterColor0 = GameStateManager.Singleton.localCharacterSheet.CharacterColor0
		};

		print($"<color=#88DD00>Owner has created new syncable data</color>");
	}

	void OnCharacterDataChanged(SyncableCharacterData previousValue, SyncableCharacterData newValue)
	{
		ChangeColor(newValue.CharacterColor0);
		ChangeUsername(newValue.Playername.ToString());
		print($"<color=#00DD88>Data has been changed</color>");
	}
	void ChangeUsername(string newUsername)
	{
		playerNameText.text = newUsername;
		//print($"<color=#DD0088>Client data Username has been changed to {newUsername}</color>");
		//print($"<color=#DD0088>Data Username has been changed</color>");
	}

	void ChangeColor(Color32 newColor)
	{
		playerCharacterMesh.material.color = newColor;
		//print($"<color=#DD0088>Data CharacterColor0 has been changed</color>");
	}

}
