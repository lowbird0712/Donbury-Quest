using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxMngScript : MonoBehaviour {
    static public BoxMngScript Inst { get; set; } = null;

    [SerializeField] QuestButtonScript[]    questButtons;
    [SerializeField] Image[]                questImages;
    [SerializeField] Text[]                 questTexts;

    int                                     questStageIndex = -1;

    static public int                       QuestStageIndex { get => Inst.questStageIndex; set { Inst.questStageIndex = value; } }

    private void Awake() => Inst = this;
    void Start() {
        gameObject.SetActive(false);
        SetDailyQuest(); //// 추후에 하루 중 첫 접속일 때에만 실행되도록 수정
    }

    public void SetDailyQuestCheatButton() => SetDailyQuest();

    static public void QuestClear() {
        Inst.questButtons[Inst.questStageIndex].MakeClearedQuest();
        Inst.questStageIndex = -1;
        MainGameMngScript.DotoriNum.Value++;
    }

    void SetDailyQuest() {
        List<Recipe>    recipeList = new List<Recipe> (RecipeBookMngScript.RecipeList);
        List<Recipe>    unLockRecipeList = new List<Recipe>();
        List<string>    questNameList = new List<string>();
        Recipe          recipe;
        int             filledQuestNum = 0;
        foreach (Recipe iter in recipeList) {
            if (iter.state == RecipeState.RS_UNLOCKED)
                unLockRecipeList.Add(iter);
        }
        for (int i = 0; i < questTexts.Length; i++) {
            questButtons[i].GetComponent<Button>().interactable = false;
            while (unLockRecipeList.Count > 0 && filledQuestNum < questTexts.Length) {
                do
                    recipe = unLockRecipeList[Random.Range(0, unLockRecipeList.Count)];
                while (questNameList.Contains(recipe.recipeName));
                questButtons[i].GetComponent<Button>().interactable = true;
                questButtons[i].StageNum = recipe.stageNum;
                questImages[i].sprite = recipe.recipeSprite;
                questTexts[i].text = recipe.recipeName;
                unLockRecipeList.Remove(recipe);
                questNameList.Add(recipe.recipeName);
                filledQuestNum++;
            }
        }
    }
}
