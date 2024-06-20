using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public enum  dataName
    {
        PlayerSex,
        StartScreen,
    }

    private Dictionary<dataName, string> _dataType = new Dictionary<dataName, string>()
    {
        {dataName.PlayerSex,"PlayerSex"},
        {dataName.StartScreen,"StartScreen"},
    };

    public static DataManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void SaveData(dataName _name, string data)
    {
        PlayerPrefs.SetString(_dataType[_name],data);
        PlayerPrefs.Save();
    }
    
    public void SaveData(dataName _name, int data)
    {
        PlayerPrefs.SetInt(_dataType[_name],data);
        PlayerPrefs.Save();
    }
    
    public void SaveData(dataName _name, float data)
    {
        PlayerPrefs.SetFloat(_dataType[_name],data);
        PlayerPrefs.Save();
    }
    
    public int LoadDataInt(dataName _name)
    {
        int val = 0;
        if (PlayerPrefs.HasKey(_dataType[_name]))
        {
            val = PlayerPrefs.GetInt(_dataType[_name]);
        }
        return val;
    }
    
    public Dictionary<int,Tuple<string,string,int,int,int,int,string>> _weaponsData = new Dictionary<int, Tuple<string,string,int,int,int,int,string>>()
    {
        {1,Tuple.Create("Sacrifial","Sword",30,100,50,100,"The sword of a knight that symbolizes the restored honor of Dvalin. The blessings of the Anemo Archon rest on the fuller of the blade")},
        {2,Tuple.Create("Bloodtainted","Polearm",25,110,50,100,"A greatsword as light as the sigh of grass in the breeze, yet as merciless to the corrupt as a typhoon.")}, 
        {3,Tuple.Create("Harbinger","Polearm",20,130,50,100,"A symbol of a legendary pact, this sharp blade once cut off the peak of a mountain.")}, 
        {4,Tuple.Create("Deathmatch","Claymore",45,150,50,100,"A weapon once used by a young maiden who forsook her family name, stained with the blood of enemies and loved ones both.")},
        {5,Tuple.Create("Aquila Favonia","Sword",45,150,50,100,"The soul of the Knights of Favonius. Millennia later, it still calls on the winds of swift justice to vanquish all evil — just like the last heroine who wielded it.")},
        {6,Tuple.Create("Calamity Queller","Sword",45,150,50,100,"A keenly honed weapon forged from some strange crystal. Its faint blue light seems to whisper of countless matters now past.")},
        {7,Tuple.Create("Black Tassel","Sword",45,150,50,100,"A naginata used to \"cut grass.\" Any army that stands before this weapon will probably be likewise cut down.")},
        {8,Tuple.Create("Skyward Blade","Sword",45,150,50,100,"The sword of a knight that symbolizes the restored honor of Dvalin The blessings of the Anemo Archon rest on the fuller of the blade.")},
        {9,Tuple.Create("Staff of Homa","Sword",45,150,50,100,"A \"firewood staff\" that was once used in ancient and long-lost rituals.")},
        {10,Tuple.Create("Akuoumaru","Sword",45,150,50,100,"The beloved sword of the legendary \"Akuou.\" The blade is huge and majestic, but is surprisingly easy to wield.")},
        {11,Tuple.Create("Blackcliff Pole","Sword",45,150,50,100,"A weapon made of blackstone and aerosiderite. There is a dark crimson glow on its cold black sheen.")},
        {12,Tuple.Create("Festering Desire","Sword",45,150,50,100,"A creepy straight sword that almost seems to yearn for life. It drips with a shriveling venom that could even corrupt a mighty dragon.")},
        {13,Tuple.Create("Hamayumi","Claymore",45,150,50,100,"A certain shrine maiden once owned this warbow. It was made with surpassing skill, and is both intricate and sturdy.")},
        {14,Tuple.Create("Ibis Piercer","Claymore",45,150,50,100,"A golden bow forged from the description in the story. If you use it as a normal weapon,")},
        {15,Tuple.Create("Sacrificial Jade","Claymore",45,150,50,100,"An ancient jade pendant that gleams like clear water. It seems to have been used in ancient ceremonies.")},
        {16,Tuple.Create("Tidal Shadow","Claymore",45,150,50,100,"An exquisitely-crafted. standard-model sword forged for the high-ranking officers and flagship captains of Fontaine's old navy.")},
    };
}
