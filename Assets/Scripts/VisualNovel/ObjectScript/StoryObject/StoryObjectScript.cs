using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryObjectScript : ObjectScript {
    [SerializeField]
    int                 mBlockIndex = -1;
    [SerializeField]
    int                 mNextQuestNum = -1;
    [SerializeField]
    string              mNotYetString = null;
    [SerializeField]
    string              mAfterTalkString = null;
    [SerializeField]
    Sprite              mNotYetSprite = null;
    [SerializeField]
    Sprite              mAfterTalkSprite = null;

    [SerializeField]
    protected int[]     mAffectObjectIndex = null;
    [SerializeField]
    protected int[]     mAffectInteractIndex = null;

    bool                mInteractIndexLock = false;
    bool                mNextQuestNumLock = false;
    bool                mAffectObjectIndexLock = false;

    public int          BlockIndex { set { mBlockIndex = value; } }
    public int          NextQuestNum { set { mNextQuestNum = value; } }
    public string       NotYetString { set { mNotYetString = value; } }
    public string       AfterTalkString { set { mAfterTalkString = value; } }
    public bool         InteractIndexLock { set { mInteractIndexLock = value; } }
    public bool         NextQuestNumIndexLock { set { mNextQuestNumLock = value; } }
    public bool         AffectObjectIndexLock { set { mAffectObjectIndexLock = value; } }

    public void         SetInteractIndex(int _objectIndex, int _interactIndex) {
        mAffectInteractIndex[_objectIndex] = _interactIndex;
    }

    void Start() {
        mInteractArray = new InteractPointer[3];
        mInteractArray[0] = NotYet;
        mInteractArray[1] = Talk;
        mInteractArray[2] = AfterTalk;

        if (mAffectObjectIndex.Length <= 0 || mAffectInteractIndex.Length <= 0) {
            Debug.LogError("AffectObject관련 Array 중 하나의 길이가 0 이하에요!");
            return;
        }
        else if (mAffectObjectIndex.Length != mAffectInteractIndex.Length) {
            Debug.LogError("mAffectObjectIndex와 mAffectInteractIndex의 길이가 달라요!");
            return;
        }
    }

    void Update() {
        if (!mInteracting)
            return;

        switch (mInteractIndex) {
            // NotYet
            case 0:
                if (Input.GetButtonDown("Jump")) {
                    mInteracting = false;
                    PlayerScript.Interact = false;
                    StoryBlockMngScript.Active = false;
                }
                break;
            // Talk
            case 1:
                if (!StoryBlockMngScript.Active && Input.GetButtonDown("Jump")) {
                    mInteracting = false;
                    PlayerScript.Interact = false;

                    if (!mInteractIndexLock)
                        mInteractIndex = 2;

                    if (!mAffectObjectIndexLock) {
                        for (int i = 0; i < mAffectObjectIndex.Length; i++)
                            ObjectMngScript.GetObjects(mAffectObjectIndex[i]).GetComponent<ObjectScript>().InteractIndex = mAffectInteractIndex[i];
                    }
                    
                    if (!mNextQuestNumLock)
                        QuestMngScript.SetDoingQuest(mNextQuestNum);

                    SetOtherThings(SceneManager.GetActiveScene().name);
                }
                break;
            // AfterTalk
            case 2:
                if (Input.GetButtonDown("Jump")) {
                    mInteracting = false;
                    PlayerScript.Interact = false;
                    StoryBlockMngScript.Active = false;
                }
                break;
        }
    }

    protected void NotYet() {
        if (mNotYetString == null) {
            Debug.LogError("mNotYetString이 null이에요!");
            return;
        }

        StoryBlockMngScript.Text.text = mNotYetString;
        StoryBlockMngScript.NPC_Name.text = mPanelName;
        StoryBlockMngScript.NPC_Panel.sprite = mNotYetSprite;
        StoryBlockMngScript.ActiveOnlyPanels();
    }

    protected void Talk() {
        if (mBlockIndex == -1) {
            Debug.LogError("mBlockIndex가 -1이에요!");
            return;
        }

        StoryBlockMngScript.BlockIndex = mBlockIndex;
        StoryBlockMngScript.Active = true;
    }

    protected void AfterTalk() {
        if (mAfterTalkString == null) {
            Debug.LogError("mAfterTalkString이 null이에요!");
            return;
        }

        StoryBlockMngScript.Text.text = mAfterTalkString;
        StoryBlockMngScript.NPC_Name.text = mPanelName;
        StoryBlockMngScript.NPC_Panel.sprite = mAfterTalkSprite;
        StoryBlockMngScript.ActiveOnlyPanels();
    }

    protected virtual void SetOtherThings(string _mapName) {
        if (_mapName == "KokoroKitchen") {
            if (MainGameMngScript.StorySelectingIndex == 1) {
                //switch (GameMngScript.VisitedMapName["KokoroKitchen"]) {
                    //case 3:
                        //switch (mBlockIndex) {
                            //// 점장이 "규동 기본"의 재료에 대해 설명해주는 부분
                            //case 1:
                                //// 점장과의 대화를 바꾼다
                                //mBlockIndex = 2;
                                //// 다음에 진행할 퀘스트를 설정한다
                                //mNextQuestNum = 1;
                                //mNextQuestNumLock = false;
                                //break;
                            //// 점장이 "규동 기본"의 조리법에 대해 설명해주는 부분
                            //case 2:
                                //// 점장과의 대화 및 퀘스트 설정, 락 설정을 바꾼다
                                //mBlockIndex = 3;
                                //mNextQuestNum = 2;
                                //mInteractIndexLock = false;
                                //// 유리와의 대화를 바꾼다
                                //ObjectMngScript.GetObjects(0).GetComponent<StoryObjectScript>().NotYetString = "엄청 신기하게 생겼지만 써 보면 금방 익숙해 질 거야! 우선 점장님께 설명을 듣자!";
                                //break;
                            //// 점장이 쿠킹 큐브에 대해 설명해주는 부분
                            //case 3:
                                //// 점장과의 대화를 바꾼다
                                //AfterTalkString = "이제 배운대로 \"규동 기본\"을 만들어보게나. 조리대는 바로 옆에 있으니 거기에서 유리 양의 도움을 받아 같이 하면 될거네.";
                                //// 유리와의 대화를 바꾼다
                                //ObjectMngScript.GetObjects(0).GetComponent<StoryObjectScript>().NotYetString = "조리대는 바로 옆에 있어! 내가 도와줄 테니까 어서 요리를 시작하자!";
                                //// 홀로 나가지 못하는 이유를 바꾼다
                                //ObjectMngScript.GetObjects(1).GetComponent<SimplePortalScript>().InteractIndex = 2;
                                //break;
                        //}
                        //break;
                //}
            }
        }
    }
}
