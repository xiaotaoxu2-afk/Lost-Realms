using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WASDSound;

namespace WASDSound
{
    public class WASDFootstepSource : MonoBehaviour
    {
        [Tooltip("If the Audiosource isn't set, a new get created in Awake")]
        public AudioSource audioSource;
        public bool sound3D = true;
        [SerializeField] bool randomisePitch = false;
        [Tooltip("Range +-."), SerializeField]
        public float randomPitchRange = 1;
        [Tooltip("Range +-."), SerializeField]
        public float pitchOffset = 1;

        public WASDFootstepManager footsteps;

        WASDEnumAction action = WASDEnumAction.Walk;
        WASDEnumMaterial material = WASDEnumMaterial.Stone;

        private void Awake()
        {
            if (this.gameObject.GetComponent<AudioSource>())
            {
                audioSource = this.gameObject.GetComponent<AudioSource>();
            }
            else
            {
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

            if (sound3D)
            {
                audioSource.spatialBlend = 1;
            }
            else
            {
                audioSource.spatialBlend = 0;
            }
        }

        public void PlayFootstep()
        {
            AudioClip clip = footsteps.GetAudioClip(action, material);
            audioSource.PlayOneShot(clip);
        }

        public void PlayFootstepByAction(WASDEnumAction a)
        {
            action = a;
            PlayFootstep();
        }

        public void PlayFootstepByAction(WASDEnumAction a, WASDEnumMaterial m)
        {
            action = a;
            material = m;
            PlayFootstep();
        }

        public void SetMaterial(WASDEnumMaterial m)
        {
            material = m;
        }

        public void SetAction(WASDEnumAction a)
        {
            action = a;
        }
    }

}
