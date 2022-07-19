using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RecipeState {
    RS_UNLOCKED,
    RS_LOCKED
}

public class Recipe {
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

    static public List<Recipe>  RecipeList => Inst.recipeList;

    static public int LeftTopRecipeIndex {
        get => Inst.leftTopRecipeIndex;
        set {
            Inst.leftTopRecipeIndex = value;
            for (int i = LeftTopRecipeIndex; i < LeftTopRecipeIndex + Inst.recipeImages.Length; i++) {
                if (i >= RecipeList.Count) {
                    Inst.recipeButtons[i - LeftTopRecipeIndex].interactable = false;
                    Inst.recipeImages[i - LeftTopRecipeIndex].sprite = Inst.recipeEmptySprite;
                    Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = "";
                    continue;
                }
                switch (RecipeList[i].state) {
                    case RecipeState.RS_UNLOCKED:
                        Inst.recipeButtons[i - LeftTopRecipeIndex].interactable = true;
                        Inst.recipeButtons[i - LeftTopRecipeIndex].GetComponent<PracticeButtonScript>().StageNum = RecipeList[i].stageNum;
                        Inst.recipeImages[i - LeftTopRecipeIndex].sprite = RecipeList[i].recipeSprite;
                        Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = RecipeList[i].recipeName;
                        break;
                    case RecipeState.RS_LOCKED:
                        Inst.recipeButtons[i - LeftTopRecipeIndex].interactable = false;
                        Inst.recipeImages[i - LeftTopRecipeIndex].sprite = Inst.recipeLockedSprite;
                        Inst.recipeNameTexts[i - LeftTopRecipeIndex].text = "잠긴 레시피입니다.";
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
        LeftTopRecipeIndex = 0;
    }

    void Init() {
        Recipe recipe = new Recipe();
        recipe.recipeSprite = recipeSprites[0];
        recipe.recipeName = "규동 기본";
        recipe.stageNum = 6; //// 스테이지 구성 완료 후 7로 바꿀 것
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
