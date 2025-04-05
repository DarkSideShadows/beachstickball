using Unity.Netcode;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    public GameObject hostInstructions;
    public GameObject clientInstructions;
    public Transform parentTransform;

    private void Start()
    {
        // only do this on the local player
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // we are only handling the local player (not tother clients)
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        if (NetworkManager.Singleton.IsHost)
        {
            GameObject hostUI = Instantiate(hostInstructions, parentTransform);
            hostUI.SetActive(true);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            GameObject clientUI = Instantiate(clientInstructions, parentTransform);
            clientUI.SetActive(true);
        }
    }
}