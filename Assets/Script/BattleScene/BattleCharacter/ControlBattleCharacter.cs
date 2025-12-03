using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ControlBattleCharacter : MonoBehaviour
{
 /*   public List<PositionAtBattle> PlayerBattlePositionList;
    public List<Button> buttons;
    public List<Button> specialButtons;


    public BattleCharacter battleCharacter;

    public int currentCol = 0;
    public int currentRow = 0;


    private PositionAtBattle[,] PlayerBattlePositionArray;
    private Button[,] ButtonArray;
    private Button[,] specialButtonArray;


    public void setContolBattleCharacter()
    {
        PlayerBattlePositionArray = new PositionAtBattle[3, 3];
        Fill2DArrayFromList(PlayerBattlePositionList, 3, 3, PlayerBattlePositionArray);
        ButtonArray = new Button[3, 3];
        Fill2DArrayFromList(buttons, 3, 3, ButtonArray);
        InitializeButtonsList();

        specialButtonArray = new Button[4, 2];
        Fill2DArrayFromList(specialButtons, 4, 2, specialButtonArray);
        InitializeSpecialButtonsList();



        UpdateMoveButton();

    }


    void Fill2DArrayFromList<T>(List<T> list, int rows, int columns, T[,] array)
    {
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (index < list.Count)
                {
                    array[i, j] = list[index];
                    index++;
                }
            }
        }
    }

    public void Update()
    {
        // if (Input.GetMouseButtonDown(1)) {/* canceMove(); */};


        /*    if (battleCharacter.battleCharacterValue != null && currentRow < 3 && currentCol < 3&&PlayerBattlePositionArray[currentRow, currentCol].battlecharacterValue != battleCharacter.battleCharacterValue)
            {
                for (currentRow = 0; currentRow < 3; currentRow++)
                {
                    for (currentCol = 0; currentCol < 3; currentCol++)
                    {
                       if (PlayerBattlePositionArray[currentRow, currentCol].battlecharacterValue == battleCharacter.battleCharacterValue)
                        {
                            return;
                        }
                    }
                }
            }

        UpdateMoveButton();
        UpdateSpecialMoveButton();

    }


    void InitializeButtonsList()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 1 || j != 1)
                {
                    int row = i;
                    int col = j;
                    ButtonArray[i, j].onClick.AddListener(() => MoveCharacter(row, col)); // ????????
                }
            }
        }

    }


    void InitializeSpecialButtonsList()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int row = i;
                int col = j;
                specialButtonArray[i, j].onClick.AddListener(() => SpecialMoveCharacter(row, col));

            }
        }

    }

    private void SpecialMoveCharacter(int row, int col)
    {
        int rowOffset = 100;
        int colOffset = 100;

        /*  
          if  (row % 2 == 0) { rowOffset = -2; } else { rowOffset = 2; };
          if (col == 0) { colOffset = -1; } else { colOffset = 1; 




        if (row == 0) { rowOffset = -2; }
        else
        if (row == 2) { rowOffset = 2; }
        else
        {
            if (col == 0) { rowOffset = -1; }
            ;
            if (col == 1) { rowOffset = 1; }
            ;
        }

        if (row == 1) { colOffset = 2; }
        else
        if (row == 3) { colOffset = -2; }
        else
        {
            if (col == 0) { colOffset = -1; }
            ;
            if (col == 1) { colOffset = 1; }
            ;
        }


        specialButtonArray[row, col].gameObject.SetActive(true);

        if (currentRow + rowOffset >= 3 || currentRow + rowOffset < 0)
        {
            specialButtonArray[row, col].gameObject.SetActive(false);
            return;
        }


        if (currentCol + colOffset >= 3 || currentCol + colOffset < 0)
        {
            specialButtonArray[row, col].gameObject.SetActive(false);
            return;

        }

     /*   if (PlayerBattlePositionArray[currentRow + rowOffset, currentCol + colOffset].battlecharacterValue.characterValue != null)
        {
            currentRow += rowOffset;
            currentCol += colOffset;
            battleCharacter.battleCharacterValue = PlayerBattlePositionArray[currentRow, currentCol].battlecharacterValue;
            battleCharacter.SetCharacter(battleCharacter.battleCharacterValue);
        }
     


    }

    private void MoveCharacter(int row, int col)
    {
        int rowOffset = calculateOffset(row);
        int colOffset = calculateOffset(col);


        /*
                if (currentRow + rowOffset >= 3 || currentRow + rowOffset < 0) return;
                if (currentCol + colOffset >= 3 || currentCol + colOffset < 0) return;

                if (PlayerBattlePositionArray[currentRow + rowOffset, currentCol + colOffset].battlecharacterValue.characterValue != null)
                {


                    currentRow += rowOffset;
                    currentCol += colOffset;


                    battleCharacter.battleCharacterValue = PlayerBattlePositionArray[currentRow, currentCol].battlecharacterValue;
                    battleCharacter.SetCharacter(battleCharacter.battleCharacterValue);
                    return;

                }

                if (currentRow + 2 * rowOffset >= 3 || currentRow + 2 * rowOffset < 0) return;
                if (currentCol + 2 * colOffset >= 3 || currentCol + 2 * colOffset < 0) return;

                if (PlayerBattlePositionArray[currentRow + rowOffset, currentCol + colOffset].battlecharacterValue.characterValue == null 
                    && PlayerBattlePositionArray[currentRow + 2 * rowOffset, currentCol + 2 * colOffset].battlecharacterValue.characterValue != null)
                {
                    currentRow += 2 * rowOffset;
                    currentCol += 2 * colOffset;

                    battleCharacter.battleCharacterValue = PlayerBattlePositionArray[currentRow, currentCol].battlecharacterValue;
                    battleCharacter.SetCharacter(battleCharacter.battleCharacterValue);

                }

                
    }

    private void UpdateMoveButton()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 1 || j != 1) UpdateMoveButtonHelper(i, j);
            }
        }
    }

    private void UpdateSpecialMoveButton()
    {


        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                UpdateMoveSpecialButtonHelper(i, j);
            }
        }


    }



    void UpdateMoveSpecialButtonHelper(int row, int col)
    {

        return;
        int rowOffset = 100;
        int colOffset = 100;

        if (row == 0) { rowOffset = -2; }
        else
        if (row == 2) { rowOffset = 2; }
        else
        {
            if (col == 0) { rowOffset = -1; }
            ;
            if (col == 1) { rowOffset = 1; }
            ;
        }

        if (row == 1) { colOffset = 2; }
        else
        if (row == 3) { colOffset = -2; }
        else
        {
            if (col == 0) { colOffset = -1; }
            ;
            if (col == 1) { colOffset = 1; }
            ;
        }


        specialButtonArray[row, col].gameObject.SetActive(true);

        if (currentRow + rowOffset >= 3 || currentRow + rowOffset < 0)
        {
            specialButtonArray[row, col].gameObject.SetActive(false);
            return;
        }


        if (currentCol + colOffset >= 3 || currentCol + colOffset < 0)
        {
            specialButtonArray[row, col].gameObject.SetActive(false);
            return;

        }

    /*    if (PlayerBattlePositionArray[currentRow + rowOffset, currentCol + colOffset].battlecharacterValue.characterValue == null)
        {
            specialButtonArray[row, col].gameObject.SetActive(false);
            return;
        }


    }


    private int calculateOffset(int num)
    {
        if (num == 0) { return -1; }
        else if (num == 1) { return 0; }
        else if (num == 2) { return 1; }

        return 100;
    }


    private void UpdateMoveButtonHelper(int row, int col)
    {
        /*  return;
          int rowOffset = calculateOffset(row);
          int colOffset = calculateOffset(col);

          //ButtonArray[row, col].gameObject.SetActive(true);
          ButtonArray[row, col].gameObject.GetComponent<Image>().color = Color.gray;


          if (currentRow + rowOffset >= 3 || currentRow + rowOffset < 0) { 
              //ButtonArray[row, col].gameObject.SetActive(false);
              return; }
          if (currentCol + colOffset >= 3 || currentCol + colOffset < 0) { 
              //ButtonArray[row, col].gameObject.SetActive(false);
              return; }

          if (PlayerBattlePositionArray[currentRow + rowOffset, currentCol + colOffset].battlecharacterValue.characterValue != null)
          {
              ButtonArray[row, col].gameObject.GetComponent<Image>().color = Color.red;
              return;

          }

          if (currentRow + 2 * rowOffset >= 3 || currentRow + 2 * rowOffset < 0) {
              //ButtonArray[row, col].gameObject.SetActive(false);
              return; }
          if (currentCol + 2 * colOffset >= 3 || currentCol + 2 * colOffset < 0) { 
              //ButtonArray[row, col].gameObject.SetActive(false);
              return; }

          if (PlayerBattlePositionArray[currentRow + rowOffset, currentCol + colOffset].battlecharacterValue.characterValue == null
              && PlayerBattlePositionArray[currentRow + 2 * rowOffset, currentCol + 2 * colOffset].battlecharacterValue.characterValue != null)
          {
              ButtonArray[row, col].gameObject.GetComponent<Image>().color = Color.blue;
              return;

          }

         // ButtonArray[row, col].gameObject.SetActive(false);
      }
              

    }
}
/* public void Update()
 {
     if (Input.GetMouseButtonDown(1)) { canceMove(); };


     if (battleCharacterValueList[currentNum] != battleCharacter.battleCharacterValue)
     {
         currentNum = battleCharacterValueList.IndexOf(battleCharacter.battleCharacterValue);
     }
 }

                if (battleCharacterArray[currentRow, currentCol] != battleCharacter.battleCharacterValue)
                {
                    return;
                }


 public void setContolBattleCharacter()
 {
     InitializeBattleCharacterList();
     InitializeButtonsList();

     battleCharacterArray = new BattleCharacterValue[3, 3];
     Fill2DArrayFromList(battleCharacterValueList, 3, 3);
     ButtonArray = new Button[3, 3];
     Fill2DArrayFromButton(buttons, 3, 3);



     for (int i = 0; i < 3; i++)
     {
         for (int j = 0; j < 3; j++)
         {
             Debug.Log($"battleCharacterArray{i},{j}" + battleCharacterArray[i, j].gameObject.name);
             if (i != 1 || j != 1) Debug.Log($"ButtonArray{i},{j}" + ButtonArray[i, j].gameObject.name);

         }

     }

     UpdateMoveButton();

 }

 void InitializeBattleCharacterList()
 {
     currentNum = 0;
     while (battleCharacterValueList[currentNum] != battleCharacter.battleCharacterValue)
     {
         currentNum++;
     }
 }

 void InitializeButtonsList()
 {
     for (int i = 0; i < buttons.Count; i++)
     {
         int index = i;  
         if (i != 4 ){buttons[i].onClick.AddListener(() => OnButtonClick(index));}
     }
 }

 public void OnButtonClick(int index)
 {
     if (index == 0) // Move left-up (diagonal)
     {
         MoveCharacter((currentNum - 4),true);
     }
     else if (index == 1) // Move up
     {
         MoveCharacter((currentNum - 3),false);
     }
     else if (index == 2) // Move right-up (diagonal)
     {
         MoveCharacter((currentNum - 2), true);
     }
     else if (index == 3) // Move left
     {
         MoveCharacter((currentNum - 1), false);
     }
     else if (index == 5) // Move right
     {
         MoveCharacter((currentNum + 1),false);
     }
     else if (index == 6) // Move left-down (diagonal)
     {
         MoveCharacter((currentNum + 2), true);
     }
     else if (index == 7) // Move down
     {
         MoveCharacter((currentNum + 3), false);
     }
     else if (index == 8) // Move right-down (diagonal)
     {
         MoveCharacter((currentNum + 4), true);
     }
 }
 void MoveCharacter(int newNum, bool isDiagonal)
 {
     if (currentNum / 3 == newNum / 3 && isDiagonal) return;

     if ((currentNum / 3 != newNum / 3 && !isDiagonal) &&
         (Mathf.Abs(currentNum - newNum) == 1 || Mathf.Abs(newNum - currentNum) == 1))
         return;

     if (newNum < 0 || newNum >= battleCharacterValueList.Count) return;

     int index = 2 * newNum - currentNum;

     if (index < 0 || index >= battleCharacterValueList.Count)
     {
         if (battleCharacterValueList[newNum].characterValue == null) return;
     } else
     {
         if (battleCharacterValueList[newNum].characterValue == null &&
         battleCharacterValueList[index]?.characterValue == null)
         {
             return;
         }

     }

     if (battleCharacterValueList[newNum].characterValue == null)
     {
         newNum = index;
     }



     currentNum = newNum;

     battleCharacter.battleCharacterValue = battleCharacterValueList[currentNum];
     battleCharacter.SetCharacter(battleCharacter.battleCharacterValue);

     UpdateMoveButton();


 }



 void Fill2DArrayFromList(List<BattleCharacterValue> list, int rows, int columns)
 {
     int index = 0;

     for (int i = 0; i < rows; i++)
     {
         for (int j = 0; j < columns; j++)
         {
             if (index < list.Count)
             {
                 battleCharacterArray[i, j] = list[index];
                 index++;
             }
         }
     }
 }

 void Fill2DArrayFromButton(List<Button> list, int rows, int columns)
 {
     int index = 0;

     for (int i = 0; i < rows; i++)
     {
         for (int j = 0; j < columns; j++)
         {
                 ButtonArray[i, j] = list[index];
             index++;

         }
     }
 }

 void UpdateMoveButton()
 {
     for (int i = 0; i < buttons.Count; i++)
     {
         if (i != 4)
         {
             buttons[i].gameObject.SetActive(true);
             buttons[i].gameObject.GetComponent<Image>().color = Color.white;
         }
     }

     int row = currentNum % 3;
     int col = currentNum / 3;

     // Define the directions to check, [col][row]
     var directions = new (int colOffset,int rowOffset, int buttonX, int buttonY)[]
     {
     (0, 1, 2, 1),  // Down
     (0, -1, 0, 1), // Up
     (1, 0, 1, 2),  // Right
     (-1, 1, 1, 0), // Left
     (-1, -1, 0, 0),// Top-left
     (1, -1, 0, 2), // Top-right
     (-1, 1, 2, 0), // Bottom-left
     (1, 1, 2, 2)   // Bottom-right
     };

     foreach (var dir in directions)
     {
         UpdateButtonColor(row, col, dir.rowOffset, dir.colOffset, dir.buttonX, dir.buttonY);
     }
 }

 bool IsValidMove(int num, int Offset)
 {
     if (num + Offset < 0 || num + Offset >= 3)
     {
         return false;
     }

     return true;
 }


 void UpdateButtonColor(int row, int col, int rowOffset, int colOffset, int buttonX, int buttonY)
 {
     IsValidMove(row, rowOffset);
     IsValidMove(col, colOffset);


     Debug.Log("col " + col + " colOffset + " + colOffset);
     Debug.Log("row " + row + " rowOffset + " + rowOffset);
     Debug.Log("==========================================");

     if (IsValidMove(row, rowOffset) && IsValidMove(col, colOffset)
     && battleCharacterArray[col+ colOffset, row +rowOffset].characterValue != null)
     {
         UpdateButtonColorByState(buttonX, buttonY, Color.red);
     }
     else
     {


         if (IsValidMove(row, rowOffset * 2) &&
             IsValidMove(col, colOffset * 2))
         {

             Debug.Log("col " + col + " colOffset + " + colOffset);
             Debug.Log("row " + row + " rowOffset + " + rowOffset);
             Debug.Log("==========================================");

             for (int i = 0; i < 3; i++)
             {
                 for (int j = 0; j < 3; j++)
                 {
                     Debug.Log($"battleCharacterArray{i},{j}" + battleCharacterArray[i, j].gameObject.name);
                     if (i != 1 || j != 1) Debug.Log($"ButtonArray{i},{j}" + ButtonArray[i, j].gameObject.name);

                 }

             }
             Debug.Log("==========================================");
             Debug.Log("battleCharacterArray[col + colOffset * 2, row + rowOffset * 2] game object name" + battleCharacterArray[col + colOffset * 2, row + rowOffset * 2].gameObject.name);
             Debug.Log("battleCharacterArray[col + colOffset * 2, row + rowOffset * 2] game object name" + battleCharacterArray[col + colOffset * 2, row + rowOffset * 2].characterValue.characterName);


             if (battleCharacterArray[col + colOffset * 2, row + rowOffset * 2].characterValue != null)
             {
                 UpdateButtonColorByState(buttonX, buttonY, Color.blue);
             } else
             {
                 ButtonArray[buttonX, buttonY].gameObject.SetActive(false);
             }
         }
     }
 }

 void UpdateButtonColorByState(int buttonX, int buttonY, Color color)
 {
     if (ButtonArray[buttonX, buttonY] != null)
     {
         Button button = ButtonArray[buttonX, buttonY];
         if (button.gameObject != null)
         {
             var imageComponent = button.gameObject.GetComponent<Image>();
             if (imageComponent != null)
             {
                 imageComponent.color = color;
             }
         }
     }
 }



 void UpdateMoveButtonHelper()
 {
 }

 public void canceMove()
 {
   //  battleCharacter.SetCharacter(currentBattleCharacter.battleCharacterValue);
 }



} */

