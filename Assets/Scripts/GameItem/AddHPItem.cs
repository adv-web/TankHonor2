using UnityEngine;
using System.Collections;

public class AddHPItem : MonoBehaviour {

	public int HpToAdd = 20;

	private void OnCollisionEnter(Collision other) {
		if (GameManager.gm.localPlayer.GetComponent<Collider>() != other.collider) return;
		GameManager.gm.tankHealth.requestAddHP(HpToAdd);
		PhotonNetwork.Destroy(gameObject);
	}

}
