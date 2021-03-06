﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;

//主面板控制器
public class TestMainPanelController : PunBehaviour {
	public GameObject LoginPanel;			//游戏登录面板
	public GameObject MainPanel;			//游戏主面板
	public GameObject ChoosePanel;
	public GameObject LobbyPanel;           //游戏大厅面板
	public GameObject RankPanel;     //玩家排行榜面板
	public GameObject RoomPanel;            //游戏房间面板
	public GameObject UserMessagePanel;     //玩家信息面板
//	public Text LvValue;                    //玩家简要信息 - 玩家等级
//	public Text UsernameText;               //玩家简要信息 - 玩家昵称

//	public GameObject DataLoadingWindow;    //“数据加载中”提示窗口


	int requestNum = 4;
	void OnEnable(){
		//显示鼠标（玩家退出战斗后，重新加载游戏场景）
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
//		DataLoadingWindow.SetActive (true);

		if (LobbyPanel != null)
			LobbyPanel.SetActive (false);
		if (RankPanel != null)
			RankPanel.SetActive(false);
		if (RoomPanel != null)
			RoomPanel.SetActive(false);
		if (UserMessagePanel != null)
			UserMessagePanel.SetActive(false);
		if (ChoosePanel != null)
			ChoosePanel.SetActive(false);
		//玩家登录后，需要同时向PlayFab发起4个请求
		requestNum = 4;

		//获取玩家数据Player Data
		GetUserDataRequest getUserDataRequest = new GetUserDataRequest ();
		PlayFabClientAPI.GetUserData (getUserDataRequest, OnGetUserData, OnPlayFabError);

		//获取玩家账户信息
		GetAccountInfoRequest getAccountInfoRequest = new GetAccountInfoRequest()
		{
			PlayFabId = PlayFabUserData.playFabId
		};
		PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, OnGetAccountInfo, OnPlayFabError);

