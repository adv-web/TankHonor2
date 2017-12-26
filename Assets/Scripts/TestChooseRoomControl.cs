using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;

public class TestChooseRoomControl : PunBehaviour {
	public Text title;
	public Button back;
	public Button createRoomButton;
	public Dropdown chooseMaps;
	public GameObject lobbyPanel;
	public GameObject chooseRoomPanel;
	public GameObject createRoomPanel;
	public GameObject roomLoadingWindow;		//游戏房间加载提示信息
	public GameObject lobbyLoadingWindow;	//游戏大厅加载提示信息
	public GameObject rankPanel;
	public GameObject roomMessagePanel;		//房间信息面板
	public GameObject roomPanel;			//游戏房间面板
	public GameObject previousButton;		//"上一页"按钮
	public GameObject nextButton;			//"下一页"按钮
	public Text pageMessage;				//房间页数文本控件
	private RoomInfo[] roomInfo;			//游戏大厅房间列表信息
	private int currentPageNumber;			//当前房间页
	private int maxPageNumber;				//最大房间页
	private int roomPerPage;			    //每页显示房间个数
	private GameObject[] roomMessage;		//游戏房间信息
	private int playerNum;


	string mapName;
	int mapIndex;
	List<string> mapKeys;
	ExitGames.Client.Photon.Hashtable customRoomProperties;

	void OnEnable(){
		//获取房间信息面板
		RectTransform rectTransform = roomMessagePanel.GetComponent<RectTransform> ();
		roomPerPage = rectTransform.childCount;		//获取房间信息面板的条目数
		//初始化每条房间信息条目
		roomMessage = new GameObject[roomPerPage];	
		for (int i = 0; i < roomPerPage; i++) {
			roomMessage [i] = rectTransform.GetChild (i).gameObject;
			roomMessage [i].SetActive (false);			//禁用房间信息条目
		}

		currentPageNumber = 1;					//初始化当前房间页
		//maxPageNumber = 1;				    //初始化最大房间页	
		//pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();	//更新房间页数信息的显示
		OnReceivedRoomListUpdate();

		chooseRoomPanel.SetActive (true);
		roomLoadingWindow.SetActive (false);		//禁用游戏房间加载提示信息
		playerNum = PlayerPrefs.GetInt("maxPlayer");
		int halfNum = (int)(playerNum / 2);
		title.text = halfNum.ToString () + "V"+halfNum.ToString ()+"对战模式";	//更新房间页数信息的显示
		//if(PhotonNetwork.connectionStateDetailed != PeerState.JoinedLobby)
		if (createRoomPanel != null)
			createRoomPanel.SetActive (false);  //禁用创建房间面板
		if (roomPanel != null)
			roomPanel.SetActive(false);         //禁用游戏房间面板
		back.onClick.RemoveAllListeners ();		//移除返回按钮绑定的所有监听事件
		back.onClick.AddListener (delegate() {	//为返回按钮绑定新的监听事件
			rankPanel.SetActive(true);
			lobbyPanel.SetActive(true);//启用游戏登录面板
			chooseRoomPanel.SetActive(false);					//禁用游戏大厅面板
		});
			
		//复选框

		//显示游戏房间的地图
		mapName = PhotonNetwork.room.customProperties["MapName"].ToString();
		photonView.RPC("UpdateMap", PhotonTargets.All, mapName);
		mapKeys = new List<string>(GameInfo.maps.Keys);
		int length = mapKeys.Count;
		for(int i = 0; i < length; i++)
		{
			if(mapKeys[i] == mapName)
			{
				mapIndex = i;
				break;
			}
		}

	}

	/**覆写IPunCallback回调函数，当玩家进入游戏大厅时调用
	 * 禁用游戏大厅加载提示
	 */
	public override void OnJoinedLobby(){
		lobbyLoadingWindow.SetActive (false);
	}

	/**覆写IPunCallback回调函数，当玩家进入游戏房间时调用
	 * 禁用游戏大厅面板，启用游戏房间面板
	 */
	public override void OnJoinedRoom(){
		gameObject.SetActive (false);
		roomPanel.SetActive (true);
	}


	/**覆写IPunCallback回调函数，当房间列表更新时调用
	 * 更新游戏大厅中房间列表的显示
	 */
	public override void OnReceivedRoomListUpdate(){
		//获取游戏大厅中的房间列表
		roomInfo = PhotonNetwork.GetRoomList().Where(info => (int) info.customProperties["MaxPlayer"] == PlayerPrefs.GetInt("maxPlayer")).ToArray();
		maxPageNumber = (roomInfo.Length - 1) / roomPerPage + 1;	//计算房间总页数
		if (currentPageNumber > maxPageNumber)		//如果当前页大于房间总页数时
			currentPageNumber = maxPageNumber;		//将当前房间页设为房间总页数
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();	//更新房间页数信息的显示
		ButtonControl ();		//翻页按钮控制
		ShowRoomMessage ();		//显示房间信息

	}

