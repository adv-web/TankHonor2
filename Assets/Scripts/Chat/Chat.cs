using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;

public class Chat : MonoBehaviour {

	private const int TYPE_NORMAL = 0, TYPE_DANMU = 1;
	private const float DANMU_YPOS_MIN = 290f, DANMU_YPOS_MAX = 530f;
	private const float DANMU_XPOS_START = 1216f, DAMNU_XPOS_END = -148.5f;	// 懒得做复杂，别改 Chat 物体的位置

	/// <summary>
	/// 弹幕移动速度
	/// </summary>
	public float DanmuSpeed;
	
	public GameObject DanmuPrefab;
	
	public Text TextRegion;
	public InputField InputField;
	private TankMove tankMove;
	private PhotonView photonView;
	
	private readonly List<GameObject> danmuList = new List<GameObject>();

	private void Start() {
		photonView = GetComponent<PhotonView>();
	}

	// Update is called once per frame
	private void Update () {
		checkInput();
		checkDanmu();
	}

	/// <summary>
	/// 检测玩家按键是否进入或退出打字模式
	/// </summary>
	private void checkInput() {
		if (Input.GetKeyUp(KeyCode.Return) && !InputField.isFocused) {
			if (tankMove == null) {	// 初始化比 TankMove 早，所以要用到的时候lazyload 一把。出错无所谓，就忽略了
				tankMove = GameManager.gm.localPlayer.GetComponent<TankMove>();
			}
			tankMove.enabled = false;
			InputField.ActivateInputField();	
		} else if (Input.GetKeyUp(KeyCode.Escape)) {
			tankMove.enabled = true;
		}
	}

	/// <summary>
	/// 处理每帧弹幕移动
	/// </summary>
	private void checkDanmu() {
		for (var i = danmuList.Count - 1; i >= 0; i--) {
			var danmu = danmuList[i];
			var rect = danmu.GetComponent<RectTransform>();
			rect.localPosition -= new Vector3(Time.deltaTime * DanmuSpeed, 0, 0);
			if (rect.localPosition.x < DAMNU_XPOS_END - danmu.GetComponent<Text>().preferredWidth) {
				danmuList.Remove(danmu);
				Destroy(danmu);
			}
		}
	}

	/// <summary>
	/// 玩家在输入框按回车的回调
	/// </summary>
	[UsedImplicitly]
	public void OnEditEnd() {
		var content = InputField.text;
		Debug.Log(content);
		if (content.Trim() == "") return;
		photonView.RPC("SendChatMsg", PhotonTargets.All, TYPE_DANMU, PhotonNetwork.playerName, content);
		InputField.DeactivateInputField();
		InputField.text = "";
	}

	/// <summary>
	/// RPC 接收发送消息的请求
	/// </summary>
	/// <param name="type">消息类型</param>
	/// <param name="name">发送者名字</param>
	/// <param name="content">消息内容</param>
	[PunRPC]
	[UsedImplicitly]
	private void SendChatMsg(int type, string name, string content) {
		if (type == TYPE_NORMAL) {
			SendNormalMsg(name, content);
		} else {
			SendDanmu(content);	
		}
	}

	private void SendNormalMsg(string name, string content) {
		TextRegion.text = name + "：" + content;
	}

	private void SendDanmu(string content) {
		// use obj pool later
		var obj = Instantiate(DanmuPrefab, transform) as GameObject;
		if (obj == null) {
			Debug.Log("instantiate danmu prefab got null!");
			return;
		}
		var rectTransform = obj.GetComponent<RectTransform>();
		var yPos = Random.Range(DANMU_YPOS_MIN, DANMU_YPOS_MAX);
		rectTransform.localPosition = new Vector3(DANMU_XPOS_START, yPos, transform.position.z);
		obj.GetComponent<Text>().text = content;
		danmuList.Add(obj);
	}
}
