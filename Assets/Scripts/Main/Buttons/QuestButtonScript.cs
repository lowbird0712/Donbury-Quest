using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestButtonScript : MonoBehaviour {
    [SerializeField] Image clearedImage;
    [SerializeField] int questIndex;

    int stageNum = -1;

    public int StageNum { set { stageNum = value; } }

    public void MakeClearedQuest() {
        clearedImage.gameObject.SetActive(true);
        GetComponent<Button>().interactable = false;
    }

    public void QuestButton() {
        MainGameMngScript.StageNum = stageNum;
        TayuBoxMngScript.QuestStageIndex = questIndex;
        SceneManager.LoadScene("CardGameScene");
    }
}