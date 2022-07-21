using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestPortalScript : SimplePortalScript {
    void Start() {
        mInteractArray = new InteractPointer[3];

        mInteractArray[0] = Closed;
        mInteractArray[1] = QuestEnter;
        mInteractArray[2] = DefaultClosed;
    }

    // void Update : 내용이 SimplePortalScript::Update와 같으므로 생략함
    // InteractIndex가 1이고 퀘스트가 아직 완료되지 않은 경우 SimplePortalScript::Closed, SimplePortalScript::DefaultClosed와 처리가 같음에 유의

    void QuestEnter() {
        bool questCleared = QuestMngScript.ClearCheck();

        if (questCleared) {
            GameMngScript.PreMapName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(mNextMapName);
        }
        else
            DefaultClosed();
    }
}
