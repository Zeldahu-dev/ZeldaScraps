using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace ZeldaScraps
{
    internal class ChristmasBauble : GrabbableObject
    {
        float targetHSV = 0f;
        float targetHSV2 = 0f;
        public bool isFancy = false;
        public float materialColorSaturation = 0.9f;
        public float materialColorValue = 0.7f;
        public float material2ColorSaturation = 0.9f;
        public float material2ColorValue = 0.7f;
        bool isRare = false;
        public GameObject rareParticles = null!;
        public GameObject rareLight = null!;
        public float chanceOfBreaking = 20;
        public AudioSource breakingAudioSource = null!;
        public ParticleSystem breakingParticles = null!;
        public override void Start()
        {
            base.Start();
            if (IsServer)
            {
                targetHSV = UnityEngine.Random.Range(0f, 1f);
                targetHSV2 = UnityEngine.Random.Range(0f, 1f);
                ChangeColorClientRpc(targetHSV, targetHSV2);
                if (UnityEngine.Random.RandomRangeInt(1,101) <= Plugin.BoundConfig.RareBaubleChance.Value)
                {
                    BecomeRareClientRpc();
                }
            }
        }

        public override void PocketItem()
        {
            base.PocketItem();
            rareLight.SetActive(false);
            rareParticles.SetActive(false);
        }

        public override void EquipItem()
        {
            base.EquipItem();
            rareLight.SetActive(isRare);
            rareParticles.SetActive(isRare);
        }

        public override void OnHitGround()
        {
            base.OnHitGround();
            if (UnityEngine.Random.Range(0,100) < chanceOfBreaking && IsServer)
            {
                BreakBaubleClientRpc();
            }
        }

        public override int GetItemDataToSave()
        {
            return isRare ? 1 : 0;
        }

        public override void LoadItemSaveData(int saveData)
        {
            isRare = (saveData == 1);
        }

        [ClientRpc]
        public void ChangeColorClientRpc(float HSV, float HSV2)
        {
            mainObjectRenderer.materials[0].color = Color.HSVToRGB(HSV, materialColorSaturation, materialColorValue);
            if (isFancy)
            {
                mainObjectRenderer.materials[2].color = Color.HSVToRGB(HSV2, material2ColorSaturation, material2ColorValue);
            }
        }

        [ClientRpc]
        public void BecomeRareClientRpc()
        {
            isRare = true;
            SetScrapValue(scrapValue * 4);
            rareLight.SetActive(true);
            rareParticles.SetActive(true);

        }

        [ClientRpc]
        public void BreakBaubleClientRpc()
        {
            StartCoroutine(BreakBaubleCoroutine());
        }
        private IEnumerator BreakBaubleCoroutine()
        {
            breakingAudioSource.Play();
            breakingParticles.GetComponent<Renderer>().materials[0].color = Color.HSVToRGB(targetHSV, materialColorSaturation, material2ColorValue);
            breakingParticles.Play();
            mainObjectRenderer.gameObject.SetActive(false);
            GetComponentInChildren<ScanNodeProperties>().gameObject.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            if (isRare)
            {
                rareLight.SetActive(false);
                rareParticles.SetActive(false);
                Landmine.SpawnExplosion(transform.position, true, 0, 1, 20, 20);
            }
            yield return new WaitForSeconds(15f);
            if (IsServer)
            {
                GetComponent<NetworkObject>().Despawn(true);
            }
            yield break;
        }
    }
}
