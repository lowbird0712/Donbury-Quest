using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum PageState {
    PS_UNLOCKED,
    PS_LOCKED,
    PS_BLOCKED
}

class Page {
    public string      storyText;
    public PageState   state = PageState.PS_BLOCKED;
}

public class TravelNoteMngScript : MonoBehaviour {
    static public TravelNoteMngScript Inst { get; set; } = null;

    [SerializeField] Text   leftPageText;
    [SerializeField] Text   rightPageText;
    [SerializeField] Text   leftUnlockText;
    [SerializeField] Text   rightUnlockText;

    List<Page>              pageList = new List<Page>();
    int                     leftPageIndex;

    int LeftPageIndex {
        get => leftPageIndex;
        set {
            leftPageIndex = value;
            if (leftPageIndex + 1 == pageList.Count) {
                switch (pageList[leftPageIndex].state) {
                    case PageState.PS_BLOCKED:
                        leftPageText.text = "아직 페이지를 해금할 수 없습니다! 유랑일지의 이전 페이지들을 먼저 해금해주세요!";
                        leftUnlockText.text = "잠금";
                        break;
                    case PageState.PS_LOCKED:
                        leftPageText.text = "도토리 3개를 사용해 해금할 수 있습니다! 페이지 아래의 버튼을 누르면 해금이 진행됩니다!";
                        leftUnlockText.text = "잠금";
                        break;
                    case PageState.PS_UNLOCKED:
                        leftPageText.text = pageList[leftPageIndex].storyText;
                        leftUnlockText.text = "완료";
                        break;
                }
                rightPageText.text = "마지막 페이지입니다.";
                rightUnlockText.text = "잠금";
                return;
            }
            if (pageList[leftPageIndex].state == PageState.PS_BLOCKED) {
                leftPageText.text = "아직 페이지를 해금할 수 없습니다! 유랑일지의 이전 페이지들을 먼저 해금해주세요!";
                rightPageText.text = "아직 페이지를 해금할 수 없습니다! 유랑일지의 이전 페이지들을 먼저 해금해주세요!";
                leftUnlockText.text = "잠금";
                rightUnlockText.text = "잠금";
            }
            else if (pageList[leftPageIndex].state == PageState.PS_LOCKED) {
                leftPageText.text = "도토리 3개를 사용해 해금할 수 있습니다! 페이지 아래의 버튼을 누르면 해금이 진행됩니다!";
                rightPageText.text = "아직 페이지를 해금할 수 없습니다! 유랑일지의 이전 페이지들을 먼저 해금해주세요!";
                leftUnlockText.text = "3";
                rightUnlockText.text = "잠금";
            }
            else if (pageList[leftPageIndex + 1].state == PageState.PS_LOCKED) {
                leftPageText.text = pageList[leftPageIndex].storyText;
                rightPageText.text = "도토리 3개를 사용해 해금할 수 있습니다! 페이지 아래의 버튼을 누르면 해금이 진행됩니다!";
                leftUnlockText.text = "완료";
                rightUnlockText.text = "3";
            }
            else {
                leftPageText.text = pageList[leftPageIndex].storyText;
                rightPageText.text = pageList[leftPageIndex + 1].storyText;
                leftUnlockText.text = "완료";
                rightUnlockText.text = "완료";
            }
        }
    }

    private void Awake() => Inst = this;
    private void Start() {
        Init();
        gameObject.SetActive(false);
    }

    void Init() {
        Page page = new Page();
        page.storyText = "앞으로 하루 이틀 정도만 더 가면 우리의 첫 목적지인 \"도토리숲 마을\"에 도착한다." +
                        " 많은 수의 도토리 나무가 자라 이루어진 마을이고 이름처럼 도토리가 많이 열리는 마을이다." +
                        " 마침 우리가 도착할 즈음에는 \"도토리 축제\"가 시작되는 시기이기도 하니 나도 린도 기대가 많이 된다.";
        page.state = PageState.PS_LOCKED;
        pageList.Add(page);
        page = new Page();
        page.storyText = "2 페이지!";
        page.state = PageState.PS_BLOCKED;
        pageList.Add(page);
        page = new Page();
        page.storyText = "3 페이지!";
        page.state = PageState.PS_BLOCKED;
        pageList.Add(page);
        LeftPageIndex = 0;
    }

    public void GoToLeft() {
        if (leftPageIndex >= 2)
            LeftPageIndex -= 2;
    }

    public void GoToRight() {
        if (leftPageIndex + 2 < pageList.Count)
            LeftPageIndex += 2;
    }

    public void LeftUnlock() {
        if (pageList[leftPageIndex].state != PageState.PS_LOCKED)
            return;
        if (MainGameMngScript.DotoriNum < 3) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        MainGameMngScript.DotoriNum -= 3;
        if (StageMngScript.MaxUnlockIndex < StageMngScript.NextUnblockIndex) {
            StageMngScript.MaxUnlockIndex += 5;
            StageMngScript.UnBlockNext();
        }
        StageMngScript.MaxUnlockIndex += 5;
        pageList[leftPageIndex].state = PageState.PS_UNLOCKED;
        if (leftPageIndex + 1 < pageList.Count)
            pageList[leftPageIndex + 1].state = PageState.PS_LOCKED;
        LeftPageIndex = leftPageIndex;
    }

    public void RightUnlock() {
        if (pageList[leftPageIndex + 1].state != PageState.PS_LOCKED)
            return;
        if (MainGameMngScript.DotoriNum < 3) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        MainGameMngScript.DotoriNum -= 3;
        if (StageMngScript.MaxUnlockIndex < StageMngScript.NextUnblockIndex) {
            StageMngScript.MaxUnlockIndex += 5;
            StageMngScript.UnBlockNext();
        }
        StageMngScript.MaxUnlockIndex += 5;
        pageList[leftPageIndex + 1].state = PageState.PS_UNLOCKED;
        if (leftPageIndex + 2 < pageList.Count)
            pageList[leftPageIndex + 2].state = PageState.PS_LOCKED;
        LeftPageIndex = leftPageIndex;
    }
}
