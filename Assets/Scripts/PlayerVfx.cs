    using UnityEngine;
    using Unity.Netcode;
    using System.Collections;

    public class PlayerVfx : NetworkBehaviour
    {
        [Header("AttackVFX")]
        GameObject currentCharge;
        public GameObject chargeVFX;
        public GameObject heavyBurstVFX;
        public GameObject impactVFX;
        public GameObject skillGVFX;

        public Transform handPoint;
        public Transform rightHandPoint;
        public Transform skillGPoint;

        public GameObject leftTrail;
        public GameObject rightTrail;

        public Transform leftPoint;
        public Transform rightPoint;

        public GameObject leftSwingVFX;
        public GameObject rightSwingVFX;

        [Header("JumpVFX")]
        public GameObject jumpVFX;
        public GameObject jumpLandVFX;
        public Transform jumpPoint;
        public GameObject jumpTrail;

        [Header("WalkVFX")]
        public GameObject walkVFX;
        public Transform leftFootPoint;
        public Transform rightFootPoint;

        [Header("HealVFX")]
        public GameObject healGlowVFX;
        public GameObject healVFX;
        public Transform healVfxPoint;

        public Animator animator;

        GameObject currentGlow;

        void Update()
            {
                if (!IsOwner) return;

                if (Input.GetKeyDown(KeyCode.J))
                    StartCoroutine(HealRoutine());
            }

            IEnumerator HealRoutine()
            {
                animator.SetTrigger("Heal");

                yield return new WaitForSeconds(0.5f);

                StartHealGlowServerRpc();

                yield return new WaitForSeconds(1f);

                PlayHealVFXServerRpc();

                yield return new WaitForSeconds(5f);
            }

            // =========================
            // HEAL
            // =========================

            [ServerRpc]
            void StartHealGlowServerRpc()
            {
                StartHealGlowClientRpc();
            }

            [ClientRpc]
            void StartHealGlowClientRpc()
            {
                currentGlow = Instantiate(healGlowVFX, healVfxPoint.position, Quaternion.identity);
                currentGlow.transform.SetParent(healVfxPoint);
            }

            [ServerRpc]
            void PlayHealVFXServerRpc()
            {
                PlayHealVFXClientRpc();
            }

            [ClientRpc]
            void PlayHealVFXClientRpc()
            {
                GameObject vfx = Instantiate(healVFX, healVfxPoint.position, Quaternion.identity);
                vfx.transform.SetParent(healVfxPoint);

                Destroy(vfx, 2f);

                if (currentGlow != null)
                    Destroy(currentGlow);
            }
            public void StartHealGlow()
        {
            if (!IsOwner) return;
            StartHealGlowServerRpc();
        }

        public void PlayHealVFX()
        {
            if (!IsOwner) return;
            PlayHealVFXServerRpc();
        }

            // =========================
            // CHARGE + HEAVY
            // =========================

            [ServerRpc]
            public void StartChargeServerRpc()
            {
                StartChargeClientRpc();
            }

            [ClientRpc]
            void StartChargeClientRpc()
            {
                if (currentCharge != null)
                    Destroy(currentCharge);

                currentCharge = Instantiate(chargeVFX, handPoint.position, Quaternion.identity);
                currentCharge.transform.SetParent(handPoint);
                currentCharge.transform.localPosition = Vector3.zero;
                currentCharge.transform.localRotation = Quaternion.identity;
                currentCharge.transform.localScale = Vector3.one;
            }

            [ServerRpc]
            public void ReleaseHeavyServerRpc(Vector3 pos, Vector3 forward)
            {
                ReleaseHeavyClientRpc(pos, forward);
            }

            [ClientRpc]
            void ReleaseHeavyClientRpc(Vector3 pos, Vector3 forward)
            {
                if (currentCharge != null)
                    Destroy(currentCharge);

                GameObject burst = Instantiate(heavyBurstVFX, pos, Quaternion.LookRotation(forward));
                Destroy(burst, 1f);
            }
            public void ReleaseHeavy()
        {
            if (!IsOwner) return;

            Vector3 pos = handPoint.position + transform.forward * 0.6f + Vector3.up * 0.2f;

            ReleaseHeavyServerRpc(pos, transform.forward);
        }
         [ServerRpc]
        public void StopChargeServerRpc()
        {
            StopChargeClientRpc();
        }

        [ClientRpc]
        void StopChargeClientRpc()
        {
            if (currentCharge != null)
            {
                Destroy(currentCharge);
                currentCharge = null;
            }
        }
        public void StopCharge()
        {
            if (!IsOwner) return;

            StopChargeServerRpc();
        }

            // =========================
            // TRAILS
            // =========================

            [ServerRpc]
            public void SetTrailServerRpc(bool left, bool state)
            {
                SetTrailClientRpc(left, state);
            }

            [ClientRpc]
            void SetTrailClientRpc(bool left, bool state)
            {
                if (left) leftTrail.SetActive(state);
                else rightTrail.SetActive(state);
            }
            public void EnableLeftTrail()
        {
            if (!IsOwner) return;
            SetTrailServerRpc(true, true);
        }

        public void DisableLeftTrail()
        {
            if (!IsOwner) return;
            SetTrailServerRpc(true, false);
        }

        public void EnableRightTrail()
        {
            if (!IsOwner) return;
            SetTrailServerRpc(false, true);
        }

        public void DisableRightTrail()
        {
            if (!IsOwner) return;
            SetTrailServerRpc(false, false);
        }
        // =========================
        // RESET TRAILS
        // =========================

        [ServerRpc]
        public void ResetTrailsServerRpc()
        {
            ResetTrailsClientRpc();
        }

        [ClientRpc]
        void ResetTrailsClientRpc()
        {
            leftTrail.SetActive(false);
            rightTrail.SetActive(false);

            // optional para clear agad yung lumang trail
            if (leftTrail.TryGetComponent(out TrailRenderer lt))
                lt.Clear();

            if (rightTrail.TryGetComponent(out TrailRenderer rt))
                rt.Clear();
        }

        public void ResetTrails()
        {
            if (!IsOwner) return;

            ResetTrailsServerRpc();
        }       

            // =========================
            // SWING
            // =========================

            [ServerRpc]
            public void PlaySwingServerRpc(bool left)
            {
                PlaySwingClientRpc(left);
            }

            [ClientRpc]
            void PlaySwingClientRpc(bool left)
            {
                if (left)
                {
                    leftTrail.SetActive(true);
                    GameObject vfx = Instantiate(leftSwingVFX, leftPoint.position, Quaternion.identity);
                    vfx.transform.SetParent(leftPoint);
                    Destroy(vfx, 1f);
                }
                else
                {
                    rightTrail.SetActive(true);
                    GameObject vfx = Instantiate(rightSwingVFX, rightPoint.position, Quaternion.identity);
                    vfx.transform.SetParent(rightPoint);
                    Destroy(vfx, 1f);
                }
            }

            public void PlayLeftSwing()
        {
            if (!IsOwner) return;
            PlaySwingServerRpc(true);
        }

        public void PlayRightSwing()
        {
            if (!IsOwner) return;
            PlaySwingServerRpc(false);
        }

            // =========================
            // BASIC attack IMPACT
            // =========================
            public void PlayImpactLeft()
        {
            if (!IsOwner) return;

            Vector3 pos = handPoint.position + transform.forward * 0.6f;

            PlayImpactServerRpc(pos, transform.forward);
        }

        public void PlayImpactRight()
        {
            if (!IsOwner) return;

            Vector3 pos = rightHandPoint.position + transform.forward * 0.6f;

            PlayImpactServerRpc(pos, transform.forward);
        }

            [ServerRpc]
            public void PlayImpactServerRpc(Vector3 pos, Vector3 forward)
            {
                PlayImpactClientRpc(pos, forward);
            }

            [ClientRpc]
            void PlayImpactClientRpc(Vector3 pos, Vector3 forward)
            {
                GameObject vfx = Instantiate(impactVFX, pos, Quaternion.LookRotation(forward));
                Destroy(vfx, 1f);
            }

            // =========================
            // WALK
            // =========================

            [ServerRpc]
            public void PlayFootstepServerRpc(bool left)
            {
                PlayFootstepClientRpc(left);
            }

            [ClientRpc]
            void PlayFootstepClientRpc(bool left)
            {
                Transform point = left ? leftFootPoint : rightFootPoint;

                GameObject vfx = Instantiate(walkVFX, point.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            // =========================
            // JUMP
            // =========================

            [ServerRpc]
            public void PlayJumpServerRpc()
            {
                PlayJumpClientRpc();
            }

            [ClientRpc]
            void PlayJumpClientRpc()
            {
                GameObject vfx = Instantiate(jumpVFX, jumpPoint.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            [ServerRpc]
            public void PlayLandServerRpc()
            {
                PlayLandClientRpc();
            }

            [ClientRpc]
            void PlayLandClientRpc()
            {
                GameObject vfx = Instantiate(jumpLandVFX, jumpPoint.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }

                public void PlayJumpVFX()
            {
                if (!IsOwner) return;
                PlayJumpServerRpc();
            }

            public void PlayLandVFX()
            {
                if (!IsOwner) return;
                PlayLandServerRpc();
            }

            // =========================
            // JUMP TRAIL
            // =========================

            [ServerRpc]
            public void SetJumpTrailServerRpc(bool state)
            {
                SetJumpTrailClientRpc(state);
            }

            [ClientRpc]
            void SetJumpTrailClientRpc(bool state)
            {
                jumpTrail.SetActive(state);
            }
                public void EJumpTrail()
            {
                if (!IsOwner) return;
                SetJumpTrailServerRpc(true);
            }

            public void DisJumpTrail()
            {
                if (!IsOwner) return;
                SetJumpTrailServerRpc(false);
            }

            // =========================
            // Skill Guantlet
            // =========================

            [ServerRpc]
            public void skillGServerRpc(Vector3 pos, Vector3 forward)
            {
                skillGClientRpc(pos, forward);
            }

            [ClientRpc]
            void skillGClientRpc(Vector3 pos, Vector3 forward)
            {
                GameObject vfx = Instantiate(skillGVFX, pos, Quaternion.LookRotation(forward));
                Destroy(vfx, 1f);
            }

            public void GSkillVFX()
            {
                if (!IsOwner) return;

                Vector3 pos = skillGPoint.position;

                skillGServerRpc(pos, skillGPoint.forward);
            }
            
    }