using UnityEngine;
using System.Collections;

public class Coin : PickUp {

	protected override void Start () {
        base.Start();
	
	}
    
    void OnTriggerEnter(Collider col) {
        CPlayer player = col.GetComponent<CPlayer>();
        if (player)
        {
            player.CollectMoney(this);
            Game.Get().coins.PlaySound();
            GameObject.Destroy(gameObject, 0.1f);
        }
    }

}
