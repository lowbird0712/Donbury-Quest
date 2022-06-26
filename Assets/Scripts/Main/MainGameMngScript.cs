using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript Inst { get; set; } = null;

    [SerializeField] GameObject     calander;

    bool                            isUIActive;
    int                             stageNum = -1;

    static public int StageNum { set { Inst.stageNum = value; } }

    private void Awake() => Inst = this;

    public static void SendStageNum() {
        CardGameMngScript.StageNum = Inst.stageNum;
        Destroy(Inst.gameObject);
    }


    public void CalanderButton() {
        if (!isUIActive) {
            isUIActive = true;
            calander.SetActive(true);
        }
        else if (calander.activeSelf) {
            isUIActive = false;
            calander.SetActive(false);
        }
    }
}
