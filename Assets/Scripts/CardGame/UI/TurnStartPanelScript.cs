using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurnStartPanelScript : PanelScript {
    override public void Show() {
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, Utils.turnStartPanelUpDownDotweenTime)).SetEase(Ease.InOutQuad)
            .AppendInterval(Utils.turnStartPanelAppendDotweenTIme)
            .Append(transform.DOScale(Vector3.zero, Utils.turnStartPanelUpDownDotweenTime)).SetEase(Ease.InOutQuad);
    }

    override public void Show(string _msg) {
        if (textTMP != null)
            textTMP.text = _msg;
        else
            text.text = _msg;
        Show();
    }
}
