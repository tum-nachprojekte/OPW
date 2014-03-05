using UnityEngine;
using System.Collections;

public class BarSprite : MonoBehaviour
{

    public float Max = 10;
    public float Current = 5;
    private float Size;
    private Vector3 startPosition;
    [SerializeField]
    public TextAlignment align = TextAlignment.Center;
    private Bounds startBounds;

    void Start() {
        ResetBar();
    }

    public void ResetBar()
    {
        startPosition = transform.localPosition;
        Size = transform.localScale.x;
        startBounds = renderer.bounds;
    }

    void Update()
    {

        var position = startPosition;
        var currentSize = Current / Max * Size;        
        var scale = transform.localScale;

        if(align == TextAlignment.Center) {
        }
        else if (align == TextAlignment.Left)
        {
            position.x += scale.x / Size * startBounds.size.x / 4 - startBounds.size.x / 4;
        }
        else if (align == TextAlignment.Right) {
            position.x -= scale.x / Size * startBounds.size.x / 4 - startBounds.size.x / 4;
        }
        transform.localPosition = position;

        scale.x = Mathf.Lerp(scale.x, currentSize, Time.deltaTime * Hub.Instance.BarLerpFactor);

        transform.localScale = scale;

    }
}
