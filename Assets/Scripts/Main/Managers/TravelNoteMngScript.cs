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
                        leftPageText.text = "���� �������� �ر��� �� �����ϴ�! ���������� ���� ���������� ���� �ر����ּ���!";
                        leftUnlockText.text = "���";
                        break;
                    case PageState.PS_LOCKED:
                        leftPageText.text = "���丮 3���� ����� �ر��� �� �ֽ��ϴ�! ������ �Ʒ��� ��ư�� ������ �ر��� ����˴ϴ�!";
                        leftUnlockText.text = "���";
                        break;
                    case PageState.PS_UNLOCKED:
                        leftPageText.text = pageList[leftPageIndex].storyText;
                        leftUnlockText.text = "�Ϸ�";
                        break;
                }
                rightPageText.text = "������ �������Դϴ�.";
                rightUnlockText.text = "���";
                return;
            }
            if (pageList[leftPageIndex].state == PageState.PS_BLOCKED) {
                leftPageText.text = "���� �������� �ر��� �� �����ϴ�! ���������� ���� ���������� ���� �ر����ּ���!";
                rightPageText.text = "���� �������� �ر��� �� �����ϴ�! ���������� ���� ���������� ���� �ر����ּ���!";
                leftUnlockText.text = "���";
                rightUnlockText.text = "���";
            }
            else if (pageList[leftPageIndex].state == PageState.PS_LOCKED) {
                leftPageText.text = "���丮 3���� ����� �ر��� �� �ֽ��ϴ�! ������ �Ʒ��� ��ư�� ������ �ر��� ����˴ϴ�!";
                rightPageText.text = "���� �������� �ر��� �� �����ϴ�! ���������� ���� ���������� ���� �ر����ּ���!";
                leftUnlockText.text = "3";
                rightUnlockText.text = "���";
            }
            else if (pageList[leftPageIndex + 1].state == PageState.PS_LOCKED) {
                leftPageText.text = pageList[leftPageIndex].storyText;
                rightPageText.text = "���丮 3���� ����� �ر��� �� �ֽ��ϴ�! ������ �Ʒ��� ��ư�� ������ �ر��� ����˴ϴ�!";
                leftUnlockText.text = "�Ϸ�";
                rightUnlockText.text = "3";
            }
            else {
                leftPageText.text = pageList[leftPageIndex].storyText;
                rightPageText.text = pageList[leftPageIndex + 1].storyText;
                leftUnlockText.text = "�Ϸ�";
                rightUnlockText.text = "�Ϸ�";
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
        page.storyText = "������ �Ϸ� ��Ʋ ������ �� ���� �츮�� ù �������� \"���丮�� ����\"�� �����Ѵ�." +
                        " ���� ���� ���丮 ������ �ڶ� �̷���� �����̰� �̸�ó�� ���丮�� ���� ������ �����̴�." +
                        " ��ħ �츮�� ������ �������� \"���丮 ����\"�� ���۵Ǵ� �ñ��̱⵵ �ϴ� ���� ���� ��밡 ���� �ȴ�.";
        page.state = PageState.PS_LOCKED;
        pageList.Add(page);
        page = new Page();
        page.storyText = "2 ������!";
        page.state = PageState.PS_BLOCKED;
        pageList.Add(page);
        page = new Page();
        page.storyText = "3 ������!";
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
            MainGameMngScript.MessagePanel.Show("���丮 ������ �����մϴ�!");
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
            MainGameMngScript.MessagePanel.Show("���丮 ������ �����մϴ�!");
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
