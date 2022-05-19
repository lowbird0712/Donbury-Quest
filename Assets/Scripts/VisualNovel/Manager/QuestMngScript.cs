using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Quest {
	public string name = "";
	public string[] intConditionNames;
	public int[] intConditions;
	public string[] boolConditionNames;
	public bool[] boolConditions;
	public int clearedBlockIndex = -1;
}

public class QuestConditions {
	public int[] intConditions;
	public bool[] boolConditions;
}

public class QuestMngScript : MonoBehaviour {
	static public QuestMngScript Inst { get; set; } = null;

	[SerializeField]
	Image mQuestPanel = null;
	[SerializeField]
	Text mQuestText = null;
	[SerializeField]
	Image mConditionPanel = null;
	[SerializeField]
	Text mConditionTextProto = null;

	Text[] mIntConditionTexts = null;
	Text[] mBoolConditionTexts = null;
	float mConditionTextY = 0;

	Quest[] mQuests;
	Quest mOriginalQuest = new Quest();
	QuestConditions mDoingQuestConditions = new QuestConditions();
	//string                          mDoingQuestMapName = null;
	//int                             mDoingQuestIndex = -1;
	bool mIsClearChecking = false;

	static public Image QuestPanel { get { return Inst.mQuestPanel; } }
	static public Text QuestText { get { return Inst.mQuestText; } }
	static public Quest OriginalQuest { get { return Inst.mOriginalQuest; } }
	static public QuestConditions DoingQuestConditions { get { return Inst.mDoingQuestConditions; } }
	static public bool IsClearChecking { set { Inst.mIsClearChecking = value; } }

	private void Awake() {
		if (!Inst)
			Inst = this;
	}

	private void Update() {
		if (ShouldBlocked())
			return;

		// 퀘스트 창을 연다
		if (Input.GetKeyDown(KeyCode.Q)) {
			GameObject questPanel = mQuestPanel.gameObject;

			if (mQuestPanel.IsActive())
				questPanel.SetActive(false);
			else {
				questPanel.SetActive(true);
				PlayerScript.FreezeAnimation();
			}
		}

		if (mIsClearChecking && ClearCheck()) {
			StoryBlockMngScript.StartStoryBlock(OriginalQuest.clearedBlockIndex);
			IsClearChecking = false;
		}
	}

