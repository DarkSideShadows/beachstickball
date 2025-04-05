using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;
using UnityEngine;

/*
    Handles instruction and waiting menu before moving on to gameplay
    - show startScreen for host only
    - client and host press enter to mark themselves as ready
    - when both players are ready, host spawns ball
 */
public class GameManager : NetworkBehaviour
{
    public GameObject startScreen;
    public GameObject waitingScreen;
    public BallController ballController;

    private NetworkVariable<int> readyCount = new NetworkVariable<int>(0); // synced between host/client
    private bool localReady = false;
    private bool gameStarted = false;

    public TMPro.TextMeshProUGUI countdownText;

    void Start()
    {
        startScreen.SetActive(true);    // contains playing instructions for the player
        waitingScreen.SetActive(false); // indicates waiting for other player to join
        countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!localReady && Input.GetKeyDown(KeyCode.Return))
        {
            localReady = true;
            startScreen.SetActive(false); // startScreen -> waiting screen
            waitingScreen.SetActive(true);
            SubmitReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SubmitReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        readyCount.Value++;
        if (readyCount.Value == 2 && !gameStarted)
        {
            HideWaitingScreenClientRpc();   // go to game screen
            StartCountdownClientRpc();      // countdown on all clients
        }
    }

    [ClientRpc]
    void HideWaitingScreenClientRpc()
    {
        waitingScreen.SetActive(false);
    }

    [ClientRpc]
    void StartCountdownClientRpc()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);

        // start game after countdown (only host triggers ball start - this is because host is both SERVER and CLIENT)
        if (IsServer)
            ballController.ActivateBallServerRpc();
    }
}