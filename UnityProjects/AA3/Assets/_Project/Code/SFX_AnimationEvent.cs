using UnityEngine;

namespace _Project.Code
{
    public class SfxAnimationEvent : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip clip;
 
        public void PlaySfx()
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
 
    }
}