	static public void LoadQuest(string _mapName) {
		if (_mapName == "TayuLivingRoom") {
			if (MainGameMngScript.StorySelectingIndex == 1) {
				//Inst.mQuests = new Quest[1];

				//// 방문을 열어 바깥으로 나가기
				//Inst.mQuests[0] = new Quest();
				//Inst.mQuests[0].name = "방문을 열고 바깥으로 나가자!";
			}
		}
		//else if (_mapName == "Minimori1stRoad") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["Minimori1stRoad"]) {
		//            case 1:
		//                Inst.mQuests = new Quest[2];
		//                // 린에게 말 걸기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "뛰어온 린에게 말을 걸자!";
		//                // 코코로 식당으로 마저 가기
		//                Inst.mQuests[1] = new Quest();
		//                Inst.mQuests[1].name = "코코로 식당으로 마저 가자!";
		//                break;
		//            case 2:
		//                Inst.mQuests = new Quest[1];
		//                // 도토리 센터 미니모리 평야 점에 들어가기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "도토리 센터 미니모리 평야 점에 들어가자!";
		//                break;
		//            case 3:
		//                Inst.mQuests = new Quest[1];
		//                // 재료를 가지고 주방으로 돌아가기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "재료를 가지고 주방으로 돌아가자!";
		//                break;
		//        }
		//    }
		//}
		//else if (_mapName == "KokoroRestaurant") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["KokoroRestaurant"]) {
		//            case 1:
		//                Inst.mQuests = new Quest[2];
		//                // 유리 언니에게 말 걸기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "유리 언니에게 말을 걸자!";
		//                // 주방으로 들어가기
		//                Inst.mQuests[1] = new Quest();
		//                Inst.mQuests[1].name = "주방 안쪽으로 들어가자!";
		//                break;
		//            case 2:
		//                Inst.mQuests = new Quest[1];
		//                // 주방으로 다시 가서 점장님께 말 걸기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "주방으로 다시 가서 점장님께 말을 걸자!";
		//                break;
		//            case 3:
		//                // 새 퀘스트 없음
		//                break;
		//            case 4:
		//                // 지나가는 길
		//                break;
		//        }
		//    }
		//}
		//else if (_mapName == "KokoroKitchen") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["KokoroKitchen"]) {
		//            case 1:
		//                Inst.mQuests = new Quest[2];
		//                // 점장님께 설명 듣기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "점장님께 식당 일에 대한 설명을 듣자!";
		//                // 주방으로 다시 나가 손님 맞이하기
		//                Inst.mQuests[1] = new Quest();
		//                Inst.mQuests[1].name = "주방으로 다시 나가 손님을 맞이하자!";
		//                break;
		//            case 2:
		//                Inst.mQuests = new Quest[1];
		//                // 코코로 식당 입구에서 유리 언니와 합류하기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "코코로 식당 입구에서 유리 언니와 합류하자!";
		//                break;
		//            case 3:
		//                Inst.mQuests = new Quest[3];
		//                // 점장님께 "규동 기본" 배우기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "점장님께 \"규동 기본\"을 배우자!";
		//                // 점장님께 쿠킹 박스에 대한 설명 듣기
		//                Inst.mQuests[1] = new Quest();
		//                Inst.mQuests[1].name = "점장님께 쿠킹 박스에 대한 설명을 듣자!";
		//                // 조리대에서 "규동 기본" 만들어 보기
		//                Inst.mQuests[2] = new Quest();
		//                Inst.mQuests[2].name = "조리대에서 \"규동 기본\"을 만들어 보자!";
		//                break;
		//        }
		//    }
		//}
		//else if (_mapName == "DotoriMinimoriPlane") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["DotoriMinimoriPlane"]) {
		//            case 1:
		//                Inst.mQuests = new Quest[1];
		//                // 계산대로 가서 양파와 우삼겹 계산하기
		//                Inst.mQuests[0] = new Quest();
		//                Inst.mQuests[0].name = "계산대로 가서 양파와 우삼겹을 계산하자!";
		//                Inst.mQuests[0].intConditionNames = new string[2];
		//                Inst.mQuests[0].intConditionNames[0] = "미니모리규 우삼겹";
		//                Inst.mQuests[0].intConditionNames[1] = "양파";
		//                Inst.mQuests[0].intConditions = new int[2];
		//                Inst.mQuests[0].intConditions[0] = 1;
		//                Inst.mQuests[0].intConditions[1] = 1;
		//                Inst.mQuests[0].clearedBlockIndex = 1;
		//                IsClearChecking = true;
		//                break;
		//        }
		//    }
		//}
	}

