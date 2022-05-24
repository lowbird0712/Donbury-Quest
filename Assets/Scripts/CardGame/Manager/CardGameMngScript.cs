using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
// 테스트 주석
public class CardGameMngScript : MonoBehaviour {
    public static CardGameMngScript                                                         Inst { get; private set; }
    void Awake() => Inst = this;
    
    [SerializeField] [Tooltip("게임의 속도가 매우 빨라집니다")] bool                        fastMode;
    [SerializeField] [Tooltip("시작 카드 개수를 정합니다")] int                             startCardCount;
    [SerializeField] [Tooltip("패에 들고 있을 수 있는 최대 카드 개수를 정합니다")] int      maxCardCount;
    [SerializeField] [Tooltip("첫 턴에 놓을 수 있는 카드의 개수를 정합니다")] int           startPutCardCount;
    [SerializeField] [Tooltip("놓을 수 있는 카드의 최대 개수를 정합니다")] int              maxPutCardCount;

    [SerializeField] TurnStartPanelScript                                                   turnStartPanel;

    [Header("게임 시스템 변수")]
    public bool                                                                             isLoading;
    public List<bool>                                                                       isCoroutine = new List<bool>();

    bool                                                                                    myTurn;
    Dictionary<string, int>                                                                 stageInfo = new Dictionary<string, int>();
    Dictionary<string, int>                                                                 currentStageInfo = new Dictionary<string, int>();

    static public int                                                                       MaxCardCount => Inst.maxCardCount;
    static public int                                                                       StartPutCardCount => Inst.startPutCardCount;
    static public int                                                                       MaxPutCardCount => Inst.maxPutCardCount;
    static public bool                                                                      MyTurn => Inst.myTurn;
    static public Dictionary<string, int>                                                   StageInfo => Inst.stageInfo;
    static public Dictionary<string, int>                                                   CurrentStageInfo => Inst.currentStageInfo;

    public static List<bool> IsCoroutine => Inst.isCoroutine;

    void Start() => StartGame();

    void Update() {
#if UNITY_EDITOR
        CheatKey();
#endif
    }

    private void OnDestroy() {
        if (fastMode) {
            Utils.cardDrawDotweenTime /= Utils.fastModeFloat;
            Utils.cardDrawExtraTime /= Utils.fastModeFloat;
        }
    }

    void                StartGame() => StartCoroutine(StartGameCo());
    void                StageClear() => turnStartPanel.Show();

    void CheatKey() {
        if (Input.GetKeyDown(KeyCode.O))
            CardMngScript.AddCardItem();
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(EndTurnCo());
    }

    void GameSetup(int _stageNum) {
        if (fastMode) {
            Utils.cardDrawDotweenTime *= Utils.fastModeFloat;
            Utils.cardDrawExtraTime *= Utils.fastModeFloat;
        }
        Init(_stageNum);
        CardMngScript.Init(_stageNum);
    }

    void Init(int _stageNum) {
        switch (_stageNum) {
            case 0:
                // 규동 기본 1개 만들기
                stageInfo.Add("규동이 든 냄비(완료)", 1);
                currentStageInfo.Add("규동이 든 냄비(완료)", 0);
                break;
        }
    }

    void TurnStart() {
        CardMngScript.IncreaseCardPutCount();
        turnStartPanel.Show();
    }

    bool IsStageClear() {
        foreach (var menu in stageInfo.Keys) {
            if (stageInfo[menu] > currentStageInfo[menu])
                return false;
        }
        return true;
    }

    IEnumerator StartGameCo() {
        GameSetup(0);

        isLoading = true;

        for (int i = 0; i < startCardCount; i++) {
            CardMngScript.AddCardItem();
            yield return new WaitForSeconds(Utils.cardDrawDotweenTime + Utils.cardDrawExtraTime);
        }

        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {
        isLoading = true;

        myTurn = true;
        TurnStart();
        yield return new WaitForSeconds(Utils.turnStartPanelDotweenTime);
        CardMngScript.AddCardItem();
        yield return new WaitForSeconds(Utils.cardDrawDotweenTime);

        isLoading = false;
    }

    IEnumerator EndTurnCo() {
        myTurn = false;
        isCoroutine.Add(true);
        isCoroutine.Add(true);
        StartCoroutine(CardMngScript.ExecuteCardsCo());
        while (isCoroutine[0]) yield return null;
        StartCoroutine(GridObjectMngScript.ExecuteGridObjectCo());
        while (isCoroutine[1]) yield return null;
        if (IsStageClear())
            StageClear();
        else
            StartCoroutine(StartTurnCo());
        isCoroutine.Clear();
    }
}
