using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using static GetColor;
public class BattleOrderPrefab : MonoBehaviour
{
    public Image Background;

    public TMP_Text OrdernameText;  
    public TMP_Text OrderMoveText;  
    public Image OrderCharacterIcon;  
    public TMP_Text OrderForceText;
    public TMP_Text OrderText;
    public GameObject MoveOj;

    private Character OrderCharacter;

    public bool isEnemyAtOrder;

    private BattleCharacterValue orderBattleCharacterValue;


    public TMP_Text Order;


    private void Update()
    {
    }
    public void OrderTextUpdata()
    {
        int indexInParent = transform.GetSiblingIndex() + 1;
        OrderText.text = "NO. " + indexInParent.ToString();

        if (orderBattleCharacterValue.IsPersonBattle())
        {
            OrderForceText.text = OrderCharacter.GetHealthAndMaxHealthString();
            MoveOj.gameObject.SetActive(false);
        }
        else
        {
            OrderForceText.text = OrderCharacter.Force.ToString("N0") + "/" + OrderCharacter.MaxForce.ToString("N0");
            MoveOj.gameObject.SetActive(true);

        }
        OrderMoveText.text = GetValueColorString(" * " + orderBattleCharacterValue.MoveNum.ToString(), ValueColorType.CanMove);

    }


    public void SetCharacterToOrder(BattleCharacterValue battleCharacterValue)
    {
        //    this.OrderSpeed = orderSpeed;

        if (orderBattleCharacterValue != null)
        {
            orderBattleCharacterValue.OnValueChanged -= OrderTextUpdata;
        }

        this.orderBattleCharacterValue = battleCharacterValue;


        OrderCharacter = battleCharacterValue.characterValue;
        if (OrderCharacter != null)
        {
            orderBattleCharacterValue.OnValueChanged += OrderTextUpdata;

            OrderCharacterIcon.sprite = OrderCharacter.icon;
            OrdernameText.text = OrderCharacter.GetCharacterName();
            OrderTextUpdata();
        }

        if (battleCharacterValue.isEnemy)
        {
            Background.color = GetRowColor(RowType.enemy);  
        } else
        {
            Background.color = GetRowColor(RowType.player);

        }

        isEnemyAtOrder = battleCharacterValue.isEnemy;
    }

    public BattleCharacterValue GetBattleCharacterValue()
    {
        return orderBattleCharacterValue;
    }

    public int GetSpeed()
    {
        return orderBattleCharacterValue.MoveSpeed;
    }


    private void OnDestroy()
    {
        if (orderBattleCharacterValue != null)
        {
            orderBattleCharacterValue.OnValueChanged -= OrderTextUpdata;
        }
    }

}
