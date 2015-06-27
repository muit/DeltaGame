using UnityEngine;
using System.Collections;

public class Coin : PickUp {

	protected override void Start () {
        base.Start();
	
	}
    
    void OnTriggerEnter(Collider col) {
        CPlayer player = col.GetComponentInParent<CPlayer>();
        if (player)
        {
            player.CollectMoney(this);
        }
    }

}
