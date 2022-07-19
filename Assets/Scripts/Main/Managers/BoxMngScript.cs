using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxMngScript : MonoBehaviour {
    static public BoxMngScript Inst { get; set; } = null;

    [SerializeField] PracticeButtonScript[] questButtons;
    [SerializeField] Image[]                questImages;
    [SerializeField] Text[]                 questTexts;

    private void Awake() => Inst = this;
    void Start() {
        gameObject.SetActive(false);
        SetDailyQuest();
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

    public void SetDailyQuestCheatButton() => SetDailyQuest();
}
