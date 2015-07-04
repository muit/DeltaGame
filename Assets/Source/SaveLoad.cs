using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class SaveLoad : MonoBehaviour{

    public string seed;
    public Dictionary<string, List<ScrollItem>> bestMarks = new Dictionary<string, List<ScrollItem>>();

    public string[] randomNames;

    void Start() {
        DontDestroyOnLoad(this);
    }

    public void SaveSeed(string newSeed)
    {
        seed = newSeed;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/seed.gd");
        bf.Serialize(file, newSeed);
        file.Close();
    }

    public string LoadSeed()
    {
        if (File.Exists(Application.persistentDataPath + "/seed.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/seed.gd", FileMode.Open);
            seed = (string)bf.Deserialize(file);
            file.Close();
            return seed;
        }
        return null;
    }

    public void SaveBestMark(string seed, string name, int points)
    {
        LoadBestMark(seed);

        if (name == null || name == "") {
            name = randomNames[Random.Range(0, randomNames.Length)];
            if (name == null) {
                name = "";
            }
        }

        if (bestMarks == null) {
            bestMarks = new Dictionary<string, List<ScrollItem>>();
        }

        if (!bestMarks.ContainsKey(seed)) {
            bestMarks.Add(seed, new List<ScrollItem>());
        }

        bestMarks[seed].Add(new ScrollItem(name, points));

        bestMarks[seed] = bestMarks[seed].OrderByDescending(a => a.points).ToList();
        if (bestMarks[seed].Count >= 10) {
            //Remove lower marks
            bestMarks[seed].RemoveRange(10, bestMarks[seed].Count - 10);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/bestMarks.gd");
        bf.Serialize(file, bestMarks);
        file.Close();
    }

    public List<ScrollItem> LoadBestMark(string seed)
    {
        if (File.Exists(Application.persistentDataPath + "/bestMarks.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/bestMarks.gd", FileMode.Open);
            bestMarks = (Dictionary<string, List<ScrollItem>>)bf.Deserialize(file);
            file.Close();
            if (bestMarks != null && bestMarks[seed] != null)
            {
                bestMarks[seed] = bestMarks[seed].OrderByDescending(a => a.points).ToList();
                return bestMarks[seed];
            }
        }
        return null;
    }
}
