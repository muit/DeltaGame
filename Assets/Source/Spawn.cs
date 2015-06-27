using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {
    public bool active = false;
    public float lifeTimeEnabled = 0.5f;
    public float lifeTimeDisabled = 0.25f;
    public bool persistent = false; 

    private ParticleSystem particles;

	void Awake () {
        particles = GetComponentInChildren<ParticleSystem>();
	}

    IEnumerator Start()
    {
        while (TNManager.isJoiningChannel) yield return null;
        if (active)
        {
            TNManager.Create(Game.Get().onlinePlayer, transform.position, transform.rotation, persistent);
        }
    }

    void OnTriggerEnter(Collider col){
        CPlayer player = col.GetComponent<CPlayer>();
        if (player && !active) {
            SetActiveSpawn();
        }
    }

    public void SetActiveSpawn(bool value = true) {
        if (value)
        {
            particles.startLifetime = lifeTimeEnabled;
            if (Game.Get().activeSpawn != this)
            {
                Game.Get().activeSpawn.SetActiveSpawn(false);
                Game.Get().activeSpawn = this;
            }
        } else {
            particles.startLifetime = lifeTimeDisabled;
        }
        active = value;
    }
}
