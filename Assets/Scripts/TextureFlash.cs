using System.Collections;
using UnityEngine;

public class TextureFlash : MonoBehaviour
{
    [SerializeField] private Texture2D texture0;
    [SerializeField] private Texture2D texture1;
    [SerializeField] private Texture2D texture2;
    [SerializeField] private Texture2D texture3;
    [SerializeField] private Texture2D texture4;
    [SerializeField] private float displayDuration = 0.5f;
    [SerializeField] private KeyCode triggerKey = KeyCode.Space;

    private Material _material;
    private Texture2D[] _textures;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _textures = new Texture2D[] { texture0, texture1, texture2, texture3, texture4 };
        SetInvisible();
    }

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            StopAllCoroutines();
            StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        _material.mainTexture = _textures[Random.Range(0, _textures.Length)];
        _material.SetTextureScale("_BaseMap", new Vector2(1f, -1f));
        _material.SetTextureOffset("_BaseMap", new Vector2(0f, 1f));
        SetVisible();
        yield return new WaitForSeconds(displayDuration);
        SetInvisible();
    }

    private void SetVisible()   { Color c = _material.color; c.a = 1f; _material.color = c; }
    private void SetInvisible() { Color c = _material.color; c.a = 0f; _material.color = c; }
    
}
