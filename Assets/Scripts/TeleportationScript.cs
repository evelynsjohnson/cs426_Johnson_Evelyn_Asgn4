using UnityEngine;
using Unity.Netcode;

public class TeleportationScript : NetworkBehaviour
{
    public Transform teleportDestination;

    private Renderer rend;
    private Collider col;

    private NetworkVariable<bool> disabled = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    public override void OnNetworkSpawn()
    {
        //disabled.OnValueChanged += OnDisabledChanged;
        //OnDisabledChanged(false, disabled.Value);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        //if (disabled.Value) return;
        NetworkObject playerNetObj = other.gameObject.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;
        if (playerNetObj.IsOwner)
        {
            RequestTeleportServerRpc(playerNetObj.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeleportServerRpc(ulong playerNetworkId)
    {
        //if (disabled.Value) return;
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
            playerNetworkId, out NetworkObject playerNetObj))
            return;
        TeleportPlayerClientRpc(
            teleportDestination.position + Vector3.up,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerNetObj.OwnerClientId }
                }
            }
        );

        //disabled.Value = true;
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 newPosition, ClientRpcParams rpcParams = default)
    {
        GameObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;

        localPlayer.transform.position = newPosition;

        Rigidbody rb = localPlayer.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnDisabledChanged(bool oldValue, bool newValue)
    {
        //if (newValue)
        //{
        //    if (col != null)
        //        col.enabled = false;

        //    if (rend != null)
        //        rend.material.color = Color.gray;
        //}
    }
}
