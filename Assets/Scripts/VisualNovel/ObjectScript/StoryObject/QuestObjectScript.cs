using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectScript : StoryObjectScript {
    private void Start() {
        mInteractArray = new InteractPointer[3];
        mInteractArray[0] = NotYet;
        mInteractArray[1] = QuestInteract;
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

    // void Update : 내용이 StoryObjectScript::Update와 같으므로 생략함
    // InteractIndex가 1일 경우 StoryObjectScript::Talk와 처리가 같음에 유의

    void QuestInteract() {
        bool questCleared = QuestMngScript.ClearCheck();

        if (questCleared)
            Talk();
        else
            NotYet();
    }
}
