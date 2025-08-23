using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpaceShipSelect : MonoBehaviour
{
    public GameObject SpaceShip1;
    public GameObject SpaceShip2;

    public GameObject SpaceShip1Name;
    public GameObject SpaceShip2Name;

    public int MaxShips = 2;
    public int currentShip = 1;

    private float PauseTime;
    private bool spaceShipSelected = false;
    private bool TimeCounting = false;


    // Start is called before the first frame update
    void Start()
    {
        SpaceShip1Name.gameObject.SetActive(true);
        SpaceShip2Name.gameObject.SetActive(false);
        SpaceShip2.SetActive(false);
        SpaceShip1.SetActive(true);
        spaceShipSelected = true;

        PauseTime = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeCounting == true)
        {
            PauseTime -= Time.deltaTime;
            if (PauseTime <= 0)
            {
                TimeCounting = false;
                PauseTime = 0.5f;
                spaceShipSelected = true;
            }
        }
        if (Input.GetAxis("Horizontal") > 0 && !TimeCounting)
        {            
            Debug.Log("Right");
            if (currentShip < MaxShips)
            {
                currentShip++;
                SpaceShip1Name.gameObject.SetActive(false);
                SpaceShip2Name.gameObject.SetActive(true);
                SpaceShip1.SetActive(false);
                SpaceShip2.SetActive(true);
                spaceShipSelected = true;
                TimeCounting = true;
            }
        }
        if (Input.GetAxis("Horizontal") < 0 && !TimeCounting)
        {
            Debug.Log("Left");
            if (currentShip > 1)
            {
                currentShip--;
                SpaceShip1Name.gameObject.SetActive(true);
                SpaceShip2Name.gameObject.SetActive(false);
                SpaceShip2.SetActive(false);
                SpaceShip1.SetActive(true);
                spaceShipSelected = true;
                TimeCounting = true;
            }
        }

        if (spaceShipSelected == true)
        {
            Debug.Log(currentShip);
            if (currentShip == 1)
            {
                GameController.SelectedShip = SpaceShip1;
                SpaceShip1.gameObject.SetActive(true);
                SpaceShip2.gameObject.SetActive(false);
                spaceShipSelected = false;
            }
            else if (currentShip == 2)
            {
                GameController.SelectedShip = SpaceShip2;
                SpaceShip2.gameObject.SetActive(true);
                SpaceShip1.gameObject.SetActive(false);
                spaceShipSelected = false;
            }
        }
    }
}
