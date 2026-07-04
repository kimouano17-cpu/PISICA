using Unity.Netcode;
using UnityEngine;

public class SlashTrail : NetworkBehaviour  // <-- palitan NetworkBehaviour
{
    public TrailRenderer trail;
    public TrailRenderer trail2;
    
    public GameObject slashBurstLeft;
    public GameObject slashBurstRight;
    public GameObject slashBurstCross;
    
    public Transform spawnPoint;
    public Transform spawnPoint2;
    public Transform spawnPoint3;

    // ================================
    // TRAIL
    // ================================
    public void TrailOn()
    {
        trail.emitting = true;
        if (IsOwner) TrailOnServerRpc();
    }

    public void TrailOff()
    {
        trail.emitting = false;
        if (IsOwner) TrailOffServerRpc();
    }

    [ServerRpc]
    void TrailOnServerRpc()  { TrailOnClientRpc(); }

    [ServerRpc]
    void TrailOffServerRpc() { TrailOffClientRpc(); }

    [ClientRpc]
    void TrailOnClientRpc()
    {
        if (IsOwner) return;  // sarili na natin, nilaro na
        trail.emitting = true;
    }

    [ClientRpc]
    void TrailOffClientRpc()
    {
        if (IsOwner) return;
        trail.emitting = false;
    }

    // ================================
    // VFX
    // ================================
    public void PlaySlashL()
    {
        SpawnVFX(slashBurstLeft, spawnPoint.position, spawnPoint.rotation);
        if (IsOwner) PlayVFXServerRpc(0, spawnPoint.position, spawnPoint.rotation);
    }

    public void PlaySlashR()
    {
        SpawnVFX(slashBurstRight, spawnPoint2.position, spawnPoint2.rotation);
        if (IsOwner) PlayVFXServerRpc(1, spawnPoint2.position, spawnPoint2.rotation);
    }

    public void PlayCross()
    {
        SpawnVFX(slashBurstCross, spawnPoint3.position, spawnPoint3.rotation);
        if (IsOwner) PlayVFXServerRpc(2, spawnPoint3.position, spawnPoint3.rotation);
    }

    [ServerRpc]
    void PlayVFXServerRpc(int vfxIndex, Vector3 pos, Quaternion rot)
    {
        PlayVFXClientRpc(vfxIndex, pos, rot);
    }

    [ClientRpc]
    void PlayVFXClientRpc(int vfxIndex, Vector3 pos, Quaternion rot)
    {
        if (IsOwner) return;  // hindi na kailangang i-play ulit sa sarili

        GameObject prefab = vfxIndex switch
        {
            0 => slashBurstLeft,
            1 => slashBurstRight,
            2 => slashBurstCross,
            _ => null
        };

        if (prefab != null) SpawnVFX(prefab, pos, rot);
    }

    void SpawnVFX(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        GameObject vfx = Instantiate(prefab, pos, rot);
        Destroy(vfx, 1f);
    }
}