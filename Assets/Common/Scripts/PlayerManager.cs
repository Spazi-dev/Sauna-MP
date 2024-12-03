using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;
using TMPro;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
	/// <summary>
	/// A generic player manager that manages the lifecycle of a player
	/// </summary>
	/// 
	struct SyncableCustomData : INetworkSerializable
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
		float m_SecondsBetweenDataChanges;
		float m_ElapsedSecondsSinceLastChange;

		/// <summary>
		/// The NetworkVariable holding the custom data to synchronize.
		/// </summary>
		NetworkVariable<SyncableCustomData> m_SyncedCustomData = new NetworkVariable<SyncableCustomData>(writePerm: NetworkVariableWritePermission.Server); //you can adjust who can write to it with parameters

		public override void OnNetworkSpawn()
		{
			/* When an Network Object is spawned, you usally want to setup some if its components
			 * so that they behave differently depending on whether this object is owned by the local player or by other clients. */
			base.OnNetworkSpawn();
			if (IsOwner)
			{
				OnLocalPlayerSpawned();
				return;
			}

			OnNonLocalPlayerSpawned();

			if (IsClient)
			{
				/*
				 * We call the color change method manually when we connect to ensure that our color is correctly initialized.
				 * This is helpful for when a client joins mid-game and needs to catch up with the current state of the game.
				 */
				OnClientCustomDataChanged(m_SyncedCustomData.Value, m_SyncedCustomData.Value);
				m_SyncedCustomData.OnValueChanged += OnClientCustomDataChanged; //this will be called on the client whenever the value is changed by the server
				print($"<color=#DD8800>Client spawned and its data synced </color>");
			}
		}

		public override void OnNetworkDespawn()
		{
			base.OnNetworkDespawn();
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

			playerNameTag.SetActive(false); //The player should not see their own nametag?
		}

		void OnLocalPlayerDeSpawned()
		{
			s_LocalPlayer = null;
		}

		void OnNonLocalPlayerDeSpawned() { }

		void Update()
		{
			if (!IsSpawned)
			{
				//the player disconnected
				return;
			}

			if (!IsServer)
			{
				/*
				 * By default, only the server is allowed to change the value of NetworkVariables.
				 * This can be changed through the NetworkVariable's constructor.
				 */
				//return;
			}

			m_ElapsedSecondsSinceLastChange += Time.deltaTime;

			if (m_ElapsedSecondsSinceLastChange >= m_SecondsBetweenDataChanges)
			{
				m_ElapsedSecondsSinceLastChange = 0;
				//print($"<color=#DD8800>Server is changing data...</color>");
				OnServerChangeData();
			}
		}

		void OnServerChangeData()
		{
			m_SyncedCustomData.Value = new SyncableCustomData
			{
				Playername = MultiplayerUseCasesUtilities.GetRandomUsername(),
				CharacterColor0 = MultiplayerUseCasesUtilities.GetRandomColor()

			};
			print($"<color=#88DD00>Server has created new syncable data</color>");
		}

		void OnClientCustomDataChanged(SyncableCustomData previousValue, SyncableCustomData newValue)
		{
			//print($"<color=#00DD88>Client data Username is being changed from {previousValue.Playername.ToString()}...</color>");
			print($"<color=#00DD88>Client data Username is being changed...</color>");
			OnClientUsernameChanged(newValue.Playername.ToString());
		}
		void OnClientUsernameChanged(string newUsername)
		{
			playerNameText.text = newUsername;
			//print($"<color=#DD0088>Client data Username has been changed to {newUsername}</color>");
			print($"<color=#DD0088>Client data Username has been changed</color>");
		}

	}
}
