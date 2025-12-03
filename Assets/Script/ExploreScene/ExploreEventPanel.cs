using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ExploreEventPanel : MonoBehaviour
{

    public TMP_Text title;
    public TMP_Text eventDescription;
    public Button check;
    public Button cance;

    public string CardType;
    public ExploreCardNode exploreCardNode;

    void Start()
    {
        check.onClick.AddListener(EventHappend);
        cance.onClick.AddListener(DestroyPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatEvent(string CardType)
    {
        //Nothing = 0,Trap = 1,Heal = 2,Battle = 3,Item = 4,Escape = 5,Pass = 6
        this.CardType = CardType;

        switch (CardType)
        {
            case "trap":
                title.text = "Trap!";
                eventDescription.text = "You triggered the trap !";
                AdjustButton(1);
                break;
            case "heal":
                title.text = "Heal!";
                eventDescription.text = "Your health has been restored !";
                AdjustButton(1);
                break;
            case "battle":
                title.text = "Battle!";
                eventDescription.text = "The battle happens !";
                AdjustButton(1);

                break;
            case "item":
                title.text = "Item!";
                eventDescription.text = "You get the Item !";
                AdjustButton(1);

                break;
            case "escape":
                title.text = "Escape!";
                eventDescription.text = "Exit the exploration !";
                AdjustButton(2);

                break;
            case "pass":
                title.text = "Pass!";
                eventDescription.text = "You have completed the exploration! Do you choose to continue?";
                AdjustButton(2);

                break;



            default:
                break;
        }

    }

    void AdjustButton(int num)
    {
        if (num == 1)
        {
            check.gameObject.transform.localPosition = new Vector3(0, -170, 0);
            cance.gameObject.SetActive(false);
        } else if (num == 2)
        {
            check.gameObject.transform.localPosition = new Vector3(-200, -170, 0);
            cance.gameObject.transform.localPosition = new Vector3(200, -170, 0);


        }
    }

    void EventHappend()
    {
        switch (CardType)
        {
            case "trap":
                exploreCardNode.TrapEvent();
                break;
            case "heal":
                exploreCardNode.HealEvent();
                break;
            case "battle":
                exploreCardNode.BattleEvent();
                break;
            case "item":
                exploreCardNode.ItemEvent();
                break;
            case "escape":
                exploreCardNode.EscapeEvent();
                break;
            case "pass":
                exploreCardNode.PassEvent();
                break;
        }

        DestroyPanel();


    }


    void DestroyPanel()
    {
        Destroy(gameObject);
    }


}
