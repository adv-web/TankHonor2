using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
//登录面板控制器
public class LoginPanelController : MonoBehaviour {

	public GameObject loginAccountPanel;    //账号登录面板
	public GameObject registerPanel;        //注册面板
	public GameObject loginingWindow;       //“登录中”提示窗口

	public GameObject serverIPPanel;        //服务器IP设置面板

	public GameObject loginPanel;           //登录面板
	public GameObject mainPanel;            //游戏主面板

    //登录面板启用时调用，初始化面板的显示
	void OnEnable(){
		loginAccountPanel.SetActive (true);
		registerPanel.SetActive (false);
		loginingWindow.SetActive(false);
		if (serverIPPanel != null) {
			serverIPPanel.SetActive (false);
		}
	}

    
    //PlayFab请求出错时调用，在控制台输出错误信息
    void OnPlayFabError(PlayFabError error){
		Debug.LogError (error.ToString ());
		loginingWindow.SetActive(false);
	}
}
