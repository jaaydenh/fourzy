using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CircleProgress : MonoBehaviour 
{
    [SerializeField]
    private Material progressMaterial;

    [SerializeField]
    private Color progressColor = Color.white;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float currentValue = 0;

    private Coroutine progressCoroutine;

    private int progressUniform = Shader.PropertyToID("_Progress");
    private int colorUniform;

    private void Awake()
    {
        progressMaterial = new Material(progressMaterial);

        Image image = this.GetComponent<Image>();
        if (image)
        {
            image.material = progressMaterial;
        }

        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.sharedMaterial = progressMaterial;
        }

        progressUniform = Shader.PropertyToID("_Progress");
        colorUniform = Shader.PropertyToID("_Color");
    }

    public void SetupNewValue(float value, bool animated = true, float animationDuration = 1.5f)
    {
        if (progressCoroutine != null)
        {
            this.StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        if (animated)
        {
            progressCoroutine = StartCoroutine(SetupAnimatedProgress(value, animationDuration));
        }
        else
        {
            this.SetupNewValue(value);
        }
    }

    IEnumerator SetupAnimatedProgress(float value, float animationDuration)
    {
        float curValue = currentValue;
        float newValue = value;

        for (float time = 0; time < animationDuration; time += Time.deltaTime)
        {
            currentValue = Mathf.SmoothStep(curValue, newValue, time / animationDuration);
            this.SetupNewValue(currentValue);

            yield return null;
        }
    }

    private void SetupNewValue(float value)
    {
        currentValue = value;
        progressMaterial.SetFloat(progressUniform, currentValue * Mathf.PI * 2);
        progressMaterial.SetColor(colorUniform, progressColor);
    }

    void OnValidate()
    {
        this.SetupNewValue(currentValue);
    }
}
