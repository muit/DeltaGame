using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class SaveLoad : MonoBehaviour{

    public string seed;
    public List<ScrollItem> bestMarks = new List<ScrollItem>();

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
            bestMarks = new List<ScrollItem>();
        }

        bestMarks.Add(new ScrollItem(name, points));

        bestMarks = bestMarks.OrderByDescending(a => a.points).ToList();
        if (bestMarks.Count >= 10) {
            //Remove lower marks
            bestMarks.RemoveRange(10, bestMarks.Count - 10);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/bestMarks_" + seed + ".gd");
        bf.Serialize(file, bestMarks);
        file.Close();
    }

    public void LoadBestMark(InputField input)
    {
        Menu menu = FindObjectOfType<Menu>();
        if(menu){
            menu.bestMarks.PopulateList(LoadBestMark(input.text));
        }
    }

    public List<ScrollItem> LoadBestMark(string seed)
    {
        if (File.Exists(Application.persistentDataPath + "/bestMarks_" + seed + ".gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/bestMarks_"+seed+".gd", FileMode.Open);
            bestMarks = (List<ScrollItem>)bf.Deserialize(file);
            file.Close();
            if (bestMarks != null)
            {
                bestMarks = bestMarks.OrderByDescending(a => a.points).ToList();
                return bestMarks;
            }
        }
        return null;
    }
}
