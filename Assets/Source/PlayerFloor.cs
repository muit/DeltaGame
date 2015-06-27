using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PlayerFloor : MonoBehaviour {

    private List<Collider> colliders = new List<Collider>();
    private CPlayer player;
	void Awake () {
        player = transform.parent.GetComponent<CPlayer>();
	}
	
	void Update () {
        if (colliders.Count <= 0)
        {
            player.canJumpAgain = false;
        }
	}

    void OnTriggerEnter(Collider col) {
        colliders.Add(col);
        player.canJumpAgain = true;
    }

    void OnTriggerExit(Collider col){
        colliders.Remove(col);
    }
}
