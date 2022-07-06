using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum RecipeState {
    RS_UNLOCKED,
    RS_LOCKED
}

class Recipe {
    public Sprite       recipeSprite;
    public string       recipeName;
    public int          stageNum;
    public RecipeState  state = RecipeState.RS_LOCKED;
}

public class RecipeBookMngScript : MonoBehaviour {
    static public RecipeBookMngScript Inst { get; set; } = null;

    [SerializeField] Sprite     recipeEmptySprite;
    [SerializeField] Sprite     recipeLockedSprite;
    [SerializeField] Sprite[]   recipeSprites;
    [SerializeField] Button[]   recipeButtons;
    [SerializeField] Image[]    recipeImages;
    [SerializeField] Text[]     recipeNameTexts;

    List<Recipe>                recipeList = new List<Recipe> ();
    int                         leftTopRecipeIndex;

    public int LeftTopRecipeIndex {
        get => leftTopRecipeIndex;
        set {
            leftTopRecipeIndex = value;
            for (int i = leftTopRecipeIndex; i < leftTopRecipeIndex + recipeImages.Length; i++) {
                if (i >= recipeList.Count) {
                    recipeButtons[i - leftTopRecipeIndex].interactable = false;
                    recipeImages[i - leftTopRecipeIndex].sprite = recipeEmptySprite;
                    recipeNameTexts[i - leftTopRecipeIndex].text = "";
                    continue;
                }
                switch (recipeList[i].state) {
                    case RecipeState.RS_UNLOCKED:
                        recipeButtons[i - leftTopRecipeIndex].interactable = true;
                        recipeButtons[i - leftTopRecipeIndex].GetComponent<PracticeButtonScript>().StageNum = recipeList[i].stageNum;
                        recipeImages[i - leftTopRecipeIndex].sprite = recipeList[i].recipeSprite;
                        recipeNameTexts[i - leftTopRecipeIndex].text = recipeList[i].recipeName;
                        break;
                    case RecipeState.RS_LOCKED:
                        recipeButtons[i - leftTopRecipeIndex].interactable = false;
                        recipeImages[i - leftTopRecipeIndex].sprite = recipeLockedSprite;
                        recipeNameTexts[i - leftTopRecipeIndex].text = "잠긴 레시피입니다.";
                        break;
                }
            }
        }
    }

    private void Awake() => Inst = this;
    void Start() {
        Init();
        gameObject.SetActive(false);
    }

    static public void UnLockRecipe(int _index) {
        Inst.recipeList[_index].state = RecipeState.RS_UNLOCKED;
        Inst.LeftTopRecipeIndex = 0;
    }

    void Init() {
        Recipe recipe = new Recipe();
        recipe.recipeSprite = recipeSprites[0];
        recipe.recipeName = "규동 기본";
        recipe.stageNum = 6; ////
        recipe.state = RecipeState.RS_LOCKED;
        recipeList.Add(recipe);
        LeftTopRecipeIndex = 0;
    }

    public void GoToLeft() {
        if (leftTopRecipeIndex >= 8)
            LeftTopRecipeIndex -= 8;
    }

    public void GoToRight() {
        if (leftTopRecipeIndex + 8 < recipeList.Count)
            LeftTopRecipeIndex += 8;
    }
}
