using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class AddHPItem : MonoBehaviour {

	public int HpToAdd = 20;

	private PhotonView photonView;

	private void Start() {
		photonView = GetComponent<PhotonView>();
	}

	private void OnCollisionEnter(Collision other) {
		if (GameManager.gm.localPlayer.GetComponent<Collider>() != other.collider) return;
		GameManager.gm.tankHealth.requestAddHP(HpToAdd);
		photonView.RPC("DestroySelf", PhotonTargets.MasterClient);
	}

	[UsedImplicitly]
	[PunRPC]
	private void DestroySelf() {
		if (!PhotonNetwork.isMasterClient) return;
		PhotonNetwork.Destroy(gameObject);
	}

}
