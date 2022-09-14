using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMG : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Farmer, Vechicle, FarmLand;

    public Button AcceptOrderBT,EndGameOkBT;

    public Transform EndFarmLandPosition;

    public GameObject[] Lands;

    public GameObject StartMenu, InGameMenu, EndfailMenu, EndWinMenu;

    public GameObject[] NPCVechicles;



    void Start()
    {
        //int LandNumbers = PlayerPrefs.GetInt("LandNumbers");
        //for(int a = 0; a<= LandNumbers; a++)
        //{
        //    print(Lands[a].transform.parent);
        //    Lands[a].gameObject.SetActive(true);
        //    Lands[a].gameObject.transform.parent.gameObject.SetActive(true);
        //    Lands[a].gameObject.transform.parent.gameObject.tag = "Untagged";
        //    Lands[a].gameObject.transform.parent.GetComponent<BoxCollider>().enabled = false;
        //    Lands[a].gameObject.transform.parent.GetComponent<SlotMG>().enabled = false;
        //}
        //Lands[LandNumbers+1].transform.parent.gameObject.SetActive(true);
        AcceptOrderBT.onClick.AddListener(OnAcceptOrder);
        StartCoroutine(ActivateNPCVechicles());
    }

    IEnumerator ActivateNPCVechicles()
    {
        int a = 0;
        while( a <=  NPCVechicles.Length-1)
        {
            NPCVechicles[a].gameObject.SetActive(true);
            a++;
            yield return new WaitForSeconds(Random.Range(1, 10) * Time.deltaTime);
        }
    }
    public void OnAcceptOrder()
    {
        Farmer.GetComponent<CharaController>().enabled = true;
        Vechicle.GetComponent<PlayerController>().enabled = true;
        InGameMenu.SetActive(true);
        StartMenu.gameObject.SetActive(false);
    }

    public void IsPiledAllFood()
    {
        GameObject EndFarmLand = Instantiate(FarmLand, EndFarmLandPosition.position, Quaternion.identity);
        Invoke("DeactivateFarmLand", 2);
    }

    public void DeactivateFarmLand()
    {
        FarmLand.gameObject.SetActive(false);
        InGameMenu.SetActive(false);
    }


    public void OnGameFinish(bool HasWon)
    {
        if(!HasWon)
        {
            print("failed");
            InGameMenu.gameObject.SetActive(false);
            EndfailMenu.gameObject.SetActive(true);
        }
        else
        {
            print("Won");
            InGameMenu.gameObject.SetActive(false);
            EndWinMenu.gameObject.SetActive(true);
            EndGameOkBT.onClick.AddListener(ReloadGame);
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }
 
}
