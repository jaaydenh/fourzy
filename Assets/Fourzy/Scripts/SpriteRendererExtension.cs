using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SpriteRendererExtension
{
    public static void FadeInAndOutSprite(this SpriteRenderer renderer, MonoBehaviour mono, float duration, Action<SpriteRenderer> callback = null)
    {
        mono.StartCoroutine(FadeInAndOutCoroutine(renderer, duration, callback));
    }

    private static IEnumerator FadeInAndOutCoroutine(SpriteRenderer renderer, float duration, Action<SpriteRenderer> callback)
    {
        // Fading animation
        float start = Time.time;
        bool fadingOut = false;
        Color color = renderer.color;

        while (Time.time <= start + duration || color.a >= 0.00f)
        {
            if (Time.time <= start + duration) {
                if (color.a <= 0.01f) {
                    fadingOut = false;
                }
            } else {
                // if (color.a <= 0.05f) {
                //     fadingOut = false;
                // }
            }

            if (color.a >= 0.5f) {
                fadingOut = true;
            }
            

            if (fadingOut) {
                color.a = color.a - Time.deltaTime/1.5f;
            } else {
                color.a = color.a + Time.deltaTime/1.5f;
            }
            //Debug.Log("fadingOut: " + fadingOut + " , " + color.a);
            //color.a = 1f - Mathf.Clamp01((Time.time - start) / duration);
            renderer.color = color;
            yield return new WaitForEndOfFrame();
        }

        color.a = 0.0f;
        renderer.color = color;

        // Callback
        if (callback != null)
            callback(renderer);
    }
}