    using Unity.Netcode;
    using UnityEngine;

    public class EnemyVFX : NetworkBehaviour  
    {
        public TrailRenderer trail;
        public TrailRenderer trail2;
        
        public GameObject slashBurstLeft;
        public GameObject slashBurstRight;
        public GameObject slashBurstCross;
        public GameObject leftSlashVFX;
        public GameObject rightSlashVFX;
        public GameObject frontStabVFX;
        
        public Transform spawnPoint;
        public Transform spawnPoint2;
        public Transform spawnPointCross;
        public Transform spawnPointTL;
        public Transform spawnPointTR;
        public Transform spawnPointStab;

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
            SpawnVFX(slashBurstCross, spawnPointCross.position, spawnPointCross.rotation);
            if (IsOwner) PlayVFXServerRpc(2, spawnPointCross.position, spawnPointCross.rotation);
        }

        [ServerRpc]
        void PlayVFXServerRpc(int vfxIndex, Vector3 pos, Quaternion rot)
        {
            PlayVFXClientRpc(vfxIndex, pos, rot);
        }

        [ClientRpc]
        void PlayVFXClientRpc(int vfxIndex, Vector3 pos, Quaternion rot)
        {
            if (IsOwner) return; 

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

        // =========================
        // LEFT SLASH
        // =========================

        public void PlayLeftSlash()
        {
            if (!IsOwner) return;

            PlaySlashServerRpc(true);
        }

        // =========================
        // RIGHT SLASH
        // =========================

        public void PlayRightSlash()
        {
            if (!IsOwner) return;

            PlaySlashServerRpc(false);
        }

        // =========================
        // RPC
        // =========================

        [ServerRpc]
        void PlaySlashServerRpc(bool left)
        {
            PlaySlashClientRpc(left);
        }

        [ClientRpc]
        void PlaySlashClientRpc(bool left)
        {
            if (left)
            {
                GameObject vfx = Instantiate(
                    leftSlashVFX,
                    spawnPointTL.position,
                    spawnPointTL.rotation
                );

                vfx.transform.SetParent(spawnPointTL);

                Destroy(vfx, 1f);
            }
            else
            {
                GameObject vfx = Instantiate(
                    rightSlashVFX,
                    spawnPointTR.position,
                    spawnPointTR.rotation
                );

                vfx.transform.SetParent(spawnPointTR);

                Destroy(vfx, 1f);
            }
        }

        // =========================
        // FRONT STAB
        // =========================

        public void PlayFrontStab()
        {
            if (!IsOwner) return;

            PlayFrontStabServerRpc();
        }
                [ServerRpc]
        void PlayFrontStabServerRpc()
        {
            PlayFrontStabClientRpc();
        }
        [ClientRpc]
void PlayFrontStabClientRpc()
{
    GameObject vfx = Instantiate(
        frontStabVFX,
        spawnPointStab.position,
        spawnPointStab.rotation
    );

    vfx.transform.SetParent(spawnPointStab);

    Destroy(vfx, 1f);
}
    }