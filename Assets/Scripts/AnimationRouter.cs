using Unity.Netcode;
using UnityEngine;

public class AnimationRouter : MonoBehaviour
{
    [SerializeField] private EnemyAttackHitboxScript leftHand;
    [SerializeField] private EnemyAttackHitboxScript rightHand;

    private NetworkBehaviour parentNetworkObject;

    private void Awake()
    {
        // Grab the NetworkBehaviour from the root entity (Player or Enemy AI)
        parentNetworkObject = GetComponentInParent<NetworkBehaviour>();
    }

    public void TriggerLeftHandDamage(float duration)
    {
        // CRITICAL FIX: Only allow the network owner of this entity to start physics windows
        if (parentNetworkObject != null && !parentNetworkObject.IsOwner) return;

        if (leftHand != null)
            leftHand.StartDamageWindow(duration);
    }

    public void TriggerRightHandDamage(float duration)
    {
        // CRITICAL FIX: Only allow the network owner of this entity to start physics windows
        if (parentNetworkObject != null && !parentNetworkObject.IsOwner) return;

        if (rightHand != null)
            rightHand.StartDamageWindow(duration);
    }

    public void clearLeftIgnoreList()
    {
        if (leftHand != null)
            leftHand.ResetIgnoredList();
    }

    public void clearRightIgnoreList()
    {
        if (rightHand != null)
            rightHand.ResetIgnoredList();
    }
}