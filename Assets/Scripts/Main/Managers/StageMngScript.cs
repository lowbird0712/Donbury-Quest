using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMngScript : MonoBehaviour {
    static public StageMngScript Inst { get; set; } = null;

    [SerializeField] StageButtonScript[]    stageButtons;

    List<StageButtonScript>                 stageButtonList;
    int                                     maxUnlockIndex = 5;
    int                                     nextUnblockIndex = 1;

    static public int MaxUnlockIndex {
        get => Inst.maxUnlockIndex;
        set { Inst.maxUnlockIndex = value; }
    }

    static public int NextUnblockIndex => Inst.nextUnblockIndex;

    private void Awake() => Inst = this;
    private void Start() {
        stageButtonList = new List<StageButtonScript>(stageButtons);
        gameObject.SetActive(false);
    }

    static public void UnBlockNext() {
        if (Inst.nextUnblockIndex >= Inst.stageButtonList.Count)
            return;
        if (Inst.maxUnlockIndex < Inst.nextUnblockIndex)
            return;
        Inst.stageButtonList[Inst.nextUnblockIndex].UnBlock();
        Inst.nextUnblockIndex++;
    }
}
