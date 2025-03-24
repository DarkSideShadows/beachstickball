using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject hostPrefab;  // flamingo prefab for host
    public GameObject clientPrefab; // frog prefab for client

    Vector3 clientSpawnPosition = new Vector3(0.2f, 0.3f, -7);
    Vector3 hostSpawnPosition = new Vector3(-0.2f, 0.3f, 5.5f);

    private void Start()
    {
        // spawn flamingo as host on server start
        NetworkManager.Singleton.OnServerStarted += SpawnHost;

        // spawn player as client joins
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnClient;
    }

    private void OnDestroy()
    {
        // cleanup to prevent errors when stopping/restarting
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= SpawnHost; // remove host/flamingo
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnClient; // remove client/frog
        }
    }

    // function to spawn flamingo as host player
    private void SpawnHost()
    {
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.IsClient)
        {
            GameObject hostPlayer = Instantiate(hostPrefab, hostSpawnPosition, Quaternion.identity);
            hostPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
            Debug.Log($"Spawning host player for client ID: {NetworkManager.Singleton.LocalClientId}");
        }
    }

    // function to spawn frog as client player
    private void SpawnClient(ulong clientId)
    {
        // ensure server only spawns prefabs
        if (!NetworkManager.Singleton.IsServer) return;

        // if the joining client is the host, don't spawn a client prefab (host is also a client)
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost) return;

        Debug.Log($"Attempting to spawn client prefab for clientId: {clientId}");
        GameObject clientPlayer = Instantiate(clientPrefab, clientSpawnPosition, Quaternion.identity);
        NetworkObject netObj = clientPlayer.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId); // spawn client-owned player
        Debug.Log($"Spawned client prefab. Owner is: {netObj.OwnerClientId}");
    }
}