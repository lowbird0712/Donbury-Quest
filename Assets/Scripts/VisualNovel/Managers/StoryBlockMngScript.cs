using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SysKey {
	public string	key;
	public string	effectKey;
	public int		panelSpriteNum;
	public int		BGM_Num;
	public int		BGM_SubNum;
	public int		effectNum;
	public bool		fade;

	public SysKey() {
		key = null;
		effectKey = null;
		panelSpriteNum = -1;
		BGM_Num = -1;
		BGM_SubNum = -1;
		effectNum = -1;
		fade = false;
	}
	public SysKey(SysKey _key) {
		key = _key.key;
		effectKey = _key.effectKey;
		panelSpriteNum = _key.panelSpriteNum;
		BGM_Num = _key.BGM_Num;
		BGM_SubNum = _key.BGM_SubNum;
		effectNum = _key.effectNum;
		fade = _key.fade;
	}
}

public class SysKeySameness {
	public bool sameKey;
	public bool sameEffectKey;
	public bool samePanelSpriteNum;
	public bool same_BGM_Num;
	public bool same_BGM_SubNum;
	public bool sameEffectNum;

	public SysKeySameness() {
		sameKey = true;
		sameEffectKey = true;
		samePanelSpriteNum = true;
		same_BGM_Num = true;
		same_BGM_SubNum = true;
		sameEffectKey = true;
	}
}

public struct StoryNPC {
	public string name;
	public Sprite sprite;

	public StoryNPC(string _name, Sprite _sprite) {
		name = _name;
		sprite = _sprite;
	}
}

public struct StoryChain {
	public SysKey sysKey;
	public string text;
	public StoryNPC NPC;
}

public class StoryBlock {
	StoryChain[] mStoryChains;
	int mLength = 0;

	public StoryChain[] StoryChains {
		get { return mStoryChains; }
		set { mStoryChains = value; }
	}
	public int Length {
		get { return mLength; }
		set { mLength = value; }
	}
}

public class StoryBlockMngScript : MonoBehaviour {
	static public StoryBlockMngScript Inst { get; set; } = null;

	[SerializeField]
	CanvasGroup mCanvas = null;
	[SerializeField]
	CanvasGroup mCanvas_NPC = null;
	[SerializeField]
	Text mText = null;
	[SerializeField]
	Text mNPC_name = null;
	[SerializeField]
	Image mNPC_panel = null;
	[SerializeField]
	Image mPanel = null;
	[SerializeField]
	Image mCloseUpPanel = null;
	[SerializeField]
	Image mFade = null;

	[SerializeField]
	Sprite[] mNPC_sprites = null;
	[SerializeField]
	Sprite[] mPanelSprites = null;

	StoryBlock[] mStoryBlocks;
	SysKey mPastSysKey = null;
	////
	bool[] mBlockNotFromObject;
	////
	int mInitBlockIndex = -1;
	int mBlockIndex = 0;
	int mBlockInnerIndex = 0;
	bool mActive = false;
	bool mFlipling = false;

	static public Text Text { get { return Inst.mText; } }
	static public Text NPC_Name { get { return Inst.mNPC_name; } }
	static public Image NPC_Panel { get { return Inst.mNPC_panel; } }
	static public Image Panel { get { return Inst.mPanel; } }
	static public Sprite[] NPC_Sprites { get { return Inst.mNPC_sprites; } }
	static public Sprite[] PanelSprites { get { return Inst.mPanelSprites; } }

	static public int BlockIndex {
		set {
			Inst.mBlockIndex = value;
			Inst.mPastSysKey = Inst.mStoryBlocks[value].StoryChains[0].sysKey;
			Inst.mText.text = Inst.mStoryBlocks[value].StoryChains[0].text;
			Inst.mNPC_name.text = Inst.mStoryBlocks[value].StoryChains[0].NPC.name;
			Inst.mNPC_panel.sprite = Inst.mStoryBlocks[value].StoryChains[0].NPC.sprite;

			if (Inst.mStoryBlocks[value].StoryChains[0].sysKey.panelSpriteNum != -1)
				Inst.mPanel.sprite = Inst.mPanelSprites[Inst.mStoryBlocks[value].StoryChains[0].sysKey.panelSpriteNum];
		}
	}
	static public bool Active {
		get { return Inst.mActive; }
		set {
			Inst.mActive = value;
			if (value)
				Inst.StartCoroutine(Inst.FlipUp());
			else
				Inst.StartCoroutine(Inst.FlipDown());
		}
	}

	static public void ActiveOnlyPanels() {
		Inst.StartCoroutine(Inst.FlipUp());
	}

	private void Awake() => Inst = this;

	private void Start() {
		Inst.mCanvas.DOFade(0, 0);
		Inst.mPanel.DOFade(0, 0);
	}

	private void Update() {
		if (!mActive)
			return;

		// 다음 스토리 체인으로 넘어간다. 마지막 체인에 도달하면 비활성화된다
		if (Input.GetButtonDown("Jump") && !mFlipling)
			StartCoroutine(Flip());
	}

