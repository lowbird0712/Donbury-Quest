using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript Inst { get; set; } = null;

    [SerializeField] PanelScript    messagePanel;
    [SerializeField] Text           dotoriNumText; 
    [SerializeField] GameObject     calander;

    bool                            isUIActive;
    [SerializeField] int            dotoriNum; // 테스트용 SerializeField
    int                             stageNum = -1;

    static public PanelScript       MessagePanel => Inst.messagePanel;
    static public int               StageNum { set { Inst.stageNum = value; } }

    static public int DotoriNum {
        get => Inst.dotoriNum;
        set {
            Inst.dotoriNum = value;
            Inst.dotoriNumText.text = value.ToString();
        }
    }

    private void Awake() => Inst = this;
    private void Start() => DotoriNum = dotoriNum;

    static public void SendStageNum() {
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
