using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Cinemachine;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    public PathCreator Pathcreator;
    public float speed;
    float distancetravelled;
    public Transform PositionIndex;
    public GameObject FoodObject;
    public int posindex;                                     // this int isused  to keep track of  all the  points on truck on which the objects are spawned.
    public Transform FoodHolder;
    public GameObject DrivePoint;
    public bool IsReadyToDrive;
    public List<GameObject> FoodList = new List<GameObject>();
    public GameObject CarCam;
    public IEnumerator EndPointMovementCorotine;
    public GameObject Money;
    public int TotalFoodCount;
    public IEnumerator PileStuffCoro,PileDownStuff;
    public bool AutoDdrive,hasreachedend;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsReadyToDrive)
        {
            if (Input.anyKey || AutoDdrive)
            {
                distancetravelled += speed * Time.deltaTime;
                transform.position = Pathcreator.path.GetPointAtDistance(distancetravelled);
                transform.rotation = Pathcreator.path.GetRotationAtDistance(distancetravelled);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("SellingPoint"))
        {
            other.gameObject.tag = "Untagged";
            PileDownStuff = (EmptyTruck(TotalFoodCount,false,other.transform.GetChild(0).transform));
            StartCoroutine(PileDownStuff);
            IsReadyToDrive = false;
            hasreachedend = true;
        }
        else if(other.gameObject.CompareTag("RoadEndPoint"))
        {
            IsReadyToDrive = false;
            GameObject Point = GameObject.FindGameObjectWithTag("Slot");
            Point.GetComponent<BoxCollider>().enabled = true;
            EndPointMovementCorotine = MoveVechicleAtNewLand(Point.transform);
            StartCoroutine(EndPointMovementCorotine);
        }
        else if(other.gameObject.CompareTag("Slot"))
        {
            // int Price = other.gameObject.GetComponent<SlotMG>().MyPrice;
            StartCoroutine(ActivateProp(other.gameObject.transform.GetChild(0).gameObject));
            //other.gameObject.transform.GetComponent<SlotMG>().enabled = false;
            other.gameObject.GetComponent<BoxCollider>().enabled = true;
            PileDownStuff = (EmptyTruck(TotalFoodCount,true, other.transform.GetChild(1).transform));
            StartCoroutine(PileDownStuff);
            int a = other.gameObject.transform.GetSiblingIndex();
            print(a);
            PlayerPrefs.SetInt("LandNumbers", a);
            Invoke("CallEndFun", 3);
        }
        else if(other.gameObject.CompareTag("Finish"))
        {
            PileDownStuff = (EmptyTruck(TotalFoodCount, true, other.transform));
            StartCoroutine(PileDownStuff);
            Invoke("CallEndFun", 3);
        }
    }

    public void CallEndFun()
    {
        GameMG.FindObjectOfType<GameMG>().OnGameFinish(true);
    }

    IEnumerator ActivateProp(GameObject Prop)
    {
        yield return new WaitForSeconds(2);
        Prop.gameObject.SetActive(true);
    }

    IEnumerator MoveVechicleAtNewLand(Transform Point)
    {
        float Distance = Vector3.Distance(transform.position, Point.position);
        
        while(Distance > 0 )
        {
            if (Distance <= 0.5f)
            {
                StopCoroutine(EndPointMovementCorotine);
            }
            Distance = Vector3.Distance(transform.position, Point.position);
            Distance = Mathf.Round(Distance * 100.0f) * 0.01f;
            transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            transform.LookAt(Point);
            yield return new WaitForSeconds(0.1f * Time.deltaTime);

        }
    }
    public bool FillUpTruck(int FoodCount,GameObject FoodType)
    {
        print(posindex);
        if (posindex < 10)
        {
            TotalFoodCount += FoodCount;
            FoodObject = FoodType;
            PileStuffCoro = (FillTruck(FoodCount, FoodType,false,null));
            StartCoroutine(PileStuffCoro);
            return false;
        }
        else
        {
            StopCoroutine(PileStuffCoro);
            PileStuffCoro = null;
            FoodObject = null;
            posindex = 0;
            DrivePoint.gameObject.SetActive(true);
            return true;
        }
    }

    IEnumerator FillTruck(int FoodCount, GameObject FoodType,bool IsFillingMoney,Transform Other)
     {
         while (FoodCount > 0 )
         {

            if (IsFillingMoney)
            {
                Vector3 pos = Other.position;
                GameObject Food = Instantiate(FoodType, pos, Other.rotation);
                Vector3 NewPos = new Vector3(PositionIndex.GetChild(posindex).transform.position.x, PositionIndex.GetChild(posindex).transform.position.y, PositionIndex.GetChild(posindex).transform.position.z);
                Food.gameObject.transform.DOMove(NewPos, 2f).SetEase(Ease.InCubic);
                Food.transform.parent = FoodHolder;
                FoodList.Add(Food);
                posindex++;
                FoodCount--;
                if (FoodCount == 0)
                {
                    StopCoroutine(PileStuffCoro);
                    if (hasreachedend)
                    {
                        Invoke("OnAutoDriving", 2f);
                    }
                }
            }
            else
            {
                Vector3 pos = PositionIndex.GetChild(posindex).transform.position;
                GameObject Food = Instantiate(FoodType, pos, Quaternion.identity);
                Food.transform.parent = FoodHolder;
                FoodList.Add(Food);
                posindex++;
                FoodCount--;
            }

             yield return new WaitForSeconds(Time.deltaTime * 1f);
         }
     }

    public void OnAutoDriving()
    {
        IsReadyToDrive = true;
        AutoDdrive = true;
    }

    IEnumerator EmptyTruck(int FoodCount,bool isfinished,Transform Other)
    {
        while (FoodCount > 0)
        { 

            FoodCount--;
            AnimateFoodPileDown(FoodList[FoodList.Count - 1].gameObject, Other);
            Destroy(FoodList[FoodList.Count - 1].gameObject,3);
            FoodList.Remove(FoodList[FoodList.Count - 1].gameObject);
            if (FoodCount == 0)
            {
                if (!isfinished)
                {
                    PileStuffCoro = (FillTruck(TotalFoodCount, Money,true,Other));
                    StartCoroutine(PileStuffCoro);
                }
                StopCoroutine(PileDownStuff);
            }
            yield return new WaitForSeconds(Time.deltaTime * 2f);
        }
    }

    public void AnimateFoodPileDown(GameObject Temp, Transform other)
    {
        Temp.gameObject.transform.DOMove(new Vector3(other.position.x, other.position.y, other.position.z), 1f).SetEase(Ease.OutSine).OnComplete(() => { Temp.gameObject.SetActive(false); }); 
    }

}


