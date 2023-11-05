using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicVisual : MonoBehaviour
{
    private Vector3 maxScale;

    [SerializeField]
    public MicAtt micAtt;

    void Start()
    {
        maxScale = transform.localScale;
    }

    void Update()
    {
        micAtt.Update();
        transform.localScale = maxScale * micAtt.volume;

        if (micAtt.isLoud)
            GetComponent<Renderer>().material.color = Color.red;
        else
            GetComponent<Renderer>().material.color = Color.white;
    }
}
