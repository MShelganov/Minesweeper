using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    private Renderer rendererForOffset;
    private Vector2 offset;
    private float scrollSpeed = 0.1f;

    void Start()
    {
        rendererForOffset = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        offset.y = scrollSpeed * Time.time;
        rendererForOffset.material.SetTextureOffset("_MainTex", offset);
    }

    public void ResetOffest()
    {
        offset.y = 0;
    }
}
