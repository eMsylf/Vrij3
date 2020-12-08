using System.Collections;
using UnityEngine;

namespace BobJeltes
{
    public class SpriteFlash : MonoBehaviour
    {
        [Header("FloodColor shader required on sprite material")]
        public SpriteRenderer spriteRenderer;
        [ColorUsage(true, true)]
        public Color overrideColor = Color.white;
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
            StartCoroutine(DoFlashColor(duration));
        }

        public void DoSpriteFlash(float newDuration)
        {
            StartCoroutine(DoFlashColor(newDuration));
        }

        public IEnumerator DoFlashColor(float _duration)
        {
            SetSpriteColor();
            yield return new WaitForSeconds(_duration);
            ResetSpriteColor();
        }

        public void SetSpriteColor()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }
            Material spriteMaterial = spriteRenderer.material;
            spriteMaterial.SetFloat("FloodAmount", 1f);

            //Color color = spriteMaterial.GetColor("FloodColor");
            //Debug.Log("Color was " + color);
            
            spriteMaterial.SetColor("FloodColor", overrideColor);
            
            //color = spriteMaterial.GetColor("FloodColor");
            //Debug.Log("Color is now " + color);
        }

        public void ResetSpriteColor()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }

            Material spriteMaterial = spriteRenderer.material;

            spriteMaterial.SetFloat("FloodAmount", 0f);
        }
    }
}