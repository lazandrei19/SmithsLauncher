using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class wrestling : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static bool pointerDown = false;
    public float x;
    public int timp;
    public GameObject baramica;
    public GameObject baramare;
    public Text textuletz;
    public Text texttimp;
	public int timpreal = Mathf.RoundToInt( 10f / GameController.control.GetItem("selling/sword").GetDifficulty () );
    public int variabilascor = 0;
    public float smek = 2;
    Vector2 mousePosition;
    float y = 0.02f;
	public bool firsttime = false;

    float localwidth;

    public void OnPointerDown(PointerEventData eventData)
    {
		if (firsttime)
			return;
		firsttime = true;
        pointerDown = true;
        CancelInvoke("Move");

        CancelInvoke("Masoaratimpul");
        InvokeRepeating("Move", 0, 0.05f);

        InvokeRepeating("Ptmaus", 0, 0.05f);
        InvokeRepeating("Masoaratimpul", 1, 1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
    }

	void Start ()
    {
        localwidth = GetComponent<RectTransform>().rect.width;
        y = (float)((1 - (GetComponent<RectTransform>().rect.width / baramica.GetComponent<RectTransform>().rect.width)) / timp) * 0.05f;
    }

    void Masoaratimpul()
    {
        timpreal++;
        texttimp.text = "Time: " + (timp - timpreal);
    }

    void Ptmaus()
    {
        mousePosition = new Vector2(Input.mousePosition.x, 0);
    }

    int NumarRandom(int a, int b)
    {

        if(Random.Range(1, 3) == 1)
        {
            return a;
        }
        else
        {
            return b;
        }
    }

    void Move ()
    {
        if (timp <= timpreal)
        {
            CancelInvoke("Masoaratimpul");
			int price = Mathf.RoundToInt(((ItemSword) GameController.control.GetItem("selling/sword")).GetBasePrice () + ((ItemSword) GameController.control.GetItem("selling/sword")).GetBasePrice () * ((variabilascor - 50) / 200f));
			GameController.control.SetInt ("price", price);
			GameController.control.SetInt ("currency", GameController.control.GetInt ("currency") + price);
			GameController.control.SetBool ("sold", true);
			QuestSystem.q.CheckRequirements ((ItemSword) GameController.control.GetItem("selling/sword"));
			SceneManager.LoadScene("MainMenu");
        }


        if(baramare.transform.position.x - baramare.GetComponent<RectTransform>().rect.width / 2 + GetComponent<RectTransform>().rect.width / 2 < mousePosition.x
            && mousePosition.x < baramare.transform.position.x + baramare.GetComponent<RectTransform>().rect.width / 2 - GetComponent<RectTransform>().rect.width / 2)

            transform.position = new Vector3(mousePosition.x, transform.position.y, transform.position.z);



        if (localwidth < baramica.GetComponent<RectTransform>().rect.width * baramica.transform.localScale.x)
        {
            smek -= y;
            baramica.transform.localScale = new Vector3(smek * 2, 2, 2);
        }

        if ( baramica.transform.localPosition.x < (-1)*(baramare.GetComponent<RectTransform>().rect.width - baramica.transform.localScale.x * baramica.GetComponent<RectTransform>().rect.width)/2)
        {
            baramica.transform.Translate(Vector3.right * x * Time.deltaTime);
        }
        else if (baramica.transform.localPosition.x > (baramare.GetComponent<RectTransform>().rect.width - baramica.transform.localScale.x * baramica.GetComponent<RectTransform>().rect.width) / 2 )
        {
            //Debug.Log(baramica.transform.localPosition.x + " " + (baramare.GetComponent<RectTransform>().rect.width - baramica.transform.localScale.x * baramica.GetComponent<RectTransform>().rect.width) / 2);
            baramica.transform.Translate(Vector3.left * x * Time.deltaTime);
        }
        else
        {
            if (baramica.transform.localPosition.x - baramica.GetComponent<RectTransform>().rect.width / 2 * baramica.transform.localScale.x - GetComponent<RectTransform>().rect.width/2 > transform.localPosition.x - GetComponent<RectTransform>().rect.width / 2
                || baramica.transform.localPosition.x - baramica.GetComponent<RectTransform>().rect.width / 2 * baramica.transform.localScale.x  + baramica.GetComponent<RectTransform>().rect.width * baramica.transform.localScale.x + GetComponent<RectTransform>().rect.width / 2 < transform.localPosition.x - GetComponent<RectTransform>().rect.width / 2 + GetComponent<RectTransform>().rect.width)
            {
                baramica.transform.Translate(Vector3.left * x * Time.deltaTime * 2);
            }
            else
            {
                baramica.transform.Translate(Vector3.right * x * Time.deltaTime * NumarRandom(-1, 2));
            }
        }
        variabilascor = (int)((baramare.GetComponent<RectTransform>().rect.width/2 + baramica.transform.localPosition.x) *100 / baramare.GetComponent<RectTransform>().rect.width)/2+25;
		textuletz.text = "Score: " + (variabilascor - 50);
        //Debug.Log(baramica.transform.localPosition.x - GetComponent<RectTransform>().rect.width/2);
    }
	
	void Update () {

    }
}
