using UnityEngine;
using Unity.Netcode;

public class GuardianSFX : NetworkBehaviour
{
    public AudioSource audioSource;

    [Header("Walk")]
    public AudioClip walkLSFX;
    public AudioClip walkRSFX;

    [Header("Attack")]
    public AudioClip BLattackSFX;
    public AudioClip BRattackSFX;

        [Header("Dash")]
    public AudioClip dashSFX;


    public AudioClip crossAttackSFX;
    public AudioClip trustAttackSFX;

    void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip);
    }

    [ServerRpc]
    void WalkServerRpc(bool left)
    {
        WalkClientRpc(left);
    }
    [ClientRpc]
    void WalkClientRpc(bool left)
    {
        PlayClip(left ? walkLSFX : walkRSFX);
    }
    public void WalkLSFX()
    {
        if (!IsOwner) return;
        WalkServerRpc(true);
    }
    public void WalkRSFX()
    {
        if (!IsOwner) return;
        WalkServerRpc(false);
    }

    ///Attack SFX
    
        [ServerRpc]
    void AttackServerRpc(bool left)
    {
        AttackClientRpc(left);
    }

    [ClientRpc]
    void AttackClientRpc(bool left)
    {
        PlayClip(left ? BLattackSFX : BRattackSFX);
    }

    public void BLAttackSFX()
    {
        if (!IsOwner) return;

        AttackServerRpc(true);
    }

    public void BRAttackSFX()
    {
        if (!IsOwner) return;

        AttackServerRpc(false);
    }
    //Cross Attak SFX
    [ServerRpc]
    void CrossAttackServerRpc()
    {
        CrossAttackClientRpc();
    }
    [ClientRpc]
    void CrossAttackClientRpc()
    {
        PlayClip(crossAttackSFX);
    }

    public void CrossAttackSFX()
    {
        if(!IsOwner) return;
        CrossAttackServerRpc();
    }
        //Trust
    [ServerRpc]
    void TrustAttackServerRpc()
    {
        TrustAttackClientRpc();
    }
    [ClientRpc]
    void TrustAttackClientRpc()
    {
        PlayClip(trustAttackSFX);
    }
    
    public void TrustAttackSFX()
    {
        if(!IsOwner) return;
        TrustAttackServerRpc();
    }

            //Dash
    [ServerRpc]
    void DashServerRpc()
    {
        DashClientRpc();
    }
    [ClientRpc]
    void DashClientRpc()
    {
        PlayClip(dashSFX);
    }
    
    public void DashSFX()
    {
        if(!IsOwner) return;
        DashServerRpc();
    }



}