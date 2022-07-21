using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Utils : MonoBehaviour {
    ///////////////////// 비주얼노벨 적용 변수 ///////////////////////////////////////////////////////////////////////////////
    static public float             storyBlockFlipTime = 0.3f;
    static public float             storyBlockFadeTime = 0.5f;

    ///////////////////// 카드게임 적용 변수 //////////////////////////////////////////////////////////////////////////////////
    static public Vector3           cardScale = new Vector3(6.0f, 6.0f, 1.0f);
    static public float             cardDragFloat = 1.5f;
    static public float             cardPutUpFloat = 3.0f;
    static public float             cardPutUpDownDotweenTime = 0.3f;
    static public float             cardSwapDotweenTime = 0.1f;
    static public float             cardPutCoverDotweenTime = 0.1f;
    static public float             cardAlignmentDotweenTime = 0.1f;

    ///////////////////// 패스트모드 적용 변수 ///////////////////////////////////////////////////////////////////////////////
    static public float             fastModeFloat = 0.1f;
    static public float             cardDrawDotweenTime = 0.5f;
    static public float             cardDrawExtraTime = 0.1f;
    static public float             turnStartPanelUpDownDotweenTime = 0.3f;
    static public float             turnStartPanelAppendDotweenTIme = 0.9f;
    static public float             cardExecDotweenTime = 0.3f;

    static public Vector3 MousePos {
        get {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z += 5;
            return pos;
        }
    }
}
