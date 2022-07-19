using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButtonScript : MonoBehaviour {
    [SerializeField] Text   stageText;
    [SerializeField] Text   neededDotoriNumText;
    [SerializeField] Image  unLockImage;
    [SerializeField] Image  unBlockImage;
    [SerializeField] int    stageNum = -1;
    [SerializeField] int    neededDotoriNum;
    [SerializeField] int    unLockRecipeIndex = -1;

    private void Start() {
        stageText.text = stageNum.ToString();
        neededDotoriNumText.text = neededDotoriNum.ToString();
    }

    public void UnBlock() => unBlockImage.enabled = false;

    public void StageButton() {
        if (MainGameMngScript.DotoriNum.Value < neededDotoriNum) {
            MainGameMngScript.MessagePanel.Show("도토리 개수가 부족합니다!");
            return;
        }
        else if (unLockImage.IsActive()) {
            MainGameMngScript.DotoriNum.Value -= neededDotoriNum;
            neededDotoriNumText.enabled = false;
            unLockImage.enabled = false;
            StageMngScript.UnBlockNext();
            if (unLockRecipeIndex != -1)
                RecipeBookMngScript.UnLockRecipe(unLockRecipeIndex);
            return;
        }
        
        MainGameMngScript.StageNum = stageNum;
        SceneManager.LoadScene("CardGameScene");
    }
}