	static public void SetDoingQuest(int _index) {
		// 진행 퀘스트 정보 저장 및 메모 텍스트 갱신
		Inst.mOriginalQuest.name = Inst.mQuests[_index].name;

		if (Inst.mQuests[_index].intConditionNames != null) {
			int intConditionLength = Inst.mQuests[_index].intConditionNames.Length;
			Inst.mOriginalQuest.intConditionNames = new string[intConditionLength];
			Inst.mOriginalQuest.intConditions = new int[intConditionLength];
			Inst.mDoingQuestConditions.intConditions = new int[intConditionLength];
			Inst.mIntConditionTexts = new Text[intConditionLength];

			for (int i = 0; i < intConditionLength; i++) {
				Inst.mOriginalQuest.intConditionNames[i] = Inst.mQuests[_index].intConditionNames[i];
				Inst.mOriginalQuest.intConditions[i] = Inst.mQuests[_index].intConditions[i];

				string conditionText = "";
				conditionText = Inst.mOriginalQuest.intConditionNames[i] + " (" + Inst.mDoingQuestConditions.intConditions[i]
					+ "/" + Inst.mOriginalQuest.intConditions[i] + ")";
				Inst.CreateConditionText("int", i, conditionText);
			}
		}
		else if (Inst.mIntConditionTexts != null) {
			for (int i = 0; i < Inst.mIntConditionTexts.Length; i++)
				Destroy(Inst.mIntConditionTexts[i]);
			Inst.mIntConditionTexts = null;
		}

		if (Inst.mQuests[_index].boolConditionNames != null) {
			int boolConditionLength = Inst.mQuests[_index].boolConditionNames.Length;
			Inst.mOriginalQuest.boolConditionNames = new string[boolConditionLength];
			Inst.mOriginalQuest.boolConditions = new bool[boolConditionLength];
			Inst.mDoingQuestConditions.boolConditions = new bool[boolConditionLength];
			Inst.mBoolConditionTexts = new Text[boolConditionLength];

			for (int i = 0; i < boolConditionLength; i++) {
				Inst.mOriginalQuest.boolConditionNames[i] = Inst.mQuests[_index].boolConditionNames[i];
				Inst.mOriginalQuest.boolConditions[i] = Inst.mQuests[_index].boolConditions[i];

				string conditionText = "";
				conditionText = Inst.mOriginalQuest.boolConditionNames[i] + " (" + Inst.mDoingQuestConditions.boolConditions[i]
					+ "/" + Inst.mOriginalQuest.boolConditions[i] + ")";
				Inst.CreateConditionText("bool", i, conditionText);
			}
		}
		else if (Inst.mBoolConditionTexts != null) {
			for (int i = 0; i < Inst.mBoolConditionTexts.Length; i++)
				Destroy(Inst.mBoolConditionTexts[i]);
			Inst.mBoolConditionTexts = null;
		}

		Inst.mOriginalQuest.clearedBlockIndex = Inst.mQuests[_index].clearedBlockIndex;

		//Inst.mDoingQuestMapName = SceneManager.GetActiveScene().name;
		//Inst.mDoingQuestIndex = _index;

		// 진행 퀘스트 메인 텍스트 갱신
		Inst.mQuestText.text = Inst.mOriginalQuest.name;
	}

	static public void RefreshIntConditionText(int _index) {
		Inst.mIntConditionTexts[_index].text = Inst.mOriginalQuest.intConditionNames[_index] + " (" + Inst.mDoingQuestConditions.intConditions[_index]
			+ "/" + Inst.mOriginalQuest.intConditions[_index] + ")";
	}

	static public bool ClearCheck() {
		Quest originalQuest = OriginalQuest;
		QuestConditions doingQuestConditions = DoingQuestConditions;
		bool questCleared = true;

		if (originalQuest.intConditionNames != null) {
			int intConditionLength = originalQuest.intConditionNames.Length;
			for (int i = 0; i < intConditionLength; i++) {
				if (doingQuestConditions.intConditions[i] < originalQuest.intConditions[i]) {
					questCleared = false;
					break;
				}
			}
		}

		if (originalQuest.boolConditionNames != null && questCleared) {
			int boolConditionLength = originalQuest.boolConditionNames.Length;
			for (int i = 0; i < boolConditionLength; i++) {
				if (!doingQuestConditions.boolConditions[i]) {
					questCleared = false;
					break;
				}
			}
		}

		if (questCleared)
			return true;
		else
			return false;
	}

	bool ShouldBlocked() {
		if (PlayerScript.Inst == null)
			return true;

		bool shouldBlocked = false;

		if (PlayerScript.Interact)
			shouldBlocked = true;
		if (StoryBlockMngScript.Active)
			shouldBlocked = true;
		if (GameMngScript.IsMenuActive)
			shouldBlocked = true;

		return shouldBlocked;
	}

	void CreateConditionText(string _type, int _index, string _text) {
		Text conditionText = Instantiate(Inst.mConditionTextProto);
		conditionText.transform.SetParent(Inst.mConditionPanel.transform, false);
		conditionText.gameObject.SetActive(true);
		conditionText.transform.localPosition += new Vector3(0, Inst.mConditionTextY, 0);
		conditionText.text = _text;

		if (_type == "int")
			mIntConditionTexts[_index] = conditionText;
		else if (_type == "bool")
			mBoolConditionTexts[_index] = conditionText;

		conditionText = null;

		Inst.mConditionTextY += 30;
		Inst.mConditionPanel.rectTransform.sizeDelta += new Vector2(0, 30);
	}
}