	//显示房间信息
	void ShowRoomMessage(){
		int start, end, i, j;
		start = (currentPageNumber - 1) * roomPerPage;			//计算需要显示房间信息的起始序号
		if (currentPageNumber * roomPerPage < roomInfo.Length)	//计算需要显示房间信息的末尾序号
			end = currentPageNumber * roomPerPage;
		else
			end = roomInfo.Length;
		//依次显示每条房间信息
		for (i = start,j = 0; i < end; i++,j++) {
			RectTransform rectTransform = roomMessage [j].GetComponent<RectTransform> ();
			string roomName = roomInfo [i].name;	//获取房间名称
			Text[] texts = roomMessage[j].GetComponentsInChildren<Text>();
			texts[0].text = (i + 1).ToString();
			texts[1].text = roomName;
			texts[2].text = roomInfo[i].playerCount + "/" + roomInfo[i].maxPlayers;

			Button button = rectTransform.GetComponent<Button>();
			//Button button = rectTransform.GetChild (3).GetComponent<Button> ();				//获取"进入房间"按钮组件
			//如果游戏房间人数已满，或者游戏房间的Open属性为false（房间内游戏已开始），表示房间无法加入，禁用"进入房间"按钮
			if (roomInfo[i].playerCount == roomInfo[i].maxPlayers || roomInfo[i].open == false)
				button.interactable = false;
			//如果房间可以加入，启用"进入房间"按钮，给按钮绑定新的监听事件，加入此房间
			else
			{
				button.interactable = true;
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(delegate ()
					{
						ClickJoinRoomButton(roomName);
					});
			}
			roomMessage [j].SetActive (true);	//启用房间信息条目
		}
		//禁用不显示的房间信息条目
		while (j < roomPerPage) {
			roomMessage [j++].SetActive (false);
		}
	}

	//翻页按钮控制函数
	void ButtonControl(){
		//如果当前页为1，禁用"上一页"按钮；否则，启用"上一页"按钮
		if (currentPageNumber == 1)
			previousButton.SetActive (false);
		else
			previousButton.SetActive (true);
		//如果当前页等于房间总页数，禁用"下一页"按钮；否则，启用"下一页"按钮
		if (currentPageNumber == maxPageNumber)
			nextButton.SetActive (false);
		else
			nextButton.SetActive (true);
	}

	//"创建房间"按钮事件处理函数，启用创建房间面板
	public void ClickCreateRoomButton(){
		createRoomPanel.SetActive (true);
	}

	//"上一页"按钮事件处理函数
	public void ClickPreviousButton(){
		if(currentPageNumber !=1){
			currentPageNumber--;		//当前房间页减一
			pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();	//更新房间页数显示
		}
		ButtonControl ();			//当前房间页更新，调动翻页控制函数
		ShowRoomMessage ();			//当前房间页更新，重新显示房间信息
	}
	//"下一页"按钮事件处理函数
	public void ClickNextButton(){
		if(currentPageNumber!=maxPageNumber){
			currentPageNumber++;		//当前房间页加一
			pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();	//更新房间页数显示
		}
		ButtonControl ();			//当前房间页更新，调动翻页控制函数
		ShowRoomMessage ();			//当前房间页更新，重新显示房间信息
	}

	//"进入房间"按钮事件处理函数
	public void ClickJoinRoomButton(string roomName){
		PhotonNetwork.JoinRoom(roomName);	//根据房间名加入游戏房间
	}

	//上一张地图
	public void ClickMapLeftButton()
	{
		int length = mapKeys.Count;
		mapIndex--;
		if (mapIndex < 0) mapIndex = length - 1;
		mapName = mapKeys[mapIndex];
		customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "MapName", mapName } };
		PhotonNetwork.room.SetCustomProperties(customProperties);
		photonView.RPC("UpdateMap", PhotonTargets.All, mapName);
	}
	//下一张地图
	public void ClickMapRightButton()
	{
		int length = mapKeys.Count;
		mapIndex++;
		if (mapIndex >= length) mapIndex = 0;
		mapName = mapKeys[mapIndex];
		customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "MapName", mapName } };
		PhotonNetwork.room.SetCustomProperties(customRoomProperties);
		photonView.RPC("UpdateMap", PhotonTargets.All, mapName);
	}
}
