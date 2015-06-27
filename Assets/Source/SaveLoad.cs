using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour{

    public string seed;

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
        return "";
    }
}
