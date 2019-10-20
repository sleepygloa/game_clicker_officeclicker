using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeControl : MonoBehaviour
{

    SpriteRenderer spr;

    public Employee info;

    public float speed;

    public Vector2 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        if (string.IsNullOrEmpty(info.name)) { 
            SetInfo();
        
        }

        

        StartCoroutine(EarnMoneyAuto());
        StartCoroutine(HpDecreaseAuto());
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    { 
        SpriteChange();
        ShowInfo();
    }

    IEnumerator EarnMoneyAuto() {
        while (true) {

            int m = 10;
            GameManager.money += m;
            ShowTextMoney(m);

            yield return new WaitForSeconds(1.0f);
        }
    }

    void ShowTextMoney(int m) {
        GameObject obj = Instantiate(GameManager.gm.prefabTextMoney, this.gameObject.transform.Find("Canvas"), false);

        Animator anim = obj.GetComponent<Animator>();
        anim.SetTrigger("Start");

        Text txt = obj.GetComponent<Text>();
        txt.text = "+ " + m.ToString("###,###");

        Destroy(obj, 3f);
    }

    IEnumerator HpDecreaseAuto()
    {
        while (true)
        {

            info.hp -= 1;

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator Move()
    {
        while (true)
        {

            float x = transform.position.x + Random.Range(-2f, 2f);
            float y = transform.position.y + Random.Range(-2f, 2f);

            Vector2 target = new Vector2(x, y);
            target = CheckTarget(target);

            prevPosition = transform.position;

            while (Vector2.Distance(transform.position, target) > 0.01f) { 
                transform.position = Vector2.MoveTowards(transform.position, target, speed);
                yield return null;
            
            }
            yield return new WaitForSeconds(1.0f);
        }   
    }

    Vector2 CheckTarget(Vector2 currentTarget) {
        Vector2 temp = currentTarget;

        //위치수정

        if (currentTarget.x < GameManager.gm.limitPoint1.x)
        {
            temp = new Vector2(currentTarget.x + 4, temp.y);
        }
        else if(currentTarget.y > GameManager.gm.limitPoint2.x){
            temp = new Vector2(currentTarget.x - 4, temp.y);
        }

        if (currentTarget.y > GameManager.gm.limitPoint1.y)
        {
            temp = new Vector2(temp.x, currentTarget.y - 4);
        }
        else if (currentTarget.y < GameManager.gm.limitPoint2.y)
        {
            temp = new Vector2(temp.x, currentTarget.y + 4);
        }

        return temp;
    }

    void SpriteChange() {
        Sprite[] set = null;

        if (info.gender == (int)Gender.Female)
        {
            set = GameManager.gm.spriteF;
        }
        else 
        {
            set = GameManager.gm.spriteM;
        }

        //절대값 비교
        Vector2 abs = (Vector2)transform.position - prevPosition;
        //왼쪽 오른쪽
        if (Mathf.Abs(abs.x) > Mathf.Abs(abs.y))
        {
            spr.sprite = set[2];

            if (transform.position.x > prevPosition.x)
            {
                spr.flipX = false;
            }
            else if(transform.position.x < prevPosition.x)
            {
                spr.flipX = true;
            }

        }
        //위 아래
        else {

            spr.flipX = false;

            if (transform.position.y > prevPosition.y)
            {
                spr.sprite = set[1];
            }
            else if (transform.position.y < prevPosition.y)
            {
                spr.sprite = set[0];
            }
        }


    }


    void SetInfo() {
        info.name = GameManager.familyName + GameManager.names;
        info.hp = 100;

        info.design = Random.Range(0, 101);
        info.programming = Random.Range(0, 101);
        info.sound = Random.Range(0, 101);
        info.art = Random.Range(0, 101);


        info.salary = Random.Range(100, 1001);

        int r = Random.Range(0, 2);
        info.gender = r;

    }

    void ShowInfo() {
        Text txt = gameObject.transform.Find("Canvas/Text_Name").GetComponent<Text>();
        txt.text = info.name;

        Image img = transform.Find("Canvas/Image/Image_Gauge").GetComponent<Image>();
        img.fillAmount = info.hp / 100f;
    }

    //isTrigger 켜있을때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coffee") == true) {
            Debug.Log("커피를 먹음   ");
            info.hp = 100;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Employee") == true) 
        {
            Collider2D col1 = this.gameObject.GetComponent<Collider2D>();
            Collider2D col2 = collision.collider;
            Physics2D.IgnoreCollision(col1, col2);

        }
    }
}

public enum Gender { 
    Female = 0,
    Male = 1
}

[System.Serializable]
public class Employee {
    public string name;
    public int gender;

    public float design;
    public float programming;
    public float art;
    public float sound;

    public float hp;
    public long money;

    public float salary;
}

