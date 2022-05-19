using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelScript : MonoBehaviour {
    [SerializeField] TMP_Text textTMP;

    void Start() => ScaleZero();

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;

    [ContextMenu("ScaleOne")]
    public void ScaleOne() => transform.localScale = Vector3.one;

    virtual public void Show() {
        ScaleOne();
    }

    virtual public void Show(string _msg) {
        textTMP.text = _msg;
        ScaleOne();
    }
}
