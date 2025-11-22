using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace ZeldaScraps
{
    internal class DancingCactus : GrabbableObject
    {
        bool isMakingNoise = false;
        float timeTillNextNoise = 0;
        public AudioSource songSource = null!;
        public Animator animator = null!;

        public override void Update()
        {
            base.Update();
            if (IsServer)
            {
                if (timeTillNextNoise <= 0)
                {
                    if (UnityEngine.Random.RandomRangeInt(1,101) <= 10)
                    {
                        timeTillNextNoise = UnityEngine.Random.Range(13f, 15f);
                    }
                    else
                    {
                        timeTillNextNoise = UnityEngine.Random.Range(23f, 43f);
                    }
                    Debug.Log("Playing noise now, setting cooldown to " + timeTillNextNoise + " seconds");
                    MakeNoiseClientRpc();
                }
                timeTillNextNoise -= Time.deltaTime;
            }
        }

        [ClientRpc]
        public void MakeNoiseClientRpc()
        {
            StartCoroutine(MakeNoiseCoroutine());
        }

        private IEnumerator MakeNoiseCoroutine()
        {
            isMakingNoise = true;
            songSource.Play();
            animator.Play("Dance");
            RoundManager.Instance.PlayAudibleNoise(transform.position,15,0.5f, 0,isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            yield return new WaitForSeconds(13f);
            isMakingNoise = false;
            animator.Play("Nothing");
            yield break;
        }
    }
}
