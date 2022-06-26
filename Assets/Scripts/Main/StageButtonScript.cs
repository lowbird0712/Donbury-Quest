using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButtonScript : MonoBehaviour {
    [SerializeField] int stageNum = -1;

    public void StageButton() {
        MainGameMngScript.StageNum = stageNum;
        SceneManager.LoadScene("CardGameScene");
    }
}
