using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArraySet : MonoBehaviour
{
    public BattlePanelValue battlePanelValue;  
    public List<BattlePosition> battlePositions;


    public void SetEnemyToPosition()
    {
        if (battlePanelValue.enemyCharacters.Count > 0)
        {
            // ????????????????????
            List<Character> enemies = new List<Character>(battlePanelValue.enemyCharacters);

            // ??????
            foreach (Character enemy in enemies)
            {
                BattlePosition position;

                // ??????????
                do
                {
                    position = battlePositions[Random.Range(0, battlePositions.Count)];
                } while (position.characterAtBattlePosition != null); // ??????????????

                // ?????????
                position.characterAtBattlePosition = enemy;
            //    enemy.battlePosition = position;
            }
        }
    }

    public void ClearPosition()
    {
        for (int i = 0; i < battlePositions.Count; i++)
        {
             

            if (battlePositions[i].characterAtBattlePosition != null)
            {
             //   battlePositions[i].characterAtBattlePosition.battlePosition = null;
                battlePositions[i].characterAtBattlePosition = null;

            }
        }
        battlePanelValue.enemyCharacters.Clear();
    }



}
