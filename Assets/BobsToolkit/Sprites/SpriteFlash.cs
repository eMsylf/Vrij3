using System.Collections;
using UnityEngine;

namespace BobJeltes
{
    [System.Serializable]
    public class SpriteFlash
    {
        public bool enabled = true;
        public SpriteRenderer spriteRenderer;
        public Color color = Color.red;
        public float duration = 0.1f;
        
        private Color originalColor;
        private bool originalColorSet = false;

        public void SetupSpriteFlash(SpriteRenderer parent)
        {
            if (parent != null)
                spriteRenderer = parent;

            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite renderer of " + spriteRenderer.name + " is not assinged", spriteRenderer);
                return;
            }
        }

        public IEnumerator DoFlashColor()
        {
            if (!enabled)
                yield break;
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