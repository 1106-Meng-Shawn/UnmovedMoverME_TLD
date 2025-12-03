using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpoloreSelectCard : MonoBehaviour
{
    public GameObject left; 
    public GameObject center; 
    public GameObject right;

    public GameObject leftCollider;
    public GameObject centerCollider;
    public GameObject rightCollider;


    ExploreCardNode leftNode;
    ExploreCardNode centerNode;
    ExploreCardNode rightNode;


    private void Start()
    {
        leftNode = left.GetComponent<ExploreCardNode>();
        centerNode = center.GetComponent<ExploreCardNode>();
        rightNode = right.GetComponent<ExploreCardNode>();


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == leftCollider)
                {
                    MoveToTheCard("left");
                }
                else if (hit.transform.gameObject == centerCollider) 
                {
                    MoveToTheCard("center");
                }
                else if (hit.transform.gameObject == rightCollider)
                {
                    MoveToTheCard("right");
                }
            }
        }
    }

    private void MoveToTheCard(string direction)
    {
        /*
        if (exploreCardTree.farRightNode.gameObject.activeSelf) { exploreCardTree.farRightNode.gameObject.SetActive(false); }
        if (exploreCardTree.farLeftNode.gameObject.activeSelf) { exploreCardTree.farLeftNode.gameObject.SetActive(false); }


        if (direction == "left")
        {

            leftNode.EventHappen();

            exploreCardTree.root.SetExploreCardData(leftNode.GetExploreCardData());
            exploreCardTree.level = leftNode.GetLevel();

            leftNode.MoveTheCard("left");
            centerNode.MoveTheCard("left");
            rightNode.MoveTheCard("left");
            exploreCardTree.farLeftNode.MoveTheCard("left");
            exploreCardTree.farRightNode.MoveTheCard("left");

            exploreCardTree.farRightNode.Center.MoveTheCard("left"); // second level
            exploreCardTree.farRightNode.Right.MoveTheCard("left"); // second level


            exploreCardTree.farRightNode.Right.Center.MoveTheCard("left");// third level
            exploreCardTree.farRightNode.Right.Center.Center.MoveTheCard("left");//fourth
            exploreCardTree.farRightNode.Right.Center.Center.Center.MoveTheCard("left"); // fifth




        }
        else if (direction == "center")
            {
            centerNode.EventHappen();

            exploreCardTree.root.SetExploreCardData(centerNode.GetExploreCardData());

            exploreCardTree.level = centerNode.GetLevel();

            leftNode.MoveTheCard("center");
            centerNode.MoveTheCard("center");
                rightNode.MoveTheCard("center");

            exploreCardTree.farLeftNode.MoveTheCard("center");
            exploreCardTree.farLeftNode.MoveThefarCard(-1, "center");

            exploreCardTree.farRightNode.MoveTheCard("center");
            exploreCardTree.farRightNode.MoveThefarCard(1, "center");




        }
        else if (direction == "right")
        {

            rightNode.EventHappen();

            exploreCardTree.root.SetExploreCardData(rightNode.GetExploreCardData());

            exploreCardTree.level = rightNode.GetLevel();


            leftNode.MoveTheCard("right");
            centerNode.MoveTheCard("right");
            rightNode.MoveTheCard("right");
            exploreCardTree.farLeftNode.MoveTheCard("right");
            exploreCardTree.farRightNode.MoveTheCard("right");


            exploreCardTree.farLeftNode.Center.MoveTheCard("right"); // second level
            exploreCardTree.farLeftNode.Left.MoveTheCard("right"); // second level


            exploreCardTree.farLeftNode.Left.Center.MoveTheCard("right");// third level, most left
            exploreCardTree.farLeftNode.Left.Center.Center.MoveTheCard("right");//fourth, most left
            exploreCardTree.farLeftNode.Left.Center.Center.Center.MoveTheCard("right"); // fifth most left



        }

        exploreCardTree.GenerateExploreCardTree();
        */
    }


}
