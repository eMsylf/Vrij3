using Boo.Lang;
using System;
using System.Collections;
using UnityEngine;

namespace BobJeltes
{
    public class SpriteFlashMultiple : MonoBehaviour
    {
        [Header("FloodColor shader required on sprite material")]
        public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        public bool useOverrideColor = true;
        [ColorUsage(true, true)]
        public Color overrideColor = Color.white;
        public void SetOverrideColor(string color)
        {
            if (ColorUtility.TryParseHtmlString(color, out Color newColor))
                overrideColor = newColor;
            else
                Debug.LogError("Input override color did not result in a valid color: " + color, gameObject);
        }
        public float duration = 0.1f;
        
        private void Awake()
        {
            SetupSpriteFlash();
        }

        public void SetupSpriteFlash()
        {
            if (spriteRenderers == null || spriteRenderers.Count == 0)
            {
                Debug.LogError("Sprite renderer of " + name + " is not assinged", gameObject);
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
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                SetSpriteColor(spriteRenderer);
            }
            yield return new WaitForSeconds(_duration);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                ResetSpriteColor(spriteRenderer);
            }
        }

        public void SetSpriteColor(SpriteRenderer spriteRenderer)
        {
            if (spriteRenderers == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }
            Material spriteMaterial = spriteRenderer.material;
            spriteMaterial.SetFloat("FloodAmount", 1f);

            //Color color = spriteMaterial.GetColor("FloodColor");
            //Debug.Log("Color was " + color);
            if (useOverrideColor)
                spriteMaterial.SetColor("FloodColor", overrideColor);
            
            //color = spriteMaterial.GetColor("FloodColor");
            //Debug.Log("Color is now " + color);
        }

        public void ResetSpriteColor(SpriteRenderer spriteRenderer)
        {
            if (spriteRenderers == null)
            {
                Debug.LogError("Sprite renderer is null");
                return;
            }

            Material spriteMaterial = spriteRenderer.material;

            spriteMaterial.SetFloat("FloodAmount", 0f);
        }

        private void OnDisable()
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                ResetSpriteColor(spriteRenderer);
            }
        }
    }
}