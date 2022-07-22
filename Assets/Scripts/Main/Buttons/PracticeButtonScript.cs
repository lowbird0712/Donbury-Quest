using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeButtonScript : MonoBehaviour {
    int         stageNum = -1;

    public int  StageNum { get => stageNum; set => stageNum = value; }

    public void PracticeButton() {
        MainGameMngScript.StageNum = stageNum;
        SceneManager.LoadScene("CardGameScene");
    }
}
