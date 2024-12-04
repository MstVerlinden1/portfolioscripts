using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class Relay : NetworkBehaviour
{
    [SerializeField] private GameObject joincodePrefab;
    private async void Start()
    {
        if (GameObject.Find("AuthenticatedGameObject") == null)
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            GameObject AuthenticatedGameObject = new GameObject("AuthenticatedGameObject");
            DontDestroyOnLoad(AuthenticatedGameObject);
        }
    }
    public async void CreateRelay()
    {
        try {
            SceneManager.LoadScene("Game");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(20);
            FixedString32Bytes joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );


            NetworkManager.Singleton.StartHost();

            GameObject obj = Instantiate(joincodePrefab);
            obj.GetComponent<NetworkObject>().Spawn();
            obj.GetComponent<JoinCodeDisplay>().JoinCode.Value = joinCode;
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
    public async void JoinRelay(string joinCode)
    {
        try {
            Debug.Log("Joining " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
}
