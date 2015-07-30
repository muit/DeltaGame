using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Scene : MonoBehaviour {
    public string seed = "";
    public float renderDistance = 10.0f;
    public int zOffset = 0;
    [Space(10)]
    public int distanceBetweenPrefabs = 8;
    public List<FloorPrefab> prefabs = new List<FloorPrefab>();

    [Header("Config")]
    public float speedIncrement;

    private Game game;
    private Vector3 lastPoint;
    private FloorPrefab lastPrefab;

    void Start() {

        game = Game.Get();

        if (Game.GetCache()) {
            seed = Game.GetCache().seed;
        }

        //If seed is empty generate one
        if (string.IsNullOrEmpty(seed)) {
            seed = Random.Range(10000, 99999).ToString();
        }

        Debug.Log("Seed: " + seed);

        string finalSeed = "";
        byte[] asciiBytes = Encoding.ASCII.GetBytes(seed);

        for (int i = 0; i < asciiBytes.Length; i++)
            finalSeed += asciiBytes[i].ToString();

        int loopCount = 0;
        while (true)
        {
            //Avoid possible freeze
            if (loopCount > 999) {
                Debug.LogError("Freeze detected.");
                break;
            }

            if (finalSeed.Length < 9) break;
            finalSeed = RemoveParCharacters(finalSeed);
            loopCount++;
        }

        Random.seed = int.Parse(finalSeed);


        lastPoint = transform.position;
        lastPoint.z += zOffset;

        if (game.controlledPlayer)
        {
            game.controlledPlayer.IncrementSpeed(speedIncrement * Time.deltaTime);
        }
	}
	
	void Update () {
        if ((Game.Get().GetTarget().position - lastPoint).sqrMagnitude < renderDistance * renderDistance)
        {
            GeneratePrefab(SelectRandomPrefab(), lastPoint);
            lastPoint.z += distanceBetweenPrefabs;
        }
	}

    GameObject GeneratePrefab(FloorPrefab prefab, Vector3 position) {
        if (prefab == null) Debug.LogError("Selected Floor is null.");

        GameObject generatedFloor = TNManager.Instantiate(prefab.prefab, position, Quaternion.identity) as GameObject;
        generatedFloor.transform.parent = this.transform;
        lastPrefab = prefab;
        return generatedFloor;
    }

    FloorPrefab SelectRandomPrefab()
    {
        //Limit depending on the different rules
        List<FloorPrefab> limitedPrefabs;
        if (lastPrefab != null)
        {
            float sqrDistance = (Vector3.zero - Game.Get().GetTarget().position).sqrMagnitude;

            limitedPrefabs = prefabs.FindAll(x => x.canRepeat || x.name != lastPrefab.name || x.startAt * x.startAt <= sqrDistance);
        }
        else
        {
            limitedPrefabs = prefabs;
        }

        int totalProbability = 0;
        //Load Total Probability
        limitedPrefabs.ForEach(x => totalProbability += x.probability);
        int random = Random.Range(0, totalProbability);

        int count = 0;
        FloorPrefab selected = null;
        foreach (FloorPrefab floor in prefabs)
        {
            count += floor.probability;
            if (count > random)
            {
                selected = floor;
                break;
            }
        }

        return selected;
    }

    string RemoveParCharacters(string text){
        string finalText = "";
        for (int i = 0; i < text.Length; i++) {
            if (i % 2 == 0) {
                finalText += text[i];
            }
        }
        return finalText;
    }

    public Vector3 GetGenerationPoint() {
        return lastPoint;
    }
}

[System.Serializable]
public class FloorPrefab{
    public string name = "Prefab";
    public GameObject prefab;
    public int  probability = 1;
    public int  startAt = 0;
    public bool canRepeat = true;
}