		//获取游戏数据Title Data
		GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();
		PlayFabClientAPI.GetTitleData(getTitleDataRequest, OnGetTitleData, OnPlayFabError);

	}

	//玩家数据Player Data获取成功时调用
	void OnGetUserData(GetUserDataResult result){
		Debug.Log ("User Data Loaded");
		//在本地保存玩家数据
		PlayFabUserData.userData = result.Data;

		//成就系统相关数据保存
		if (result.Data.ContainsKey("AchievementPoints"))
			PlayFabUserData.achievementPoints = int.Parse(result.Data["AchievementPoints"].Value);
		else PlayFabUserData.achievementPoints = 0;
		if (result.Data.ContainsKey("LV"))
			PlayFabUserData.lv = int.Parse(result.Data["LV"].Value);
		else
			PlayFabUserData.lv = 1;
//		LvValue.text = PlayFabUserData.lv.ToString();
		if (result.Data.ContainsKey("Exp"))
			PlayFabUserData.exp = int.Parse(result.Data["Exp"].Value);
		else PlayFabUserData.exp = 0;

		//天赋系统相关数据保存
		if (result.Data.ContainsKey("ExpAndMoneySkillLV"))
			PlayFabUserData.expAndMoneySkillLV = int.Parse(result.Data["ExpAndMoneySkillLV"].Value);
		else PlayFabUserData.expAndMoneySkillLV = 0;
		if (result.Data.ContainsKey("ShootingRangeSkillLV"))
			PlayFabUserData.shootingRangeSkillLV = int.Parse(result.Data["ShootingRangeSkillLV"].Value);
		else PlayFabUserData.shootingRangeSkillLV = 0;
		if (result.Data.ContainsKey("ShootingIntervalSkillLV"))
			PlayFabUserData.shootingIntervalSkillLV = int.Parse(result.Data["ShootingIntervalSkillLV"].Value);
		else PlayFabUserData.shootingIntervalSkillLV = 0;
		if (result.Data.ContainsKey("ShootingDamageSkillLV"))
			PlayFabUserData.shootingDamageSkillLV = int.Parse(result.Data["ShootingDamageSkillLV"].Value);
		else PlayFabUserData.shootingDamageSkillLV = 0;

		//玩家战斗数据保存
		if (result.Data.ContainsKey ("TotalKill"))
			PlayFabUserData.totalKill = int.Parse (result.Data ["TotalKill"].Value);
		else
			PlayFabUserData.totalKill = 0;
		if (result.Data.ContainsKey ("TotalDeath"))
			PlayFabUserData.totalDeath = int.Parse (result.Data ["TotalDeath"].Value);
		else
			PlayFabUserData.totalDeath = 0;
		if (PlayFabUserData.totalDeath == 0)
			PlayFabUserData.killPerDeath = (float)PlayFabUserData.totalKill*100.0f;
		else
			PlayFabUserData.killPerDeath = PlayFabUserData.totalKill * 100.0f / PlayFabUserData.totalDeath;

		if (result.Data.ContainsKey ("TotalWin"))
			PlayFabUserData.totalWin = int.Parse (result.Data ["TotalWin"].Value);
		else
			PlayFabUserData.totalWin = 0;
		if (result.Data.ContainsKey ("TotalGame"))
			PlayFabUserData.totalGame = int.Parse (result.Data ["TotalGame"].Value);
		else
			PlayFabUserData.totalGame = 0;
		if (PlayFabUserData.totalGame == 0)
			PlayFabUserData.winPercentage = 0.0f;
		else
			PlayFabUserData.winPercentage = PlayFabUserData.totalWin * 100.0f / PlayFabUserData.totalGame;

		//玩家装备的道具
		if (result.Data.ContainsKey ("EquipedWeapon"))
			PlayFabUserData.equipedWeapon = result.Data["EquipedWeapon"].Value;
		else
			PlayFabUserData.equipedWeapon = "M4A3E2";

		//获取玩家的仓库数据
		GetUserInventoryRequest getUserInventoryRequest = new GetUserInventoryRequest();
		PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, OnGetUserInventory, OnPlayFabError);
	}

	//玩家仓库数据获取成功时调用
	void OnGetUserInventory(GetUserInventoryResult result){
		Debug.Log("User Inventory Loaded");
		if (result.VirtualCurrency.Count == 0) {
			OnMessageResponse();
			return;
		}
		//检测玩家是否拥有装备道具
		bool hasEquipedWeapon = false;
		foreach (ItemInstance i in result.Inventory) {
			if (i.ItemClass == PlayFabUserData.equipedWeapon) {
				hasEquipedWeapon = true;
				break;
			}
		}
		//如果玩家未拥有装备的道具（超出使用期限）
		if (!hasEquipedWeapon)
		{
			PlayFabUserData.equipedWeapon = "M4A3E2";

			//更新玩家属性Player Data“EquipedWeapon”
			UpdateUserDataRequest request = new UpdateUserDataRequest();
			request.Data = new Dictionary<string, string>();
			request.Data.Add("EquipedWeapon", PlayFabUserData.equipedWeapon);
			PlayFabClientAPI.UpdateUserData(request, OnUpdateUserData, OnPlayFabError);
		}
		else
		{
			OnMessageResponse();
			Debug.Log("User Data Saved");
		}

		OnMessageResponse();    //PlayFab的数据是否接收完毕

	}

	//玩家属性更新成功后调用
	void OnUpdateUserData(UpdateUserDataResult result)
	{
		Debug.Log("User Data Saved");
	}

	//游戏道具数据接收成功后调用
	void OnGetCatalogItems(GetCatalogItemsResult result){
		//在GameInfo中保存游戏道具信息
		GameInfo.catalogItems = result.Catalog;
		OnMessageResponse();    //PlayFab的数据是否接收完毕
	}

	//玩家账号信息接收成功后调用
	void OnGetAccountInfo(GetAccountInfoResult result)
	{
		//在PlayFabUserData中保存玩家邮箱信息
		PlayFabUserData.email = result.AccountInfo.PrivateInfo.Email;
		OnMessageResponse();    //PlayFab的数据是否接收完毕
	}

	//游戏数据接收成功后调用
	void OnGetTitleData(GetTitleDataResult result)
	{
		//在GameInfo中保存游戏数据
		GameInfo.titleData = result.Data;
		OnMessageResponse();    //PlayFab的数据是否接收完毕
	}

	//PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error)
	{
		Debug.LogError("Get an error:" + error.Error);
	}

	//PlayFab的数据是否接收完毕
	void OnMessageResponse()
	{
		requestNum--;
		if (requestNum == 0)        //PlayFab的数据已接收完毕，在游戏主面板显示游戏大厅以及其他信息
		{
//			DataLoadingWindow.SetActive(false);
			LobbyPanel.SetActive(true);
		}
	}

	//禁用所有面板
	public void disableAllPanel()
	{
		LobbyPanel.SetActive(false);
		RankPanel.SetActive(false);
		RoomPanel.SetActive(false);
		UserMessagePanel.SetActive(false);
		ChoosePanel.SetActive (false);
	}

	[UsedImplicitly]
	public void Clickvs5Panel() {
		PlayerPrefs.SetInt("maxPlayer", 10);
		disableAllPanel();
		ChoosePanel.SetActive(true);
	}

	[UsedImplicitly]
	public void Clickvs3Panel(){
		PlayerPrefs.SetInt("maxPlayer", 6);
		disableAllPanel();
		ChoosePanel.SetActive(true);
	}

	[UsedImplicitly]
	public void Clickvs1Panel(){
		PlayerPrefs.SetInt("maxPlayer", 2);
		disableAllPanel();
		ChoosePanel.SetActive(true);
	}

	//点击玩家信息，显示玩家信息面板
	public void ClickUserMessage()
	{
		disableAllPanel();
		UserMessagePanel.SetActive(true);
	}
}
