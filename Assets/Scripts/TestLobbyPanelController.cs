using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;

public class TestLobbyPanelController: PunBehaviour {

	public GameObject loginPanel;			//游戏登录面板
	public GameObject mainPanel;
	public GameObject lobbyPanel;
	public GameObject usrInfoPanel;
	public GameObject rankPanel;
	public GameObject chooseRoomPanel;
	public GameObject vsPanel;
	public GameObject vs5Panel;
	public GameObject vs3Panel;
	public GameObject vs1Panel;
	public Button usrInfoButton;
	public Button quitButton;
	public GameObject lobbyLoadingWindow;	//游戏大厅加载提示信息

	//当游戏大厅面板启用时调用，初始化信息
	void OnEnable(){
		rankPanel.SetActive (true);
		lobbyPanel.SetActive (true);
		chooseRoomPanel.SetActive (false);
		lobbyLoadingWindow.SetActive (false);
		if(!PhotonNetwork.insideLobby)
			lobbyLoadingWindow.SetActive (true);	//启用游戏大厅加载提示信息
		else lobbyLoadingWindow.SetActive (false);
	}

	public void QuitClick(){
		PhotonNetwork.Disconnect();					//断开客户端与Photon服务器的连接
		loginPanel.SetActive(true);					//启用游戏登录面板
		mainPanel.SetActive(false);					//禁用游戏大厅面板
		rankPanel.SetActive(false);
	}
		
	public void usrInfoButtonClick(){
		usrInfoPanel.SetActive(true);
	}

	public void vs5PanelClick(){
		chooseRoomPanel.SetActive(true);
		//set player num 5
		lobbyPanel.SetActive (false);
		rankPanel.SetActive (false);
	}

	public void vs3PanelClick(){
		chooseRoomPanel.SetActive(true);
		//set player num 3
		lobbyPanel.SetActive (false);
		rankPanel.SetActive (false);
	}

	public void vs1PanelClick(){
		chooseRoomPanel.SetActive(true);
		//set player num 1
		lobbyPanel.SetActive (false);
		rankPanel.SetActive (false);
	}

	/**覆写IPunCallback回调函数，当玩家进入游戏大厅时调用
	 * 禁用游戏大厅加载提示
	 */
	public override void OnJoinedLobby(){
		lobbyLoadingWindow.SetActive (false);
	}

}

