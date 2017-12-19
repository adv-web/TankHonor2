using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
//玩家账号详细信息面板控制器
public class UserMessagePanelController : MonoBehaviour {

    public GameObject mainPanel;            //游戏主面板
    public GameObject lobbyPanel;           //大厅面板
    public GameObject userMessagePanel;     //玩家账号详细信息面板

    public Button backButton;       //返回按钮

	public InputField userId;             //PlayFab账号ID
    public Text level;        		//账号当前等级（军衔）
    public Text expText;            //账号经验值
	public Text rank;

    //面板启用时调用，显示玩家账号信息
    void OnEnable () {
		//为返回按钮绑定响应函数
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate ()
        {
            lobbyPanel.SetActive(true);                 //如果玩家在游戏房间中，点击返回按钮后，游戏界面显示游戏房间。
            userMessagePanel.SetActive(false);              //如果玩家在游戏大厅中，点击返回按钮后，游戏界面显示游戏大厅。
        });

        userId.text = PlayFabUserData.playFabId;    

        //根据玩家当前等级，显示军衔和勋章
		level.text = GameInfo.levelRankNames[PlayFabUserData.lv - 1];
		rank.text = "10";
        if (PlayFabUserData.lv < GameInfo.levelExps.Length)
        {
            expText.text = PlayFabUserData.exp.ToString() + "/" + GameInfo.levelExps[PlayFabUserData.lv - 1].ToString();
        }
        else
        {
            expText.text = PlayFabUserData.exp.ToString()+"(已满级)";
        }
    }
		 
   //PlayFab请求发生错误时调用，在控制台输出错误原因
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);
    }
		

	//玩家昵称设置成功时调用
	void OnUpdateUserTitleDisplayName(UpdateUserTitleDisplayNameResult result){

		PurchaseItemRequest request = new PurchaseItemRequest()
		{
			CatalogVersion = PlayFabUserData.catalogVersion,
			VirtualCurrency = "FR",
			Price = 0,
			ItemId = "AK47"
		};
		/*
		*loginPanel.SetActive(false);                //禁用游戏登录面板
		*mainPanel.SetActive(true);                  //启用游戏主面板
		*/
	}
}