	public static void LoadStoryBlock(string _mapName) {
		StoryNPC Tayu = new StoryNPC("타유", Inst.mNPC_sprites[0]);
		StoryNPC Rin = new StoryNPC("린", Inst.mNPC_sprites[1]);
		//StoryNPC Yuri = new StoryNPC("유리", Inst.mNPC_sprites[1]);
		//StoryNPC Master = new StoryNPC("점장", Inst.mNPC_sprites[2]);
		//StoryNPC Minami = new StoryNPC("미나미", Inst.mNPC_sprites[1]);

		////
		#region
		//if (_mapName == "TayuLivingRoom") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        Inst.mStoryBlocks = new StoryBlock[2];
		//        Inst.mBlockNotFromObject = new bool[2];

		//        // Start Monologue
		//        Inst.mStoryBlocks[0] = new StoryBlock();
		//        Inst.mStoryBlocks[0].Length = 10;
		//        Inst.mStoryBlocks[0].StoryChains = new StoryChain[10];
		//        StoryChain storyChain = default;
		//        storyChain.text = "(봄, 타유의 집 거실.)";
		//        Inst.mStoryBlocks[0].StoryChains[0] = storyChain;
		//        storyChain.text = "오늘부터 나도 20살이야..!";
		//        storyChain.NPC_name = "타유";
		//        storyChain.NPC_sprite = Inst.mNPC_sprites[0];
		//        Inst.mStoryBlocks[0].StoryChains[1] = storyChain;
		//        storyChain.text = "어머니, 아버지께서 마을 밖으로 나가는 것은 허락해주시지 않았지만 마을에 있는 식당 아르바이트 정도는 허락해 주셨어..!";
		//        Inst.mStoryBlocks[0].StoryChains[2] = storyChain;
		//        storyChain.text = "참.. 나도 이제 내 앞가림 정도는 할 줄 아는데.. 두 분 다 너무 걱정이 심하시다니까..";
		//        Inst.mStoryBlocks[0].StoryChains[3] = storyChain;
		//        storyChain.text = "하긴, 코코로 식당에는 어릴 적부터 자주 갔었고, 반 년 전부터 유리 언니가 거기서 먼저 일을 시작했으니까 부모님께서도 안심하고 나를 보낼 수 있으셨을거야..";
		//        Inst.mStoryBlocks[0].StoryChains[4] = storyChain;
		//        storyChain.text = "코코로 식당은 돈부리가 맛있기로 유명하니까 여기에서도 많은 것을 배울 수 있겠지..!";
		//        Inst.mStoryBlocks[0].StoryChains[5] = storyChain;
		//        storyChain.text = "그래..! 일단 일을 배울 수 있다는 것 만으로 만족하자..!";
		//        Inst.mStoryBlocks[0].StoryChains[6] = storyChain;
		//        storyChain.text = "준비물은.. 도토리.. 숟가락.. 좋아..! 다 챙겼어..!";
		//        Inst.mStoryBlocks[0].StoryChains[7] = storyChain;
		//        storyChain.text = "그럼 슬슬 나가보도록 할까..?";
		//        Inst.mStoryBlocks[0].StoryChains[8] = storyChain;
		//        storyChain.text = "(방문을 열고 바깥으로 나가자. 방문 앞에서 'C'를 누르면 문을 열 수 있다.)";
		//        storyChain.NPC_name = "Null Sprite";
		//        Inst.mStoryBlocks[0].StoryChains[9] = storyChain;
		//        Inst.mBlockNotFromObject[0] = true;

		//        StartStoryBlock(0);
		//        QuestMngScript.SetDoingQuest(0);
		//    }
		//}
		//else if (_mapName == "Minimori1stRoad") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["Minimori1stRoad"]) {
		//            case 1:
		//                Inst.mStoryBlocks = new StoryBlock[2];
		//                Inst.mBlockNotFromObject = new bool[2];
		//                // 시작 : 타유의 혼잣말
		//                Inst.mStoryBlocks[0] = new StoryBlock();
		//                Inst.mStoryBlocks[0].Length = 13;
		//                Inst.mStoryBlocks[0].StoryChains = new StoryChain[13];
		//                StoryChain storyChain = default;
		//                storyChain.text = "(미니모리 1번 골목길, 꽃향기 가득한 바람이 불고 있다.)";
		//                Inst.mStoryBlocks[0].StoryChains[0] = storyChain;
		//                storyChain.text = "달콤한 향기.. 벼 꽃이 피었구나.. 봄이 오긴 온 것 같네..";
		//                storyChain.NPC_name = "타유";
		//                storyChain.NPC_sprite = Inst.mNPC_sprites[0];
		//                Inst.mStoryBlocks[0].StoryChains[1] = storyChain;
		//                storyChain.text = "매 년 이 즈음이 되면 미니모리 평야의 벼들이 꽃을 피우고는 했지..";
		//                Inst.mStoryBlocks[0].StoryChains[2] = storyChain;
		//                storyChain.text = "그리고 여름이 오면 마을 사람들 다같이 벼를 돌보고..";
		//                Inst.mStoryBlocks[0].StoryChains[3] = storyChain;
		//                storyChain.text = "가을이 되면 드디어 알이 가득 찬 벼를 수확한 후 축하하는 의미에서 모두가 모여 큰 축제를 벌일 거야..!";
		//                Inst.mStoryBlocks[0].StoryChains[4] = storyChain;
		//                storyChain.text = "그리고 겨울 동안에는 내년을 위해 지금까지 고생한 땅을 감사하는 마음으로 쉬게 해 주는 거지..";
		//                Inst.mStoryBlocks[0].StoryChains[5] = storyChain;
		//                storyChain.text = "이렇게 생각해보니 미니모리 평야에는 쌀과 밥을 사랑하는 사람들이 정말 많이 사는 것 같아..!";
		//                Inst.mStoryBlocks[0].StoryChains[6] = storyChain;
		//                storyChain.text = "나도 그렇고.. 미니모리 평야의 쌀 덕분에 이렇게 맛있는 돈부리들을 먹을 수 있어..";
		//                Inst.mStoryBlocks[0].StoryChains[7] = storyChain;
		//                storyChain.text = "다시 한 번 미니모리 평야에게 감사하자..!";
		//                Inst.mStoryBlocks[0].StoryChains[8] = storyChain;
		//                storyChain.text = "(타유가 그렇게 봄이 왔음을 실감하는 동안 저 멀리서 타유를 부르며 뛰어오는 누군가가 보인다.)";
		//                storyChain.NPC_name = "Null Sprite";
		//                Inst.mStoryBlocks[0].StoryChains[9] = storyChain;
		//                storyChain.text = "타유~~!";
		//                storyChain.NPC_name = "린";
		//                storyChain.NPC_sprite = Inst.mNPC_sprites[1];
		//                Inst.mStoryBlocks[0].StoryChains[10] = storyChain;
		//                storyChain.text = "……헥..헥..! 같이 가 타유..!";
		//                Inst.mStoryBlocks[0].StoryChains[11] = storyChain;
		//                storyChain.text = "(뛰어오고 있는 린에게 가서 말을 걸자. NPC 앞에서‘C’를 누르면 말을 걸 수 있다.)";
		//                storyChain.NPC_name = "Null Sprite";
		//                Inst.mStoryBlocks[0].StoryChains[12] = storyChain;
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 린과의 대화
		//                Inst.MakeStoryBlock(1, 20);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "헥..헥… 겨우 따라잡았다..!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "굳이 뛰어올 필요까지 없었는데.. 무슨 일이야..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그냥~ 멀리 타유가 보여서 왔어~! 어디 가는 길이야~?", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "나 오늘부터 코코로 식당에서 아르바이트 시작하기로 했어..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "진짜~? 결국 부모님께서 허락해 주셨구나~!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "맞아.. 어찌어찌 부모님을 설득할 수 있었던 것 같아..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "다행이야~! 타유는 어릴 때부터 돈부리를 배우는 것이 꿈이었잖아~", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그렇지.. 사실은 골드스푼의 큰 레스토랑으로 가서 배우고 싶었지만 말이야..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "그래도 여기 돈부리도 엄청 맛있으니까 여러 가지 좋은 경험을 할 수 있을 거야..!");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "맞아~! 그리고 여기 미니모리 평야는 밥에 있어서는 델리델리 대륙 최고니까 골드스푼에서 못 배우는 것도 배울 수 있다고 생각해~!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "격려 고마워.. 매번 느끼지만 린은 참 상냥하구나..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "헤헤… 그런 말 들으면 쑥스러워~", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "후후.. 음.. 벌써 시간이 이렇게 됐네..?", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "이제 슬슬 가게로 가 봐야 할 것 같아..! 첫 날이라 준비할 것도 이것저것 많을 거니까..");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그래~ 첫 출근 잘 하고~ 나중에 놀러갈게~! 바이바이~!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "바이바이..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "......");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "린도 여전히 활발한 애라니까..");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "이제.. 이쪽으로 쭉 가면 코코로 식당이었지..!");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[1], "(가던 길을 마저 가면 코코로 식당이 나온다. 식당에 들어가서 출근 준비를 하자.)");
		//                // 스토리 블록 및 퀘스트 설정
		//                StartStoryBlock(0);
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//            case 2:
		//                Inst.mStoryBlocks = new StoryBlock[1];
		//                Inst.mBlockNotFromObject = new bool[1];
		//                // 시작 : 유리와의 대화
		//                Inst.MakeStoryBlock(0, 12);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "많이 기다렸지..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "아니야! 나도 나온 지 얼마 안 됐어!", Yuri);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그러면 출발하기에 앞서 우리가 구해와야 할 것들을 한 번 정리할게!");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "점장님께서 말씀하신 대로 지금 규동을 만드는 데 있어 부족한 재료는 양파하고 우삼겹이야!");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "양파는 근처의 도토리 센터에서 흔하게 팔고 있으니 그냥 사 오면 돼!");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그리고 델리델리 대륙에서의 소고기는 대부분 바로 옆에 있는 미니모리 목장에서 생산되는 소고기, 통칭 미니모리규를 사용해!");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "다행히 미니모리 목장은 우리 미니모리 평야의 바로 옆이어서 미니모리규의 유통이 잘 되고 있지!");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그래서 양파와 마찬가지로 미니모리규 우삼겹도 도토리 센터에 팔고 있으니 크게 어렵지는 않을 거야!");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "응.. 알았어..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "미니모리 평야의 도토리 센터는 여기에서 쭉 위로 올라가면 나와!", Yuri);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "린이 많이 기다리고 있을 테니 슬슬 출발해 보자!");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(길을 따라 북쪽으로 가면 도토리 센터 미니모리 평야 점이 있다. 유리와 같이 센터에 들어가자.)");
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 스토리 블록 및 퀘스트 설정
		//                StartStoryBlock(0);
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//            case 3:
		//                // 스토리 블록 및 퀘스트 설정
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//        }
		//    }
		//}
		//else if (_mapName == "KokoroRestaurant") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["KokoroRestaurant"]) {
		//            case 1:
		//                Inst.mStoryBlocks = new StoryBlock[2];
		//                Inst.mBlockNotFromObject = new bool[2];
		//                // 시작 : 타유가 식당에 들어간다
		//                Inst.MakeStoryBlock(0, 8);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(코코로 식당에 도착한 타유, 문을 열자 종소리가 맑게 울린다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(딸랑~)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "저기.. 아무도 없나요..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "어..? 점장님! 누가 왔는데요?", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "음.. 오늘부터 새로 일하기로 한 알바생이 도착했나 보구나.", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그러게요~ 제가 안으로 데려올게요!", Yuri);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "타유 왔어? 이 쪽으로 오면 돼!");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(유리 언니가 부르는 것 같다.. 유리 언니에게 가서 말을 걸자.)");
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 유리와의 대화
		//                Inst.MakeStoryBlock(1, 4);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "내가 뭘 해야 하는지 알려줘..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "서두르기는.. 일단 안쪽으로 들어와서 점장님께 인사 드리자!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "응.. 고마워..", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[1], "(유리와 함께 주방으로 들어가서 점장님께 말을 걸자. 바로 보이는 문이 주방 입구이다.)");
		//                // 스토리 블록과 퀘스트 설정
		//                StartStoryBlock(0);
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//            case 2:
		//                Inst.mStoryBlocks = new StoryBlock[1];
		//                Inst.mBlockNotFromObject = new bool[1];
		//                // 시작 : 린이 규동을 주문한다
		//                Inst.MakeStoryBlock(0, 26);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(홀에는 평소의 밝은 표정으로 식탁에 앉아서 주문하는 린이 있었다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "점장님~! 유리 언니~! 저 주문할게요~!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "기운 넘치기는.. 자, 여기 메뉴판! 오늘은 어떤 거 먹을 거야?", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "음~ 그러면 오늘은 가볍게 규동 세 그릇만 먹을게요~!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "알겠어! 조금만 기다리고 있어!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "고마워요 유리 언니~!", Rin);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(린에게 물을 가져다 주고 있던 타유는 유리와 린의 대화를 들으면서 깊은 생각에 잠겼다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "(오늘도 린은 많이 먹는구나..)", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(린은 어렸을 때부터 먹는 것을 참 좋아했지..)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(그런데 어떻게 저렇게 살이 안 찌는 걸까..?)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(나도 내가 좋아하는 돈부리를 많이 먹어도 살이 찌지 않았으면 좋겠는데..)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(비결.. 가르쳐 달라고 해 볼까..?)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "타유~ 무슨 생각을 그렇게 하고 있어~?", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "..! 아.. 아무것도 아니야..!", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그나저나 매번 느끼는 건데.. 린은 많이 먹는데도 살이 잘 안찌지..?");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그런가~? 나는 잘 모르겠는데~?", Rin);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "굳이 이유를 들어보자면 움직이는 양이 많아서 그런 거려나~?");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "린은 활발해서 이곳 저곳 돌아다니는 것을 좋아하니까 그럴 수도 있겠다..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그런데 그렇다 쳐도 린은 너무 많이 먹어..!");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "음~ 그러면 앞으로 더 많이 돌아다니도록 할게~!", Rin);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(그렇게 말하면서 함박웃음을 짓고 규동이 나오기를 기다리는 린.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(그런 린을 보며 분명 저런 밝은 에너지도 체형 관리에 도움이 되었을 것이라고 생각하는 타유였다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(그러던 중 주방에서 유리 언니의 목소리가 들렸다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "타유! 잠시만 주방으로 좀 와볼래? 점장님께서 부르셔!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "금방 갈게..!", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(점장님께서 부르신다. 다시 주방으로 들어가 점장님께 말을 걸자.)");
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 스토리 블록과 퀘스트 설정
		//                StartStoryBlock(0);
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//            case 3:
		//                Inst.mStoryBlocks = new StoryBlock[1];
		//                Inst.mBlockNotFromObject = new bool[1];
		//                // 시작 : 린과의 대화
		//                Inst.MakeStoryBlock(0, 10);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(유리와 만나기 위해 나가려는 찰나 린이 말을 걸어온다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "타유~! 어디 가~?", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "아..! 규동 재료가 조금 모자라서 유리 언니랑 같이 사러 가는 길이야..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "손님으로 왔을 텐데.. 기다리게 해서 미안해..");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "아냐아냐~! 나 오늘은 별로 안 바쁘니까 괜찮아~", Rin);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "유리 언니랑 맛있는 거라도 사 먹으면서 천천히 다녀와~");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "에이.. 그래도 일하는 중인데 그렇게는 못 하지.. 마음만 받을게 린..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "밖에서 유리 언니가 기다리고 있어.. 그럼 슬슬 나가 볼게..!");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "응~ 잘 다녀와~", Rin);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(유리 언니가 많이 기다리고 있을 것이다.. 서둘러 미니모리 1번 도로로 나가자.)");
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 스토리 블록과 퀘스트 설정
		//                StartStoryBlock(0);
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
		//                Inst.mStoryBlocks = new StoryBlock[1];
		//                Inst.mBlockNotFromObject = new bool[1];
		//                // 점장과의 대화
		//                Inst.MakeStoryBlock(0, 38);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "안녕하세요..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "아, 자네가 타유구나. 이쪽에 앉으럄.", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "네..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "우선 내 소개를 하지. 나는 코코로 식당의 점장을 맡고 있어.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그리고 이미 친해서 알고 있겠지만 이쪽은 우리 식당의 요리사 유리 양이네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "유리 양은 반 년 전부터 우리 식당에서 일하고 있는데 처음에는 서툴렀지만 노력을 많이 해서 지금은 코코로 식당의 돈부리들을 착실하게 담당하고 있지.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "대단하네요..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "에이 그정도까지는 아니에요 점장님!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "흠흠.. 그래도 자네가 많이 성장한 것에는 틀림이 없으니 이럴 때는 순수하게 기뻐해도 된다네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그래, 자기소개는 이쯤 해 놓고.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "앞으로 타유 양 자네가 해야 할 일을 간단히 알려주도록 하지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "더 자세한 이야기는 앞으로 선배인 유리 양이 계속 알려줄 것이니 너무 걱정하지 않아도 된다네.");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(그렇게 말하며 자세를 고쳐 잡은 점장은 이야기를 계속했다)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "우선, 돈부리를 만들기 위한 재료를 구해와야 되네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "재료를 얻는 경로는 크게 세 가지가 있단다.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "우선 어떤 재료들은 비교적 구하기 쉬워서 단순히 상점에서 사 올 수 있지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "별 다를 거 없이 “골드”를 주고 재료를 받아오면 된다네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "알겠습니다..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그리고 상점에서 구할 수 있는 대부분의 재료들은 “도토리 센터”에서 팔고 있어!", Yuri);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "너도 알다시피 도토리 센터는 각 지역마다 특색이 있지!");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그 말대로네. 예를 들자면 쌀과 밥으로 유명한 우리 미니모리 평야의 도토리 센터는 쌀이나 즉석 밥, 등이 특화되어 있다는 점이 있지.", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "“도토리 센터”.. 앞으로 평소보다 더 자주 가게 되겠군요..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그리고 어떤 재료들은 상점의 재료보다 조금 특별해서 다른 사람들과 대화를 하거나 부탁을 들어 줌으로서 그 보상으로 받을 수 있단다.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "다른 사람들과의 그런 교류를 “재료 퀘스트”라고 부르니 잘 알고 있도록 하게.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "“재료 퀘스트”.. 알겠습니다..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "마지막으로 어떤 재료들은 희귀해서 자네가 유리와 함께 코코로 식당 밖으로 나가서 구해와야 하네.", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "…네..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "마음 같아서는 내가 직접 구해오고 싶지만 나는 여기 계속 남아서 식당의 운영 관리를 해야 해서 말이지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "분명 자네들에게도 좋은 경험이 될 거네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "알겠습니다.. 유리 언니와 함께 하면 재미있을 거라고 생각해요..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "맞아! 나중에 린도 같이 데려가자! 나쁘지 않을 거야!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "후후.. 즐거워 보이는구나..", Master);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(그렇게 말하는 점장의 눈동자에는 왠지 과거의 회상이 담겨 있는 것 같아 보였다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(그러던 중 밖에서 누군가의 명랑한 목소리가 주방으로 흘러들어왔다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "안녕하세요~! 밥 먹으러 왔어요~!", Rin);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "손님이 온 것 같구나.. 그러면 이야기는 이쯤 해 두고 다시 장사를 하러 가 보세. 지금 말한 것들은 앞으로 유리 양의 도움을 받으면서 천천히 익혀 가면 되니 너무 걱정하지 말게.", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "설명 감사합니다.. 열심히 할게요..!", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(손님이 온 것 같다. 주방 밖으로 나가 주문의 과정이 어떻게 되는 지 지겨보자.)");
		//                // 스토리 블록과 퀘스트 설정
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//            case 2:
		//                Inst.mStoryBlocks = new StoryBlock[1];
		//                Inst.mBlockNotFromObject = new bool[1];
		//                // 점장과의 대화
		//                Inst.MakeStoryBlock(0, 17);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "타유 양, 잠시 이리로 와 보겠나?", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "네.. 무슨 일이신가요..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "방금 린 양이 규동 세 그릇을 주문했는데 냉장고를 보니 재료가 부족한 것 같네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "자네가 유리 양과 함께 좀 구해와 줄 수 있겠는가?");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "네 알겠습니다..! 부족한 재료가 무엇인가요..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "규동을 만들 때 주재료로는 황금 다시, 양파와 우삼겹.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "그리고 조미료로는 쇼유, 노란 도토리 가루, 갈은 생강.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "토핑으로는 베니쇼가, 시치미가 필요하다네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "황금 다시와 도토리 가루에 대해서는 나중에 설명할 기회가 있을 것이니 바쁜 지금은 일단 넘어가지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "다행히 조미료들은 전부 충분하지만 주재료인 양파와 우삼겹이 조금 부족하니 부탁 좀 하겠네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "구해오는 방법은 유리 양에게 말해 두었으니 나중에 들으면 될 것이야.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "넵 알겠습니다.. 그럼 금방 같이 출발하도록 할게요..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "조심히 다녀오게나.. 그리고 고맙네..", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그래, 그러면 준비하고 나와! 식당 입구에서 기다리고 있을게!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "알겠어..!", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(유리는 그렇게 말하면서 가방을 메고 주방을 나섰다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(가방 안에 도토리.. 숟가락.. 얼추 준비를 마친 것 같으니 미니모리 1번 도로로 나가자..)");
		//                break;
		//            case 3:
		//                Inst.mStoryBlocks = new StoryBlock[4];
		//                Inst.mBlockNotFromObject = new bool[4];
		//                // 시작 : 점장과의 대화
		//                Inst.MakeStoryBlock(0, 18);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(양파와 미니모리규를 가지고 주방으로 돌아왔더니 점장님께서 규동을 만들기 위한 조리 기구들을 준비하고 계셨다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "다녀왔어요!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "다녀왔습니다..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "오, 왔구나. 어서 오게. 고생 많았네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "이제 재료도 있으니 서둘러 규동을 만들어 보도록 하지…");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "…라고 말하고 싶지만 먼저 아직 아무것도 모르는 타유 양 자네에게 규동 레시피의 기본을 알려주는 것이 순서인 것 같구만.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "하지만 지금 밖에는 린이..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그건 걱정하지 말게나. 타유 양과 유리 양이 장보기를 다녀오는 동안 내가 린 양에게 먼저 양해를 구해놓았네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "안 그런가 린 양?");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "네~! 타유, 난 괜찮으니까 천천히 점장님께 레시피를 배우도록 해~!", Rin);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "뭐든 처음에 잘 배워둬야 하는 법이라구~!");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(점장님의 물음 직후에 린 양의 밝고 청량한 목소리가 들려왔다.)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "어, 어.. 고마워..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "린도 참.. 규동 세 그릇이나 시킬 정도로 배가 고프면서 목소리는 참 크네.", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그런고로, 지금부터는 가장 기본적인 규동 레시피를 배워보도록 하지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "들을 준비가 되면 다시 나에게 말을 걸어주게.");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(레시피의 설명이 길어질 것 같다고 판단하신 건지 점장님께서는 설명 전에 잠시 간격을 두시려는 것 같다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(레시피를 배울 준비가 되면 다시 점장님께 가서 말을 걸도록 하자.)");
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 레시피 : 규동 기본(재료)
		//                Inst.MakeStoryBlock(1, 31);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그래, 가장 기본적인 형태의 규동을 배울 준비는 다 되었는가.", Master);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "네..! 기대가 많이 되요..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "허허, 의욕이 넘치는구나. 아주 좋네", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "아까 전에도 설명 했었지만 우선 다시 한 번 기본적인 규동에 들어가는 재료를 먼저 말해주지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "우리 코코로 식당에서 요리하는 규동에는 주재료로 황금 다시, 양파, 미니모리규 우삼겹.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "그리고 조미료로는 쇼유, 노랑 도토리 가루, 갈은 마늘, 갈은 생강.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "마지막으로 토핑으로는 베니쇼가, 시치미가 쓰이고 있다네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "저.. 아까부터 궁금했는데 황금 다시가 뭔가요..?", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "좋은 질문이야 타유 양. 하마터면 깜박하고 넘어갈 뻔했구만.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "황금 다시란 코코로 식당에서 직접 만들어 쓰는 가쓰오 육수를 말하네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "만드는 것이 그렇게 어렵지 않아서 육수가 필요한 여러 돈부리들에 쓰이고 있지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "타유 양이 규동 요리에 좀 익숙해지면 황금 다시를 만드는 방법에 대해서도 가르쳐 줄 생각이네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그렇군요.. 알겠습니다..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "황금 다시를 설명하는 김에 도토리에 관해서도 알려 주도록 할까.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "자네도 알다시피 델리델리 대륙에는 다양한 색깔의 도토리가 열리고 있네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "그리고 각각의 도토리는 색깔에 따라 독특한 향을 가지고 있어서 많은 지방의 요리에 향신료로써 자주 쓰이고 있지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "델리델리 대륙 내에서 가장 유명한 푸드 센터의 브랜드 이름이 도토리가 된 것도 그만큼 도토리가 많은 음식에 그 재료로써 들어가기 때문이네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그런 유래가 있는 줄은 몰랐어요..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그렇다네. 그렇게 다양한 도토리 중에 규동에 조미료로써 활용되는 것은 노란색의 도토리.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "전통적으로는 노란 도토리를 담가 만든 술의 형태로 사용하지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "달콤한 향기를 내는 노란 도토리로 담근 술을 넣은 요리에는 은은한 단 맛이 더해지네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "하지만 노란 도토리주를 담그는 데에는 그만큼 시간이 필요한 것이야.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "이는 곧 노란 도토리주의 가격이 높아지는 원인이 되지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "그래서 요즘은 기술의 도움을 받아 도토리를 갈은 후 특별한 가공을 거쳐 도토리 가루를 만들어 사용하는 경우가 늘어났네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "비록 도토리주에 비하면 그 향기의 질이 떨어지지만 보다 싼 값에 도토리주와 비슷한 향기를 낼 수 있지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "그래서 코코로 식당의 가장 기본적인 규동에는 노란 도토리 가루를 사용해 손님들에게 고급스러운 단맛을 내는 규동을 합리적인 가격에 팔 수 있도록 하고 있네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "넵.. 알겠습니다..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "흠.. 재료 설명은 이쯤 해 두고.. 다음으로는 본격적으로 기본적인 규동을 만드는 방법을 가르쳐 주도록 하겠네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "들을 준비가 되면 아까처럼 다시 나에게 말을 걸어주게.");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[1], "(황금 다시.. 노랑 도토리.. 요리에 관해 배우는 것은 언제나 즐거운 것 같다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[1], "(조리 방법을 들을 준비가 되면 다시 점장님께 가서 말을 걸도록 하자.)");
		//                // 레시피 : 규동 기본(조리법)
		//                Inst.MakeStoryBlock(2, 41);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[2], "그래 왔는가.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "이제 코코로 식당의 기본적인 규동을 요리하는 방법에 대해 알려주겠네.");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[2], "넵..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[2], "먼저 재료를 준비해야 하네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "아까 사온 양파와 미니모리규 우삼겹을 먹기 좋은 사이즈로 자르기만 하면 끝이라네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "주재료의 준비가 간단한 것이 규동의 특징이라고 할 수 있지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그렇군요.. 그래서 규동이 요리하는 데 그렇게 오래 걸리지 않는 것인가요..?", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그렇다네. 다음으로는 규동의 국물을 만드는 방법에 대한 설명이네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "우선 냄비에 황금 다시를 넣고.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "쇼유, 노랑 도토리 가루를 넣고 생강을 조금 갈아 넣으면 끝나지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그 후 양파를 넣고 먼저 끓인다네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "음.. 양파와 소고기를 같이 넣고 끓이지 않는 이유가 있나요..? 그 쪽이 좀 더 빠를 것 같은데..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "예리한 질문이군. 물론 양파와 소고기를 한꺼번에 끓이면 규동 자체는 더 빨리 완성될 수 있겠지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "하지만 그렇게 되면 소고기가 너무 많이 익어 질겨지게 된다네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그래서 소고기는 양파가 조금 투명해지고 난 뒤에 넣고 끓이는 것이 정석이네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그렇군요..! 생각보다 어려운 부분이 많은 것 같아요..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "허허, 걱정하지 말게나. 누구나 처음은 다 어려운 법이야..", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그렇게 소고기를 넣고 국물이 졸아들 때까지 끓이면 완성이네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "조미료의 자세한 분량, 끓이는 시간 등은 나중에 자네에게 줄 레시피북에 적혀 있으니 참고하도록.");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "알려주셔서 감사합니다..!", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "흠흠.. 그래도 설명만 들어서는 안되겠지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "기왕이니 이번 주문으로 들어온 규동은 타유 양이 만들어 보도록 할까?");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "…네..?", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "긴장할 것 없네. 옆에서 유리 양이 실수하지 않도록 도와 줄 것이니.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그래도 손님에게 내놓을 규동인데..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그것도 걱정하지 말게나, 이미 린 양에게 양해를 구해놓았지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "안 그런가 린 양?");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "걱정하지마 타유~ 점장님께서 대신 미니모리규를 많이 올려준다고 하셨어~!", Rin);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "응.. 고마워..!", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "허허, 린 양에게는 특별히 미니모리규를 2배로 얹어주도록 하지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "와아~! 점장님 최고~!", Rin);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "……", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "감사합니다..");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "흠흠.. 손님을 너무 오래 기다리게 할 수는 없는 노릇이니 서둘러 규동을 만들어보도록 하지.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "내 바로 옆에 있는 조리대가 보이는가?");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "네.. 그런데 조금 독특하게 생겼네요..? 납작하고 커다란.. 나무 상자..?", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "그렇다네. 저것은 우리 코코로 식당에서 사용하는 특수한 조리대, “쿠킹 박스”라고 하네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "이제부터 쿠킹 박스에 대해 간단하게 설명을 할 것이니 준비가 되면 다시 내게 말을 걸어 주게.");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[2], "(톱니바퀴가 달려 있고 신비한 분위기를 풍기는 나무 상자.. 앞면에는 여러 개의 서랍이 있다.. 기묘한 물건이다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "(개인적으로는 직접 이것 저것 만져 보고 싶지만 린이 밖에서 오래 기다리고 있으니 그럴 수는 없다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[2], "(다시 점장님께 말을 걸어 설명을 듣자.)");
		//                // 점장과의 대화
		//                Inst.MakeStoryBlock(3, 40);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "자, 그럼 쿠킹 박스에 대해 알려주도록 하겠네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "쿠킹 박스를 사용하면 기존의 방식대로 요리하는 것보다 편하고 빠르게 할 수 있다는 장점이 있지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "또한 보이는 크기와 다르게 가벼워서 주방 밖에서 캠핑을 할 때에도 유용하다네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "타유 양, 한 번 들어 보겠나?");
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[3], "(커다란 나무 상자를 들어 본다. 예상한 것과는 다르게 손쉽게 바닥에서 상자가 기울어지는 것이 느껴진다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "…가볍네요..! 이거라면 등에 메고 다녀도 괜찮을 것 같아요..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "허허.. 이 캠핑 조리 기능은 자네와 유리 양이 앞으로 자주 활용하게 될 것이야.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "…..재료를 구해오는 여정이 짧지만은 않을 수도 있다는 뜻이군요.. 알겠습니다..!", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "그 점에 있어서는 항상 감사하고 있네..", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "타유 양. 쿠킹 박스를 한 번 열어보겠나? 옆에 있는 톱니바퀴를 돌리면 된다네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "넵..", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[3], "(그렇게 대답하고 타유는 눈앞에 놓인 나무 상자의 톱니를 돌렸다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "(드르르륵..!...치익!..덜컹..!)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "(톱니바퀴를 돌리자 박스는 아주 조금씩 증기를 내뿜기 시작했고 그 뒤 박스의 뚜껑이 열리는 소리가 났다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "(그 후 뭔가 이것저것이 움직이면서 상자가 변하기 시작한다..)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "..열렸네요.. 이것은..?!", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[3], "(넓은 공간의 판.. 그 위에는 격자 무늬가 그려져 있고 몇 장의 마른 도토리 이파리가 들어 있다..)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "마치 보드 게임 판처럼 생겼네요..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "타유 양, 이 팻말을 한 번 봐 보겠나?", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "넵..", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[3], "(대답하며 타유는 점장님으로부터 나무 판 하나를 받았고 그것을 관찰하기 시작했다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "(거기에 적혀 있는 것은.. 미니모리규 우삼겹.. 그리고 우삼겹의 그림이 새겨져 있는 것이 보였다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "(또한 여러 가지 글자들이 나무 판에 새겨져 있었다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "(뒤집어 보니 우둘투둘한 홈들이 정교하게 새겨져 있는 것이 보인다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "이것은..", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "아마 지금 자네가 생각하고 있는 것이 맞을 것이네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "이것은 “우든 카드”라고 불리는 나무 팻말.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "쿠킹 박스의 사용자는 요리의 과정 하나 하나를 우든 카드를 통해 하게 되지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "또한 이 우든 카드의 사용이 요리에 기여하는 행동의 유일한 매개체이기도 하네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "덕분에 요리가 마치 카드 게임처럼 간단하게 느껴지는 것이 가능한 것이네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "신기하네요..!", Tayu);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "하지만 이런 나무 상자와 나무 판 만으로 정말 요리가 가능한 것인가요..?");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "허허허.. 놀랄 만도 하네.", Master);
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "이것이 바로 나무와 기계의 도시, 아이언트리 기술의 결정체..!");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "금방 자네가 직접 경험하게 될 것이네.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "흠흠.. 설명하는 시간이 너무 길어진 것 같군.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "아무리 린 양에게 양해를 구했을지라도 이 이상 기다리게 하는 것은 손님에게 실례겠지.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "타유 양, 이제부터 유리 양의 도움을 받아 규동 세 그릇을 만들어 보게.");
		//                Inst.MakeChain(Inst.mStoryBlocks[3], "넵.. 열심히 해 볼게요..!", Tayu);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[3], "(쿠킹 박스는 점장님 바로 옆에 있다. 쿠킹 박스 근처에서 ‘C’를 누르면 요리를 시작할 수 있으니 유리 언니의 도움을 받아 규동 세 그릇을 만들어 보도록 하자.)");
		//                // 스토리 블록과 퀘스트 설정
		//                StartStoryBlock(0);
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//        }
		//    }
		//}
		//else if (_mapName == "DotoriMinimoriPlane") {
		//    if (MainGameMngScript.StorySelectingIndex == 1) {
		//        switch (GameMngScript.VisitedMapName["DotoriMinimoriPlane"]) {
		//            case 1:
		//                Inst.mStoryBlocks = new StoryBlock[2];
		//                Inst.mBlockNotFromObject = new bool[2];
		//                // 시작 : 미나미, 유리와의 대화
		//                Inst.MakeStoryBlock(0, 20);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(도토리 센터 미니모리 평야 점)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(타유와 유리가 들어오자 입구에 걸어 놓은 도토리로 만든 종이 맑은 소리를 내며 울린다)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(도통통통~!)");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "어서오세요~", Minami);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "안녕 미나미! 지금쯤이면 미나미가 있을 거라고 생각하던 참이었어!", Yuri);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "안녕하세요..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "어머, 누군가 했더니 타유였군요~", Minami);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "오늘부터 코코로 식당에서 아르바이트를 시작한다고 들었어요~");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "맞아요.. 처음에는 긴장했지만 유리 언니랑 점장님께서 많이 도와주셔서 지금은 잘 할 수 있을 것 같아요..!", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "적응했다니 다행이에요~! 타유 정도면 앞으로도 점점 더 좋아질 거에요~", Minami);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "감사합니다..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그나저나 두 분은 오늘 무슨 일로 오신 건가요~?", Minami);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "아아, 규동 재료로 쓸 양파하고 우삼겹이 좀 부족해서 말이지.", Yuri);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "타유에게 일도 가르칠 겸 같이 데리고 나온 거야!");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "그렇군요~ 양파랑 우삼겹이면 지금 센터에 많이 있어요~", Minami);
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "계산대에 올려 놓을 테니 이 쪽으로 와서 계산만 해주시면 되요~");
		//                Inst.MakeInitChain(Inst.mStoryBlocks[0], "오케이~", Yuri);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[0], "(계산대로 가서 물건을 계산하자. 계산대 앞에서 ‘C’를 누르면 상점 창을 열 수 있다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(상점 창에서 물건의 가격이 써져 있는 버튼을 누르면 물건을 구매 할 수 있다.)");
		//                Inst.MakeChain(Inst.mStoryBlocks[0], "(구매를 완료했다면 ‘ESC’를 눌러 상점 창을 닫도록 하자.)");
		//                Inst.mBlockNotFromObject[0] = true;
		//                // 퀘스트 완료 : 유리와의 대화
		//                Inst.MakeStoryBlock(1, 3);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "이거로 부족한 재료는 전부 다 산 거 같네요..", Tayu);
		//                Inst.MakeInitChain(Inst.mStoryBlocks[1], "그래, 이제 다시 식당으로 돌아가자! 린이 많이 기다리고 있을 거야.", Yuri);
		//                Inst.MakeNarationChain(Inst.mStoryBlocks[1], "(재료를 전부 샀으니 다시 코코로 식당 주방으로 돌아가자.)");
		//                Inst.mBlockNotFromObject[1] = true;
		//                // 스토리 블록 및 스토리 설정
		//                StartStoryBlock(0);
		//                QuestMngScript.SetDoingQuest(0);
		//                break;
		//        }
		//    }
		//}
		#endregion
	}

