using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class PlayerPowerups : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.2f; // optional nicety

    private SpriteRenderer sr;
    private Material originalMat;
    private Coroutine routine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMat = sr.sharedMaterial; // keep the shared default
    }

    public void ApplyMaterial(Material effectMat, float duration)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Run(effectMat, duration));
    }

    private IEnumerator Run(Material effectMat, float duration)
    {
        // instance to avoid touching shared asset
        sr.material = new Material(effectMat);
        // optional: fade in by driving _TintStrength if your SG exposes it
        TrySet(sr.material, "_TintStrength", 1f);

        yield return new WaitForSeconds(duration);

        // optional fade out
        if (fadeTime > 0f && sr.material.HasProperty("_TintStrength"))
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime / fadeTime;
                sr.material.SetFloat("_TintStrength", Mathf.Clamp01(t));
                yield return null;
            }
        }

        sr.material = originalMat;
        routine = null;
    }

    private void TrySet(Material m, string name, float value)
    {
        if (m != null && m.HasProperty(name)) m.SetFloat(name, value);
    }
}
