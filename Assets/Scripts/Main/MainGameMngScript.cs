using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript Inst { get; set; } = null;

    [SerializeField]
    Button  mStartButton = null;
    [SerializeField]
    Image   mStoryMap = null;

    int     mStorySelectingIndex = -1;
    int     mInitGold = 0;

    static public int StorySelectingIndex {
        get { return Inst.mStorySelectingIndex; }
        set { Inst.mStorySelectingIndex = value; }
    }

    static public int InitGold {
        get { return Inst.mInitGold; }
        set { Inst.mInitGold = value; }
    }

    private void Awake() {
        Inst = this;
    }

    private void Update() {
        if (SceneManager.GetActiveScene().name == "ManagerScene") {
            TestInfo info = TesterScript.GetInfo();

            // 테스트 모드이면 테스트하고자 하는 정면으로 넘어간다
            // 그렇지 않으면 정상적으로 게임을 시작한다
            if (info.Using) {
                GameMngScript.PreMapName = "Testing";
                mStorySelectingIndex = info.storySelectingIndex;
                GameMngScript.VisitedMapName.Add(info.mapName, info.visitedNum);
                TesterScript.SetVisitedNum();
                SceneManager.LoadScene(info.mapName);
            }
            else {
                switch (mStorySelectingIndex) {
                    case 1:
                        SceneManager.LoadScene("TayuLivingRoom");
                        break;
                    default:
                        Debug.Log("없는 StorySelectingIndex 입니다!");
                        break;
                }
            }

            Destroy(gameObject);
        }
    }

    public void StartButton() {
        mStoryMap.gameObject.SetActive(true);
        mStartButton.interactable = false;
    }
}
