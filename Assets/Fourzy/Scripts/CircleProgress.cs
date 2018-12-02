//modded @vadym udod

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CircleProgress : MonoBehaviour 
{
    [SerializeField]
    private Color progressColor = Color.white;

    [SerializeField, Range(0.0f, 1.0f)]
    private float currentValue = 0;

    private Material progressMaterial;
    private Coroutine progressCoroutine;

    private int progressUniform = Shader.PropertyToID("_Progress");
    private int colorUniform = Shader.PropertyToID("_Color");
    private int rectMainTexUniform = Shader.PropertyToID("_RectMainTex");

    private bool initialized = false;

    protected void Awake()
    {
        Init();
        SetupNewValue(currentValue);
    }

    protected void OnValidate()
    {
        if (initialized)
            SetupNewValue(currentValue);
        else
            Init();
    }

    private void Init()
    {
        if (initialized)
            return;

        initialized = true;
        Sprite sprite = null;

        Image image = GetComponent<Image>();

        if (image)
        {
            progressMaterial = image.material;
            sprite = image.sprite;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            progressMaterial = spriteRenderer.material; 
            sprite = spriteRenderer.sprite;
        }

        if (sprite != null)
        {
            Vector4 rectMainTex = new Vector4(sprite.textureRect.min.x / sprite.texture.width,
                             sprite.textureRect.min.y / sprite.texture.height,
                             sprite.textureRect.max.x / sprite.texture.width,
                             sprite.textureRect.max.y / sprite.texture.height);
            progressMaterial.SetVector(rectMainTexUniform, rectMainTex);
        }

        progressMaterial.SetColor(colorUniform, progressColor);
    }

    public void SetupNewValue(float value, bool animated = true, float animationDuration = 1.5f)
    {
        Init();

        if (progressCoroutine != null)
        {
            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        if (animated && gameObject.activeInHierarchy)
            progressCoroutine = StartCoroutine(SetupAnimatedProgress(value, animationDuration));
        else
            SetupNewValue(value);
    }

    public void SetupNewColor(Color color)
    {
        Init();

        progressColor = color;
        progressMaterial.SetColor(colorUniform, progressColor);
    }

    private void SetupNewValue(float value)
    {
        currentValue = value;
        progressMaterial.SetFloat(progressUniform, currentValue * Mathf.PI * 2);
        progressMaterial.SetColor(colorUniform, progressColor);
    }

    private IEnumerator SetupAnimatedProgress(float value, float animationDuration)
    {
        float curValue = currentValue;
        float newValue = value;

        for (float time = 0; time < animationDuration; time += Time.deltaTime)
        {
            currentValue = Mathf.SmoothStep(curValue, newValue, time / animationDuration);
            SetupNewValue(currentValue);

            yield return null;
        }
    }
}
