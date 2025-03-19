using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

namespace beachstickball
{
    /* manages network synchronization of the player's position
     * requires permission from server to move
     * doing this prevents cheating, etc.
     *
     * why do we need to do this?
     * every player runs on at least two machines,
     * because of this, we need to ensure both machines have correct information about the object on screen
     * only one player controls how the object moves
     */
    public class beachstickballPlayer : NetworkBehaviour
    {
        // network variable for position sync across all clients
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>(); // this represents the player's networked position
        private PlayerController playerController;

        // when player object spawns
        public override void OnNetworkSpawn()
        {
            Position.OnValueChanged += OnStateChanged;
            playerController = GetComponent<PlayerController>();
            // enable script for local player for owners,
            // disable script for non-owners (server controls movement)
            playerController.enabled = IsOwner; 
        }

        public override void OnNetworkDespawn()
        {
            Position.OnValueChanged -= OnStateChanged;
        }

        // update player's position when server syncs it
        public void OnStateChanged(Vector3 previous, Vector3 current)
        {
            if (!IsOwner) // client applies position updates
            {
                transform.position = Position.Value;
            }

        }

        // position update is handled in player controller
    }
}