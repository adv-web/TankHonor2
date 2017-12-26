using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon;

public class TestCreateRoomPanelController : PunBehaviour {
	public GameObject createRoomPanel;
	public GameObject chooseRoomPanel;
	public GameObject roomPanel;
	public GameObject roomLoadingWindow;		//禁用游戏房间加载提示信息房 间 进 入 中 ...
	public Button backButton;
	public Text roomName;					//房间名称文本
	public Text roomNameHint;				//房间名称提示文本
	public Button startButton;
	public GameObject LoadingPanel;
	public Image mapImage; 
	public GameObject mapButtons;


	string mapName;
	int mapIndex;
	List<string> mapKeys;
	private ExitGames.Client.Photon.Hashtable customProperty;
	private int playerNum;


	void OnEnable () {
		roomNameHint.text = "";	//清空房间名称提示文本

		mapKeys = new List<string>(GameInfo.maps.Keys);
		mapIndex = 0;
		mapName = mapKeys[mapIndex];
	}

	public void startButtonClick(){
		RoomOptions roomOptions=new RoomOptions();
		roomOptions.MaxPlayers = 10;
		//更新游戏房间的地图
		roomNameHint.text = mapName;
		customProperty = new ExitGames.Client.Photon.Hashtable(){
			{"MapName", mapName},
			{"MaxPlayer", PlayerPrefs.GetInt("maxPlayer")}
		};
		roomOptions.CustomRoomProperties = customProperty;
		roomOptions.CustomRoomPropertiesForLobby = new[] {"MapName", "MaxPlayer"};
		RoomInfo[] roomInfos = PhotonNetwork.GetRoomList();	//获取游戏大厅内所有游戏房间
		bool isRoomNameRepeat = false;
		//遍历游戏房间，检查新创建的房间名是否与已有房间重复
		/*foreach (RoomInfo info in roomInfos) {
			if (roomName.text == info.name) {
				isRoomNameRepeat = true;
				break;
			}
		}*/
		//如果房间名称重复，房间名称提示文本显示"房间名称重复！"
		PhotonNetwork.CreateRoom (roomName.text, roomOptions, TypedLobby.Default);	//在默认游戏大厅中创建游戏房间
		roomLoadingWindow.SetActive (true);	//启用游戏房间加载提示信息
		createRoomPanel.SetActive (false);
	}

	public void closeButtonClick(){
		//chooseRoomPanel.SetActive (true);				
		createRoomPanel.SetActive (false);					//禁用游戏创建房间面板
	}

	//RPC函数，更新游戏房间地图的显示
	[PunRPC]
	public void UpdateMap(string name){
		mapImage.sprite = GameInfo.maps[name];
	}

	//显示或关闭地图切换按钮
	void MapButtonsControl(){
		mapButtons.SetActive (true);
	}
	///上一张地图
	public void ClickMapLeftButton()
	{
		int length = mapKeys.Count;
		mapIndex--;
		if (mapIndex < 0) mapIndex = length - 1;
		mapName = mapKeys[mapIndex];
		mapImage.sprite = GameInfo.maps[mapName];
	}
	//下一张地图
	public void ClickMapRightButton()
	{
		int length = mapKeys.Count;
		mapIndex++;
		if (mapIndex >= length) mapIndex = 0;
		mapName = mapKeys[mapIndex];
		mapImage.sprite = GameInfo.maps[mapName];
	}

}
