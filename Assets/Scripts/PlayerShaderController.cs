using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShaderController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material defaultMaterial;
    private Coroutine tintRoutine;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.sharedMaterial; // cache the base one
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void TriggerEffect(Material effectMat, float duration)
    {
        if (tintRoutine != null) StopCoroutine(tintRoutine);
        tintRoutine = StartCoroutine(TintRoutine(effectMat, duration));
    }

    private IEnumerator TintRoutine(Material effectMat, float duration)
    {
        // Assign effect material (make instance)
        sr.material = new Material(effectMat);

        // Fade in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            sr.material.SetFloat("_TintStrength", t);
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        while (t > 0f)
        {
            t -= Time.deltaTime;
            sr.material.SetFloat("_TintStrength", t);
            yield return null;
        }

        // Reset
        sr.material = defaultMaterial;
        tintRoutine = null;
    }
}
