using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButtonScript : MonoBehaviour {
    [SerializeField] Text   stageText;
    [SerializeField] Text   neededDotoriNumText;
    [SerializeField] Image  unlockImage;
    [SerializeField] Image  unBlockImage;
    [SerializeField] int    stageNum = -1;
    [SerializeField] int    neededDotoriNum;

    private void Start() {
        stageText.text = stageNum.ToString();
        neededDotoriNumText.text = neededDotoriNum.ToString();
    }

    public void UnBlock() => unBlockImage.enabled = false;

    public void StageButton() {
        if (MainGameMngScript.DotoriNum < neededDotoriNum) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        else if (unlockImage.IsActive()) {
            MainGameMngScript.DotoriNum -= neededDotoriNum;
            neededDotoriNumText.enabled = false;
            unlockImage.enabled = false;
            StageMngScript.UnBlockNext();
            return;
        }
        
        MainGameMngScript.StageNum = stageNum;
        SceneManager.LoadScene("CardGameScene");
    }
}
