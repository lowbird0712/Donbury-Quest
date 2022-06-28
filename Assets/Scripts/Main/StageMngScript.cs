using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMngScript : MonoBehaviour {
    static public StageMngScript Inst { get; set; } = null;

    [SerializeField] StageButtonScript[]    stageButtons;
    List<StageButtonScript>                 stageButtonList;
    int                                     nextUnblockIndex = 1;

    private void Awake() => Inst = this;
    private void Start() => stageButtonList = new List<StageButtonScript>(stageButtons);

    static public void UnBlockNext() {
        if (Inst.nextUnblockIndex >= Inst.stageButtonList.Count)
            return;
        Inst.stageButtonList[Inst.nextUnblockIndex].UnBlock();
        Inst.nextUnblockIndex++;
    }
}
