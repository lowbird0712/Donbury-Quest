using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMngScript : MonoBehaviour {
    static ObjectMngScript  Inst { get; set; } = null;

    [SerializeField]
    GameObject[]            mObjects = null;

    private void Awake() {
        Inst = this;
    }

    static public GameObject GetObjects(int _index) {
        return Inst.mObjects[_index];
    }

    static public void LoadObjects(string _mapName) {
        // 챕터 및 방문 횟수에 따라서 오브젝트들의 특성을 변경한다.
        if (_mapName == "Minimori1stRoad") {
            if (MainGameMngScript.StorySelectingIndex == 1) {
                switch (GameMngScript.VisitedMapName["Minimori1stRoad"]) {
                    //case 2:
                    //    // 린을 다른 곳으로 보낸다
                    //    Inst.mObjects[1].SetActive(false);
                    //    // 유리가 타유를 기다리게 한다
                    //    Inst.mObjects[2].SetActive(true);
                    //    // 도토리 센터의 문을 연다
                    //    Inst.mObjects[3].GetComponent<ObjectScript>().InteractIndex = 1;
                    //    break;
                    //case 3:
                    //    // 린을 다른 곳으로 보낸다
                    //    Inst.mObjects[1].SetActive(false);
                    //    // 코코로 식당의 문을 연다
                    //    Inst.mObjects[0].GetComponent<SimplePortalScript>().InteractIndex = 1;
                    //    break;
                }
            }
        }
        //else if (_mapName == "KokoroRestaurant") {
        //    if (MainGameMngScript.StorySelectingIndex == 1) {
        //        switch (GameMngScript.VisitedMapName["KokoroRestaurant"]) {
        //            case 2:
        //                // 주방 문을 연다
        //                Inst.mObjects[0].GetComponent<ObjectScript>().InteractIndex = 1;
        //                // 유리를 주방 안으로 보낸다
        //                Inst.mObjects[1].SetActive(false);
        //                // 린을 식탁에 앉힌다
        //                Inst.mObjects[2].SetActive(true);
        //                break;
        //            case 3:
        //                // 주방 문이 닫힌 이유를 바꾼다
        //                Inst.mObjects[0].GetComponent<SimplePortalScript>().ClosedString = "밖에서 유리 언니가 기다리고 있어.. 놓고 온 물건은 없는 것 같으니 어서 나가자..!";
        //                // 유리를 밖으로 보낸다
        //                Inst.mObjects[1].SetActive(false);
        //                // 린을 식탁에 앉힌다
        //                Inst.mObjects[2].SetActive(true);
        //                Inst.mObjects[2].GetComponent<ObjectScript>().InteractIndex = 2;
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().AfterTalkString = "밖에서 유리 언니가 부르는 거 같아~! 잘 다녀와 타유~";
        //                // 식당 문을 연다
        //                Inst.mObjects[3].GetComponent<SimplePortalScript>().InteractIndex = 1;
        //                break;
        //            case 4:
        //                // 주방 문을 연다
        //                Inst.mObjects[0].GetComponent<ObjectScript>().InteractIndex = 1;
        //                // 유리를 다른 곳으로 보낸다
        //                Inst.mObjects[1].SetActive(false);
        //                // 린을 식탁에 앉힌다
        //                Inst.mObjects[2].SetActive(true);
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().InteractIndex = 2;
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().AfterTalkString = "아직 무슨 말을 할 지 생각중이야!"; /////
        //                break;
        //        }
        //    }
        //}
        //else if (_mapName == "KokoroKitchen") {
        //    if (MainGameMngScript.StorySelectingIndex == 1) {
        //        switch (GameMngScript.VisitedMapName["KokoroKitchen"]) {
        //            case 2:
        //                // 유리와의 대화를 바꾼다
        //                Inst.mObjects[0].GetComponent<StoryObjectScript>().NotYetString = "점장님께서 찾으셔! 바로 앞에 계시니 다시 한 번 말을 걸자!";
        //                Inst.mObjects[0].GetComponent<StoryObjectScript>().AfterTalkString = "나도 거의 준비 다 됐어! 식당 입구에서 만나자!";
        //                // 홀로 나가지 못하는 이유를 바꾼다
        //                Inst.mObjects[1].GetComponent<SimplePortalScript>().ClosedString = "타유! 점장님께서 찾으셨으니 먼저 말을 걸어야 해!";
        //                // 점장과의 대화 및 퀘스트 설정, 골드 지급 여부를 바꾼다
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().AfterTalkString = "유리는 식당 입구에서 기다리고 있을 거니 미니모리 1번 도로로 다시 나가면 될 걸세.";
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().NextQuestNum = 0;
        //                Inst.mObjects[2].GetComponent<GoldGivingScript>().Giving = true;
        //                break;
        //            case 3:
        //                // 유리와의 대화를 바꾼다
        //                Inst.mObjects[0].GetComponent<StoryObjectScript>().NotYetString = "잘됐어 타유! 드디어 너의 첫 레시피를 배울 수 있게 된거야! 어서 점장님께 다시 말을 걸자!";
        //                // 점장과의 대화 및 퀘스트 설정, 락 설정을 바꾼다
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().BlockIndex = 1;
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().InteractIndexLock = true;
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().NextQuestNumIndexLock = true;
        //                Inst.mObjects[2].GetComponent<StoryObjectScript>().AffectObjectIndexLock = true;
        //                break;
        //        }
        //    }
        //}
    }
}
