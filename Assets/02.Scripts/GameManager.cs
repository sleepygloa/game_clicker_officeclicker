using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public static long money;

    public GameObject prefabCoffee;
    public GameObject prefabEmployee;
    public GameObject prefabTextMoney;

    public Sprite[] spriteF;
    public Sprite[] spriteM;

    public Vector2 limitPoint1;
    public Vector2 limitPoint2;

    public Text textMoney;

    public Sprite[] spriteButtonState;
    public bool isWhip;
    public Image imgButton;

    public GameObject panelInfo;

    public static string familyName {
        get {
            string[] name = new string[10];

            name[0] = "김";
            name[1] = "이";
            name[2] = "최";
            name[3] = "박";
            name[4] = "한";
            name[5] = "성";
            name[6] = "조";
            name[7] = "윤";
            name[8] = "서";
            name[9] = "민";

            int r = Random.Range(0, name.Length);
            string s = name[r];

            return s;
        }
    }

    public static string names {
        get
        {
            string[] name = new string[10];

            name[0] = "님님";
            name[1] = "냥냥";
            name[2] = "소소";
            name[3] = "헤헤";
            name[4] = "헿헿";
            name[5] = "핳핳";
            name[6] = "홓홓";
            name[7] = "후후";
            name[8] = "히히";
            name[9] = "화화";

            int r = Random.Range(0, name.Length);
            string s = name[r];

            return s;
        }
    }

    public List<Employee> emps;

    private string savePath;

    private void Awake()
    {
        gm = this;
        savePath = Application.persistentDataPath + "/save.sav";
    }

    // Start is called before the first frame update
    void Start()
    {
        emps = new List<Employee>();

        if (System.IO.File.Exists(savePath)) {
            Load();
            money = 10000;
            foreach (var a in emps) {
                InitializeEmployee(a);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject() == false) {
            if (isWhip == true)
            {
                EarnMoney();
            }
            else
            {
                CreateCoffee();
            }
        }

        if (GameObject.Find("Panel_Option") != null)
        {
            Time.timeScale = 0;
        }
        else {
            Time.timeScale = 1;
        }

        ShowMoneyText();
        ChangeButtonSprite();
    }

    public void EarnMoney() {
        if (Input.GetMouseButtonDown(0))
        {
            money += 1;
            Debug.Log(money);
        }
    }

    void CreateCoffee() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //게임 오브젝트생성
            Instantiate(prefabCoffee, mousePosition, Quaternion.identity);

        }
    }

    void ShowMoneyText() {
        textMoney.text = money.ToString("###,###");
    }

    void ChangeButtonState() {
        if (isWhip == true)
        {
            isWhip = false;
        }
        else {
            isWhip = true;
        }
    }

    void ChangeButtonSprite() {
        if (isWhip == true)
        {
            imgButton.sprite = spriteButtonState[0];
            imgButton.rectTransform.sizeDelta = new Vector2(70, 70);
        }
        else {
            imgButton.sprite = spriteButtonState[1];
            imgButton.rectTransform.sizeDelta = new Vector2(50, 70);
        }
    }

    public void PanelHireOpen() {

        panelInfo.SetActive(true);

        Employee e = RandomCreateEmployee();

        var textName = panelInfo.transform.Find("Text_Name").GetComponent<Text>();
        var imageCharacter = panelInfo.transform.Find("Image_Character").GetComponent<Image>();
        var textProgramming = panelInfo.transform.Find("Text_Programming").GetComponent<Text>();
        var textDesign = panelInfo.transform.Find("Text_Design").GetComponent<Text>();
        var textSound = panelInfo.transform.Find("Text_Sound").GetComponent<Text>();
        var textArt = panelInfo.transform.Find("Text_Art").GetComponent<Text>();
        var textSalary = panelInfo.transform.Find("Button_Hire/Image_Price/Text").GetComponent<Text>();
        var buttonHire = panelInfo.transform.Find("Button_Hire").GetComponent<Button>();

        textName.text = e.name;
        if (e.gender == (int)Gender.Female)
        {
            imageCharacter.sprite = spriteF[0];
        }
        else {
            imageCharacter.sprite = spriteM[0];
        }

        textProgramming.text = e.programming.ToString();
        textDesign.text = e.design.ToString();
        textSound.text = e.sound.ToString();
        textArt.text = e.art.ToString();
        textSalary.text = e.salary.ToString();

        buttonHire.onClick.RemoveAllListeners();
        buttonHire.onClick.AddListener(delegate {
            Hire((int)e.salary, e);
        });

    }


    Employee RandomCreateEmployee() {
        Employee info = new Employee();

        info.name = familyName + names;
        info.hp = 100;

        info.design = Random.Range(0, 101);
        info.programming = Random.Range(0, 101);
        info.sound = Random.Range(0, 101);
        info.art = Random.Range(0, 101);


        info.salary = Random.Range(100, 1001);

        int r = Random.Range(0, 2);
        info.gender = r;

        return info;

    }

    void CreateEmployee(Employee employee) {
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //GameObject obj = Instantiate(prefabEmployee, mousePosition, Quaternion.identity);

        GameObject obj = Instantiate(prefabEmployee, Vector3.zero, Quaternion.identity);
    }

    void InitializeEmployee(Employee e)
    {
        GameObject obj = Instantiate(prefabEmployee, Vector3.zero, Quaternion.identity);
        obj.GetComponent<EmployeeControl>().info = e;
    }

    public void Hire(int price, Employee e ) {
        if (money > price) {
            CreateEmployee(e);
            money -= price;
            emps.Add(e);

            panelInfo.SetActive(false);
        }
    }

    public void Save() {
        //PlayerPrefs.SetInt("MONEY", (int)money);
        //PlayerPrefs.SetString("MONEY", money.ToString());    
        SaveData sd = new SaveData();

        sd.money = money;
        sd.empList = emps;
        XmlManager.XmlSave<SaveData>(sd, savePath);
    }

    public void Load() {
        //money = PlayerPrefs.GetInt("MONEY", 1000);
        //money = long.Parse(PlayerPrefs.GetString("MONEY"));
        SaveData sd = XmlManager.XmlLoad<SaveData>(savePath);
        money = sd.money;
        emps = sd.empList;
    }

    private void OnApplicationQuit()
    {
        Save();
        Application.Quit();
    }

    private void OnDrawGizmos() {
        Vector2 limitPoint3 = new Vector2(limitPoint2.x, limitPoint1.y);
        Vector2 limitPoint4 = new Vector2(limitPoint1.x, limitPoint2.y);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(limitPoint1, limitPoint3);
        Gizmos.DrawLine(limitPoint3, limitPoint2);
        Gizmos.DrawLine(limitPoint1, limitPoint4);
        Gizmos.DrawLine(limitPoint4, limitPoint2);  
    }

    public void VolumnChange(Slider sl) {
        GetComponent<AudioSource>().volume = sl.value;
    }

    public void SaveDelete() {
        if (System.IO.File.Exists(savePath)) {
            System.IO.File.Delete(savePath); 
        }
    }

    public void lapTest() {
        money += 10000;
    }

}

[System.Serializable]
public class SaveData {
    public long money;
    public List<Employee> empList;
}