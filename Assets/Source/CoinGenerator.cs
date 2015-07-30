using UnityEngine;
using System.Collections;

public class CoinGenerator : MonoBehaviour {
    private Scene scene;

    public float coinY = 0;
    public Coin coinPrefab;
    public int frequency = 2;
    public int renderDistance = 16;
    public float probability = 0.1f;
    public int minCoinStack = 1;
    public int maxCoinStack = 5;

    [Header("Noise Parameters")]
    public float amplitude = 16;
    public int multiplier = 8;
    public float persistence = 1 / 4;
    public int octaves = 2;

    SimplexNoiseGenerator simplex;
    float middleX = 0;
    Transform player;
    Transform lastCoin;
    int coinStackSize = 0;
    float lastZ;
    AudioSource sound;

	void Start ()
    {
        scene = Game.Get().scene;
        simplex = new SimplexNoiseGenerator(scene.seed);
        middleX = transform.position.x;
        sound = GetComponent<AudioSource>();

        Reset();
	}

    public void OnPlayerSpawned(CPlayer player)
    {
        this.player = player.transform;
    }

	void Update ()
    {
        if (!player)
            return;

        float z = Mathf.Floor(player.position.z) + renderDistance;

        if(z-lastZ > frequency) {
            if (coinStackSize > 0)
            {
                GenerateCoin(z);
                coinStackSize--;
            }
            else if (Random.Range(0f, 1f) < probability)
            {
                GenerateCoin(z);
                lastZ = z;
                coinStackSize = Random.Range(minCoinStack, maxCoinStack);
            }
        }
	}

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        lastZ = transform.position.z;
        lastCoin = null;
        coinStackSize = 0;
    }

    public void PlaySound() {
        sound.Play();
    }

    void GenerateCoin(float z)
    {
        float x = simplex.coherentNoise(0, 0, z, octaves, multiplier, amplitude, 2, persistence);
        GameObject coin = GameObject.Instantiate(coinPrefab.gameObject, new Vector3(middleX + x, coinY, z), Quaternion.identity) as GameObject;
        coin.transform.SetParent(this.transform);

        if (lastCoin)
        {
            Debug.DrawLine(lastCoin.position, coin.transform.position, Color.red, 10f, true);
        }
        lastCoin = coin.transform;
    }
}
