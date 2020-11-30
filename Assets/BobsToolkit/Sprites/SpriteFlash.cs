using System.Collections;
using UnityEngine;

namespace BobJeltes
{
    public class SpriteFlash : MonoBehaviour
    {
        [Header("FloodColor shader required on sprite material")]
        public SpriteRenderer spriteRenderer;
        public float duration = 0.1f;
        
        private void Awake()
        {
            SetupSpriteFlash();
        }

        public void SetupSpriteFlash()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer of " + spriteRenderer.name + " is not assinged", spriteRenderer);
                return;
            }
        }

        public void DoSpriteFlash()
        {
            StartCoroutine(DoFlashColor());
        }

        public IEnumerator DoFlashColor()
        {
            SetSpriteColor();
            yield return new WaitForSeconds(duration);
            ResetSpriteColor();
        }

        public void SetSpriteColor()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }
            spriteRenderer.material.SetFloat("FloodAmount", 1f);
        }

        public void ResetSpriteColor()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }

            Material spriteMaterial = spriteRenderer.sharedMaterial;


            float floodAmount = spriteMaterial.GetFloat("FloodAmount");
            Debug.Log("Flood amount: " + floodAmount);
            spriteMaterial.SetFloat("FloodAmount", 0f);
            floodAmount = spriteMaterial.GetFloat("FloodAmount");
            Debug.Log("Flood amount: " + floodAmount);
        }
    }
}