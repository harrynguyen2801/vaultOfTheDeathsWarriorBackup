using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

public class DataManager : MonoBehaviour
{
    public enum DataPrefName
    {
        FirstGame,
        PlayerSex,
        StartScreen,
        Level,
        WeaponId,
        Coin,
    }
    
    public enum EnemyType
    {
        Skeleton,
        MageSkeleton,
        DragonNight,
        DragonUsu,
    }

    public Dictionary<EnemyType, int> DataHealthEnemy = new Dictionary<EnemyType, int>()
    {
        { EnemyType.Skeleton ,120},
        { EnemyType.MageSkeleton ,100},
        { EnemyType.DragonNight ,300},
        { EnemyType.DragonUsu ,250},

    };

    private Dictionary<DataPrefName, string> _dataType = new Dictionary<DataPrefName, string>()
    {
        {DataPrefName.PlayerSex,"PlayerSex"},
        {DataPrefName.StartScreen,"StartScreen"},
        {DataPrefName.Level,"Level"},
        {DataPrefName.WeaponId,"WeaponId"},
        {DataPrefName.Coin,"Coin"},
        {DataPrefName.FirstGame,"FirstGame"},
    };
    
    private Dictionary<int, Tuple<string, string, int, int, int, string, int,Tuple<int>>> _weaponsDataDefault = new Dictionary<int, Tuple<string, string, int, int, int, string, int, Tuple<int>>>()
    {
        {1,Tuple.Create("Sacrifial","Sword",30,100,100,"The sword of a knight that symbolizes the restored honor of Dvalin. The blessings of the Anemo Archon rest on the fuller of the blade",100,1)},
        {2,Tuple.Create("Bloodtainted","Polearm",25,110,100,"A greatsword as light as the sigh of grass in the breeze, yet as merciless to the corrupt as a typhoon.",100,1)}, 
        {3,Tuple.Create("Harbinger","Polearm",20,130,100,"A symbol of a legendary pact, this sharp blade once cut off the peak of a mountain.",100,1)}, 
        {4,Tuple.Create("Deathmatch","Claymore",45,150,100,"A weapon once used by a young maiden who forsook her family name, stained with the blood of enemies and loved ones both.",100,1)},
        {5,Tuple.Create("Aquila Favonia","Sword",45,150,100,"The soul of the Knights of Favonius. Millennia later, it still calls on the winds of swift justice to vanquish all evil — just like the last heroine who wielded it.",100,1)},
        {6,Tuple.Create("Calamity Queller","Sword",45,150,100,"A keenly honed weapon forged from some strange crystal. Its faint blue light seems to whisper of countless matters now past.",100,0)},
        {7,Tuple.Create("Black Tassel","Sword",45,150,100,"A naginata used to cut grass. Any army that stands before this weapon will probably be likewise cut down.",100,0)},
        {8,Tuple.Create("Skyward Blade","Sword",45,150,100,"The sword of a knight that symbolizes the restored honor of Dvalin The blessings of the Anemo Archon rest on the fuller of the blade.",200,0)},
        {9,Tuple.Create("Staff of Homa","Sword",45,150,100,"A firewood staff that was once used in ancient and long-lost rituals.",200,0)},
        {10,Tuple.Create("Akuoumaru","Sword",45,150,100,"The beloved sword of the legendary Akuou. The blade is huge and majestic, but is surprisingly easy to wield.",200,0)},
        {11,Tuple.Create("Blackcliff Pole","Sword",45,150,100,"A weapon made of blackstone and aerosiderite. There is a dark crimson glow on its cold black sheen.",300,0)},
        {12,Tuple.Create("Festering Desire","Sword",45,150,100,"A creepy straight sword that almost seems to yearn for life. It drips with a shriveling venom that could even corrupt a mighty dragon.",300,0)},
        {13,Tuple.Create("Hamayumi","Claymore",45,150,100,"A certain shrine maiden once owned this warbow. It was made with surpassing skill, and is both intricate and sturdy.",300,0)},
        {14,Tuple.Create("Ibis Piercer","Claymore",45,150,100,"A golden bow forged from the description in the story. If you use it as a normal weapon,",300,0)},
        {15,Tuple.Create("Sacrificial Jade","Claymore",45,150,100,"An ancient jade pendant that gleams like clear water. It seems to have been used in ancient ceremonies.",350,0)},
        {16,Tuple.Create("Tidal Shadow","Claymore",45,150,100,"An exquisitely-crafted. standard-model sword forged for the high-ranking officers and flagship captains of Fontaine's old navy.",350,0)},
    };

    public Dictionary<int, Tuple<string, string, int, int, int, string, int ,Tuple<int>>> weaponsData =
        new Dictionary<int, Tuple<string, string, int, int, int, string, int ,Tuple<int>>>() { };

    public static DataManager Instance;
    private void Awake()
    {
        Instance = this;
        if (LoadDataInt(DataPrefName.StartScreen) == 0)
        {
            weaponsData = _weaponsDataDefault;
            SaveDictWeaponToJson();
            Debug.Log("save: ");
        }
        else
        {
            Debug.Log("load: ");
            LoadDictWeaponFromJson();
        }
    }

    public void SaveDataWeapon()
    {
        SaveDictWeaponToJson();
    }
    
    public void LoadDataWeapon()
    {
        LoadDictWeaponFromJson();
    }
    private void SaveDictWeaponToJson()
    {
        var json = JsonConvert.SerializeObject(weaponsData);
        File.WriteAllText(Application.dataPath + "/saveDictWeapon.json",json);
    }

    private void LoadDictWeaponFromJson()
    {
        var json = File.ReadAllText(Application.dataPath + "/saveDictWeapon.json");
        weaponsData = JsonConvert.DeserializeObject<Dictionary<int, Tuple<string, string, int, int, int, string, int, Tuple<int>>>>(json);
    }

    public void SaveData(DataPrefName prefName, string data)
    {
        PlayerPrefs.SetString(_dataType[prefName],data);
        PlayerPrefs.Save();
    }
    
    public void SaveData(DataPrefName prefName, int data)
    {
        PlayerPrefs.SetInt(_dataType[prefName],data);
        PlayerPrefs.Save();
    }
    
    public void SaveData(DataPrefName prefName, float data)
    {
        PlayerPrefs.SetFloat(_dataType[prefName],data);
        PlayerPrefs.Save();
    }
    
    public int LoadDataInt(DataPrefName prefName)
    {
        int val = 0;
        if (PlayerPrefs.HasKey(_dataType[prefName]))
        {
            val = PlayerPrefs.GetInt(_dataType[prefName]);
            // Debug.Log(prefName + " is " +  val);
        }
        return val;
    }
}
