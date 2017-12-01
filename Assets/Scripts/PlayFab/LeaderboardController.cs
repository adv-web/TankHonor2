using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
//玩家排行榜界面控制器
public class LeaderboardController : MonoBehaviour {

	public GameObject lobbyPanel;               //大厅面板
	public GameObject roomPanel;                //房间面板
	public GameObject leaderboardPanel;         //玩家排行榜面板
	public GameObject leaderboardLoadingLabel;  //玩家排行版加载提示
	public Button backButton;                   //返回按钮
	public Text currentPanel;                   //当前面板文本信息

	public Button TotalKillButton;              //“累计杀敌数”排行榜按钮

	public GameObject[] users;                  //前三玩家信息
	public GameObject localUser;                //本地玩家信息

	public Sprite metalCourage;                 //鼓励奖牌

	private Dictionary<string,uint> leaderboard = new Dictionary<string, uint> ();  //用于存储PlayFab获取的排行榜
	private string leaderboardType="";          //排行榜类型（累计杀敌数/杀敌死亡比/胜利场次/）
	private Text[] localUserTexts;              //本地玩家的数据显示
	private Image localUserImage;               //本地玩家的奖牌

	//玩家排行榜界面启用时调用，初始化排行榜界面
	void OnEnable () {
		currentPanel.text = "排 行";
		localUserTexts = localUser.GetComponentsInChildren<Text>();
		localUserTexts[1].text = PlayFabUserData.username;
		localUserImage = localUser.GetComponentsInChildren<Image>()[1];

		//默认显示玩家的累计杀敌数排行榜
		TotalKillButton.Select();
		ClickTotalKillButton ();

		backButton.onClick.RemoveAllListeners ();		//移除返回按钮绑定的所有监听事件
		backButton.onClick.AddListener (delegate() {    //为返回按钮绑定新的监听事件
			if (PhotonNetwork.inRoom)
				roomPanel.SetActive(true);              //如果玩家在游戏房间中，点击返回按钮后，游戏界面显示游戏房间。
			else 
				lobbyPanel.SetActive(true);             //如果玩家在游戏大厅中，点击返回按钮后，游戏界面显示游戏大厅。
			leaderboardPanel.SetActive(false);
		});
	}

	//“累计杀敌数”按钮的响应函数
	public void ClickTotalKillButton(){
		leaderboardLoadingLabel.SetActive (true);       //提示排行榜加载中
		foreach (GameObject go in users) {
			go.SetActive (false);
		}
		leaderboardType = "累计杀敌数";
		//填入本地玩家的信息（暂时不显示）
		localUser.SetActive(false);
		localUserTexts[0].text = "-";
		localUserTexts[2].text = leaderboardType + "：" + PlayFabUserData.totalKill;
		localUserImage.sprite = metalCourage;

		//向PlayFab发起获取游戏排行榜的请求（累计杀敌数）
		GetLeaderboardRequest request = new GetLeaderboardRequest () {
			MaxResultsCount = users.Length,     //获取排行榜中前MaxResultsCount个玩家信息（这里是3）
			StatisticName = "TotalKill"         //根据统计数据：累计杀敌数 生成排行榜
		};
		PlayFabClientAPI.GetLeaderboard (request, OnGetLeaderboard, OnPlayFabError);
	}

	//“杀敌死亡比”按钮的响应函数
	public void ClickKillPerDeathButton(){
		leaderboardLoadingLabel.SetActive (true);       //提示排行榜加载中
		foreach (GameObject go in users) {
			go.SetActive (false);
		}
		leaderboardType = "杀敌死亡比";
		//填入本地玩家的信息（暂时不显示）
		localUser.SetActive(false);
		localUserTexts[0].text = "-";
		localUserTexts[2].text = leaderboardType + "：" + (PlayFabUserData.killPerDeath / 100.0f).ToString("0.0");
		Debug.Log("watch it " + PlayFabUserData.killPerDeath);
		localUserImage.sprite = metalCourage;

		//向PlayFab发起获取游戏排行榜的请求（杀敌死亡比）
		GetLeaderboardRequest request = new GetLeaderboardRequest () {
			MaxResultsCount = users.Length,
			StatisticName = "KillPerDeath"      //根据统计数据：杀敌死亡比 生成排行榜
		};
		PlayFabClientAPI.GetLeaderboard (request, OnGetLeaderboard, OnPlayFabError);
	}

	//“胜利场次”按钮的响应函数
	public void ClickTotalWinButton(){
		leaderboardLoadingLabel.SetActive (true);       //提示排行榜加载中
		foreach (GameObject go in users) {
			go.SetActive (false);
		}
		leaderboardType = "胜利场数";
		//填入本地玩家的信息（暂时不显示）
		localUser.SetActive(false);
		localUserTexts[0].text = "-";
		localUserTexts[2].text = leaderboardType + "：" + (int)PlayFabUserData.totalWin;
		localUserImage.sprite = metalCourage;

		//向PlayFab发起获取游戏排行榜的请求（胜利场次）
		GetLeaderboardRequest request = new GetLeaderboardRequest () {
			MaxResultsCount = users.Length,
			StatisticName = "TotalWin"          //根据统计数据：胜利场次 生成排行榜
		};
		PlayFabClientAPI.GetLeaderboard (request, OnGetLeaderboard, OnPlayFabError);
	}

	//排行榜数据获取成功后调用此函数，在界面显示排行榜信息
	void OnGetLeaderboard(GetLeaderboardResult result){
		leaderboard.Clear ();   //清空排行榜数据
		//填入排行榜数据
		foreach (PlayerLeaderboardEntry entry in result.Leaderboard) {
			leaderboard.Add (entry.DisplayName, (uint)entry.StatValue);
		}
		SetLeadboard ();        //设置排行榜界面
	}

	//设置排行榜界面
	void SetLeadboard(){
		leaderboardLoadingLabel.SetActive (false);
		int i = 0;
		Text[] texts;
		//遍历排行榜信息
		foreach (KeyValuePair<string,uint>kvp in leaderboard) {
			texts = users [i].GetComponentsInChildren<Text> ();
			//填入排行榜的玩家信息
			texts [0].text = (i + 1).ToString();
			texts [1].text = kvp.Key;
			if (leaderboardType == "累计杀敌数" || leaderboardType == "胜利场数")
				texts [2].text = leaderboardType+"："+kvp.Value.ToString ();
			else if (leaderboardType == "杀敌死亡比")        //注：PlayFab的统计数据Statistics只能存储整数数据，在存储时放大了10000倍，这里需要将获取的值除以10000
				texts [2].text = leaderboardType + "：" + (kvp.Value / 10000.0f).ToString ("0.0");
			//如果玩家进入了前三名
			if (kvp.Key == PlayFabUserData.username)
			{
				localUserTexts[0].text = texts[0].text;
				localUserTexts[2].text = texts[2].text;
				localUserImage.sprite = users[i].GetComponentsInChildren<Image>()[1].sprite;    //将玩家的奖牌更新成前三名的奖牌
			}
			users [i].SetActive (true);
			i++;
		}
		localUser.SetActive(true);
	}

	//PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error){
		Debug.LogError ("Get an error:" + error.Error);
	}
}
