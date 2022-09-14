using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleInputNamespace;
using DG.Tweening;

public class CharaController : MonoBehaviour
{
    public Joystick Joystick;

    public float Speed;

    private float turnsmoothvelocity;

    public float turnsmoothtime;

    public Animator Chara;

    public GameObject FoodstackPoint;

    public List<GameObject> FoodPile = new List<GameObject>();

    public GameObject FoodPileObject;

    public int MyMoney;

    public GameObject CharaCam, CarCam;

    public List<GameObject> MoneyList = new List<GameObject>();

    public Transform MOneyListPoint;

    public GameObject MoneyPrefab;

    public Transform TruckPositionIndex;

    public int TruckPositionIndexCounter;

    public bool IsPlayerLoadedWithFood, IsPlayerAllowedToMove;

    // Start is called before the first frame update
    void Start()
    {
        IsPlayerAllowedToMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        float Horizontal = SimpleInput.GetAxisRaw("Horizontal");
        float Vertical = SimpleInput.GetAxisRaw("Vertical");



        if (Joystick.joystickHeld && IsPlayerAllowedToMove)
        {
            Chara.SetFloat("Run", 1);
            Vector3 direction = new Vector3(Horizontal, 0, Vertical).normalized;
            float targetangle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref turnsmoothvelocity, turnsmoothtime);
            transform.rotation = Quaternion.Euler(0, targetangle, 0);
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        }
        else
        {
            Chara.SetFloat("Run", 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("UTruck"))
        {
            other.gameObject.tag = "Truck";
            IsPlayerLoadedWithFood = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Food"))
        {
            if (!IsPlayerLoadedWithFood)
            {
               // FoodPileObject = other.GetComponentInParent<SlotMG>().GetFoodType();
                StartCoroutine(PileUp(7));
                IsPlayerLoadedWithFood = true;
            }
        }
        else if (other.gameObject.CompareTag("Slot"))
        {
            int Price = other.gameObject.GetComponent<SlotMG>().MyPrice;
            StartCoroutine(PileDownMyMoney(Price, other.gameObject.transform.GetChild(0).gameObject, other.gameObject));
        }
        else if (other.gameObject.CompareTag("Truck"))
        {
            if (FoodPile.Count > 1)
            {
                other.gameObject.tag = "UTruck";
                IsPlayerAllowedToMove = false;
                if(other.gameObject.GetComponent<PlayerController>().FillUpTruck(7, FoodPileObject))
                {
                    print("Got All of them");
                }
                StartCoroutine(PileDown(7,other.transform.GetChild(0).transform));
            }
        }
        else if (other.gameObject.CompareTag("DrivePoint"))
        {
            other.gameObject.GetComponentInParent<PlayerController>().IsReadyToDrive = true;
            GameMG.FindObjectOfType<GameMG>().IsPiledAllFood();
            CarCam.gameObject.SetActive(true);
            CharaCam.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("MoneyPoint"))
        {
            StartCoroutine(PileUpMoney(MyMoney));
        }
    }

    

    IEnumerator PileUp(int Count)
    {
        while (Count >= 0)
        {
            Count -= 1;
            Vector3 pos = new Vector3(FoodPile[FoodPile.Count - 1].transform.position.x, FoodPile[FoodPile.Count - 1].transform.position.y + 0.3f, FoodPile[FoodPile.Count - 1].transform.position.z);
            GameObject Food = Instantiate(FoodPileObject, pos, transform.rotation);
            Food.transform.parent = FoodstackPoint.transform;
            FoodPile.Add(Food);
            yield return new WaitForSeconds(Time.deltaTime * 0.2f);
        }
    }


    IEnumerator PileDown(int DownNumber,Transform Other)
    {

        while (DownNumber > -1 && FoodPile.Count > -1)
        {

            GameObject Temp = FoodPile[FoodPile.Count - 1];
             FoodPile.Remove(FoodPile[FoodPile.Count - 1]);
            Destroy(Temp,1.3f);
            AnimateFoodPileDown(Temp, Other);
            DownNumber--;
            if(DownNumber == 0)
            {
                IsPlayerAllowedToMove = true;
            }
            yield return new WaitForSeconds(Time.deltaTime * 2f);
        }
    }

    public void AnimateFoodPileDown(GameObject Temp,Transform other)
    {
        Temp.gameObject.transform.DOMove(new Vector3(other.position.x, other.position.y, other.position.z), 1f).SetEase(Ease.OutSine).OnComplete(() => { Temp.gameObject.SetActive(false); });
    }

    IEnumerator PileDownMyMoney(int Price,GameObject FarmLand, GameObject Slot)
    {
        int MoneyToReduce = 0;
                                                                                                                                            
        if(Price > MyMoney)                                                                                              // player has less money
        {
            print("Player has less money");
            MoneyToReduce = MyMoney;
            print(MoneyToReduce);
            Slot.gameObject.GetComponent<SlotMG>().MyPrice = Price - MoneyToReduce;
        }
        else if (Price <= MyMoney)                                                                                     // player has more money
        {
            print("Player has enoguh money");
            MoneyToReduce = Price;
            FoodPileObject = Slot.GetComponent<SlotMG>().GetFoodType();
        }

        while(MoneyToReduce > 0)
        {
            MoneyToReduce--;
            MyMoney--;
            Price--;
            Destroy(MoneyList[MoneyList.Count - 1].gameObject);
            MoneyList.Remove(MoneyList[MoneyList.Count - 1].gameObject);
            if (Price == 0)
            {
                FarmLand.gameObject.SetActive(true);
                Slot.gameObject.GetComponent<BoxCollider>().enabled = false;       
            }
            yield return new WaitForSeconds(Time.deltaTime * 0.2f);
        }
    }

    IEnumerator PileUpMoney(int Price)
    {
        while(Price > 0)
        {
            Price--;
            Vector3 pos = new Vector3(MoneyList[MoneyList.Count - 1].transform.position.x, MoneyList[MoneyList.Count - 1].transform.position.y + 0.1f, MoneyList[MoneyList.Count - 1].transform.position.z);
            GameObject Moeny = Instantiate(MoneyPrefab, pos, Quaternion.identity);
            MoneyList.Add(Moeny);
            Moeny.transform.parent = MOneyListPoint;
            yield return new WaitForSeconds(Time.deltaTime * 0.2f);
        }
    }

}

