    using Unity.Netcode;
    using UnityEngine;

    public class BrokenGuardianVFX : NetworkBehaviour  
    {
        public TrailRenderer trail;
        public TrailRenderer trail2;
        


        public GameObject jumpBurstVFX;
        public GameObject smashBurstVFX;
        
        public Transform jumpSpawnPoint;
        public Transform smashSpawnPoint;


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
            if (IsOwner) return;  
            trail.emitting = true;
        }

        [ClientRpc]
        void TrailOffClientRpc()
        {
            if (IsOwner) return;
            trail.emitting = false;
        }
        // =========================
        // Smash VFX
        // =========================

        public void SmashVFX()
        {
            if (!IsOwner) return;

            PlaySmashServerRpc();
        }
                [ServerRpc]
        void PlaySmashServerRpc()
        {
            PlaySmashClientRpc();
        }
                [ClientRpc]
        void PlaySmashClientRpc()
        {
            GameObject vfx = Instantiate(
                smashBurstVFX,
                smashSpawnPoint.position,
                smashSpawnPoint.rotation
            );

            vfx.transform.SetParent(smashSpawnPoint);

            Destroy(vfx, 1f);
        }
        // =========================
        // Jump VFX
        // =========================

        public void JumpVFX()
        {
            if (!IsOwner) return;

            PlayJumpServerRpc();
        }
                [ServerRpc]
        void PlayJumpServerRpc()
        {
            PlayJumpClientRpc();
        }
                [ClientRpc]
        void PlayJumpClientRpc()
        {
            GameObject vfx = Instantiate(
                jumpBurstVFX,
                jumpSpawnPoint.position,
                jumpSpawnPoint.rotation
            );

            vfx.transform.SetParent(jumpSpawnPoint);

            Destroy(vfx, 1f);
        }        
    }