	static public void StartStoryBlock(int _index) {
		////
		PlayerScript.Interact = true;
		////
		BlockIndex = _index;
		Active = true;
	}

	IEnumerator FlipUp() {
		Inst.mCanvas.gameObject.SetActive(true);
		Inst.mPanel.gameObject.SetActive(true);
		if (Inst.mStoryBlocks[Inst.mBlockIndex].StoryChains[0].sysKey.key != "No Text")
			Inst.mCanvas.DOFade(1, Utils.storyBlockFlipTime);
		if (!Inst.mNPC_panel.sprite)
			Inst.mCanvas_NPC.DOFade(0, 0);
		if (Inst.mPanel.sprite)
			Inst.mPanel.DOFade(1, Utils.storyBlockFlipTime);

		yield break;
	}

	IEnumerator FlipDown() {
		Inst.mCanvas.DOFade(0, Utils.storyBlockFlipTime);
		Inst.mPanel.DOFade(0, Utils.storyBlockFlipTime);

		yield return new WaitForSeconds(Utils.storyBlockFlipTime);

		DOTween.KillAll();

		Inst.mCanvas.gameObject.SetActive(false);
		Inst.mPanel.gameObject.SetActive(false);
	}

	IEnumerator Flip() {
		//mBlockInnerIndex++;

		//if (mBlockInnerIndex == mStoryBlocks[mBlockIndex].Length) {
		//	////
		//	PlayerScript.Interact = false;
		//	////
		//	mBlockInnerIndex = 0;
		//	Active = false;

		//	yield break;
		//}

		//mFlipling = true;

		//SysKey nextKey = mStoryBlocks[mBlockIndex].StoryChains[mBlockInnerIndex].sysKey;
		//string nextText = mStoryBlocks[mBlockIndex].StoryChains[mBlockInnerIndex].text;
		//StoryNPC nextNPC = mStoryBlocks[mBlockIndex].StoryChains[mBlockInnerIndex].NPC;

		//SysKeySameness sameness = new SysKeySameness();
		//if (mPastSysKey.key != nextKey.key)
		//	sameness.sameKey = false;
		//if (mPastSysKey.effectKey != nextKey.effectKey)
		//	sameness.sameEffectKey = false;
		//if (mPastSysKey.panelSpriteNum != nextKey.panelSpriteNum)
		//	sameness.samePanelSpriteNum = false;
		//if (mPastSysKey.BGM_Num != nextKey.BGM_Num)
		//	sameness.same_BGM_Num = false;
		//if (mPastSysKey.BGM_SubNum != nextKey.BGM_SubNum)
		//	sameness.same_BGM_SubNum = false;
		//if (mPastSysKey.effectNum != nextKey.effectNum)
		//	sameness.sameEffectNum = false;

		//mText.DOFade(0, Utils.storyBlockFlipTime);
		//if (nextKey == "Naration" && !sameness.sameKey) {
		//	mNPC_name.DOFade(0, Utils.storyBlockFlipTime);
		//	mNPC_panel.DOFade(0, Utils.storyBlockFlipTime);
		//}
		//else if (nextKey.key == "No Text" && !sameness.sameKey)
		//	mCanvas.DOFade(0, Utils.storyBlockFlipTime);
		//else if (nextNPC.name != null) {
		//	mNPC_name.DOFade(0, Utils.storyBlockFlipTime);
		//	mCanvas_NPC.DOFade(0, Utils.storyBlockFlipTime);
		//}

		//if (nextKey.fade)
		//	mFade.DOFade(1, Utils.storyBlockFadeTime);
		//if (!sameness.samePanelSpriteNum && nextKey.panelSpriteNum != -1)
		//	mPanel.DOFade(0, Utils.storyBlockFlipTime);

		//yield return new WaitForSeconds(Utils.storyBlockFlipTime);

		//mText.text = nextText;
		//mText.DOFade(1, Utils.storyBlockFlipTime);
		//if (nextNPC.name != null) {
		//	mNPC_name.text = nextNPC.name;
		//	mNPC_name.DOFade(1, Utils.storyBlockFlipTime);
		//	mNPC_panel.sprite = nextNPC.sprite;
		//	if (mNPC_panel.sprite)
		//		mCanvas_NPC.DOFade(1, Utils.storyBlockFlipTime);
		//}

		//StartCoroutine(EffectKey(nextKey, sameness));

		//if (nextKey.key != "No Text" && !sameness.sameKey)
		//	mCanvas.DOFade(1, Utils.storyBlockFlipTime);
		//if (!sameness.samePanelSpriteNum && nextKey.panelSpriteNum != -1) {
		//	mPanel.sprite = mPanelSprites[nextKey.panelSpriteNum];
		//	mPanel.DOFade(1, Utils.storyBlockFlipTime);
		//}
		//if (nextKey.BGM_Num == -2 && !sameness.same_BGM_Num)
		//	SoundMngScript.PlaySound("Stop All BGM");
		//else if (nextKey.BGM_SubNum == -2 && !sameness.same_BGM_SubNum)
		//	SoundMngScript.PlaySound("Stop Sub BGM");
		//else if (!sameness.same_BGM_Num && nextKey.BGM_Num != -1)
		//	SoundMngScript.PlaySound("BGM", nextKey.BGM_Num, nextKey.BGM_SubNum);
		//else if (!sameness.same_BGM_SubNum && nextKey.BGM_SubNum != -1)
		//	SoundMngScript.PlaySound("Sub BGM", -1, nextKey.BGM_SubNum);
		//if (nextKey.effectNum != -1)
		//	SoundMngScript.PlaySound("Effect", nextKey.effectNum);
		//if (nextKey.fade)
		//	mFade.DOFade(0, Utils.storyBlockFadeTime);

		//yield return new WaitForSeconds(Utils.storyBlockFlipTime);

		//mFlipling = false;

		//StartCoroutine(EffectKey(nextKey, sameness));

		//mPastSysKey = nextKey;

		yield break;
	}

