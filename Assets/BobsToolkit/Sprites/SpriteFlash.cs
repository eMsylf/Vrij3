using System.Collections;
using UnityEngine;

namespace BobJeltes
{
    public class SpriteFlash : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Color color = Color.red;
        public float duration = 0.1f;
        
        private Color originalColor;
        private bool originalColorSet = false;

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
            if (!originalColorSet)
            {
                originalColor = spriteRenderer.color;
                originalColorSet = true;
            }
            spriteRenderer.color = color;
        }

        public void ResetSpriteColor()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }
            spriteRenderer.color = originalColor;
        }
    }
}