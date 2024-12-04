using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;


public class DataManager : MonoBehaviour
{
    public delegate void ChangeData(float exp, int levels, float gold, List<string> inventory, string right, string left);
    public static ChangeData changingData;

    [SerializeField] private GameObject player;
    [SerializeField] private string _scene;
    [SerializeField] private int _health;
    [SerializeField] private float _exp;
    [SerializeField] private int _levels;
    [SerializeField] private float _gold;
    [SerializeField] private List<string> _inventoryItems;
    [SerializeField] private string _rightWeapon;
    [SerializeField] private string _leftWeapon;
    private void OnEnable()
    {
        PlayerGold.ChangingGold += GetGold;
        PlayerStats.changingUI += GetHealth;
        PlayerExp.changingExp += GetExp;
        PlayerInventory.givingData += GetInventoryItems;
        player = GameObject.FindWithTag("Player");
    }
    private void OnDisable()
    {
        PlayerGold.ChangingGold -= GetGold;
        PlayerStats.changingUI -= GetHealth;
        PlayerExp.changingExp -= GetExp;
        PlayerInventory.givingData += GetInventoryItems;
        }
    public void TransferData()
    {
        if (changingData != null)
        {
            changingData(_exp, _levels, _gold, _inventoryItems, _rightWeapon, _leftWeapon);
        }
    }
    void GetHealth(int amount, int stamina, int maxhealth)
    {
        _health = amount;
    }
    void GetExp(float exp, int levels, float levelCost)
    {
        _exp = exp;
        _levels = levels;
    }
    void GetGold(float gold)
    {
        _exp = gold;
    }
    void GetInventoryItems(List<string> data, string right, string left)
    {
        _inventoryItems = data;
        _rightWeapon = right;
        _leftWeapon = left;
    }
    public void LoadData()
    { 
        string json = File.ReadAllText(Application.dataPath + "/Data/playerData.json");
        Data data = JsonUtility.FromJson<Data>(json);
        player = GameObject.FindWithTag("Player");
        //apply data to a var in data scrip like variable = data.variable
        player.transform.position = data.playerPos;
        player.transform.rotation = data.playerRot;
        _inventoryItems = data.inventoryItems;
        _health = data.health;
        _exp = data.exp;
        _levels = data.level;
        _gold = data.gold;
        _rightWeapon = data.rightWeapon;
        _leftWeapon = data.leftWeapon;
        TransferData();
        if (_scene != null && data._scene == SceneManager.GetActiveScene().name){
            return;
        }else{
            GameManager.Instance.gameObject.GetComponent<LoadingScreen>().SceneSwitch(data._scene);
        }
    }
    public void SaveData()
    {
        player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerInventory>().GivingData();
        Data data = new Data();
        //save data like data.variable = variable;
        data._scene = SceneManager.GetActiveScene().name;
        data.playerPos = player.transform.position;
        data.playerRot = player.transform.rotation;
        data.inventoryItems = _inventoryItems;
        data.exp = _exp;
        data.health = _health;
        data.level = _levels;
        data.gold = _gold;
        data.rightWeapon = _rightWeapon;
        data.leftWeapon = _leftWeapon;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.dataPath + "/Data/playerData.json", json);
        TransferData();
    }
}

public class Data
{
    //data to save
    public string _scene;
    public Vector3 playerPos;
    public Quaternion playerRot;
    public int health;
    public float exp;
    public int level;
    public float gold;
    public List<string> inventoryItems;
    public string rightWeapon;
    public string leftWeapon;
}