	IEnumerator EffectKey(SysKey _key, SysKeySameness _sameness) {
		//if (mFlipling) {
		//	if (_key.effectKey == "Shake" && !_sameness.sameEffectKey)
		//		mPanel.rectTransform.DOShakeAnchorPos(100.0f);
		//	else
		//		mPanel.rectTransform.DOKill();
		//}
		//else {
		//	if (_key.effectKey == "Double Fade") {
		//		if (!_key.fade)
		//			Debug.LogError("Double Fade 를 fade 옵션 없이 사용하면 안 돼요!");

		//		mFlipling = true;
		//		mFade.DOFade(1, Utils.storyBlockFadeTime);
		//		yield return new WaitForSeconds(Utils.storyBlockFlipTime);
		//		mFade.DOFade(0, Utils.storyBlockFadeTime);
		//		yield return new WaitForSeconds(Utils.storyBlockFlipTime);
		//		mFlipling = false;
		//	}
		//}

		yield break;
	}

	void MakeStoryBlock(int _blockIndex, int _length) {
		Inst.mStoryBlocks[_blockIndex] = new StoryBlock();
		Inst.mStoryBlocks[_blockIndex].Length = _length;
		Inst.mStoryBlocks[_blockIndex].StoryChains = new StoryChain[_length];
		Inst.mInitBlockIndex = 0;
	}

	void MakeChain(StoryBlock _block, SysKey _sysKey, string _string, StoryNPC _NPC = default) {
		StoryChain chain = default;
		chain.sysKey = _sysKey;
		chain.text = _string;
		chain.NPC = _NPC;
		_block.StoryChains[Inst.mInitBlockIndex] = chain;
		Inst.mInitBlockIndex++;
	}
}
