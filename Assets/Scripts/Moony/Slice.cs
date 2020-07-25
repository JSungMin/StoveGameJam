using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Slice : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Slicing()
    {
        StartCoroutine("AlphaCO");
    }
    IEnumerator AlphaCO()
    {
        Color color = spriteRenderer.color;
        while (color.a > 0.0f)
        {
            color.a -= 0.2f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
                
        
      

    }
}
