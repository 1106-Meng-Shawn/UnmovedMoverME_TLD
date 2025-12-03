using System;
using System.Collections.Generic;
using UnityEngine;
using static PositionAtBattle;
public enum BattleValue
{ 
    Attack = 0,
    Defense = 1,
    Magic = 2,
    Speed = 3,
    Lucky = 4
}

public class BattleCharacterValue : MonoBehaviour
{
    public bool isAtPosition;
    public Character characterValue;
    int criticalAnimationShowCount = 0;
    int blockAnimationShowCount = 0;


    private List<int> values = new List<int>();


    private int _moveNum;
    public int MoveNum
    {
        get => _moveNum;
        set
        {
            if (_moveNum != value)
            {
                _moveNum = value;
                NotifyValueChanged();
            }
        }
    }

    private int _moveSpeed;
    public int MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (_moveSpeed != value)
            {
                _moveSpeed = value;
                NotifyValueChanged();
            }
        }
    }



    public bool isEnemy;
    public PositionAtBattle positionAtBattle;

    public event Action OnValueChanged;

    public Vector2 position;

    /// <summary>
    /// Call this to notify any UI or systems that values have changed.
    /// </summary>
    public void NotifyValueChanged()
    {
        OnValueChanged?.Invoke();
    }

    public int GetValueAt(BattleValue index)
    {
        return values[(int)index];
    }

    public void SetValueAt(BattleValue index, int value)
    {
        int newValue = Mathf.Max(0, value);  
        if (values[(int)index] != newValue)
        {
            values[(int)index] = newValue;
            NotifyValueChanged();
        }
    }

    public void InitializeValues(List<int> initialValues)
    {
        values = new List<int>(initialValues);
        NotifyValueChanged();
    }


    public void ApplyDamage(DamageResult damageResult)
    {
        if (IsPersonBattle())
        {
            characterValue.Health = Math.Max(0, characterValue.Health - damageResult.Damage);
        }
        else
        {
            characterValue.Force = Math.Max(0, characterValue.Force - damageResult.Damage);
        }
    }


    public void ApplyHeal(int heal)
    {
        if (IsPersonBattle())
        {
            characterValue.Health = Math.Min(characterValue.GetMaxHealth(), characterValue.Health + heal);
        }
        else
        {
            characterValue.Force = Math.Min(characterValue.MaxForce, characterValue.Force + heal);
        }
    }


    public int GetForce()
    {
        // return characterValue != null ? characterValue.Force : 0;
        if (characterValue == null) return 0;
        if (IsPersonBattle())
        {
            return characterValue.Health;
        }else
        {
            return characterValue.Force;
        }
    }

    public void SetCharacterToValue(Character character)
    {
        if (characterValue != null)
        {
            characterValue.OnCharacterChanged -= OnCharacterValueChanged;
        }

        characterValue = character;

        if (character != null)
        {
            characterValue.OnCharacterChanged += OnCharacterValueChanged;

            MoveNum = character.BattleMoveNum;

            values.Clear();
            for (int i = 0; i < 5; i++)
            {
                values.Add(character.GetValue(0, i));
            }
            MoveSpeed = character.GetValue(0, 3);

            if (positionAtBattle != null)
            {
                positionAtBattle.SetCharacterToPosition(this);
            }

            NotifyValueChanged();
        }
        else
        {
            if (positionAtBattle != null)
            {
                positionAtBattle.SetCharacterToPosition(null);
            }
        }
    }
    private void OnCharacterValueChanged()
    {
        // Update internal values when character changes
        if (characterValue != null)
        {
            MoveNum = characterValue.BattleMoveNum;

            values.Clear();
            for (int i = 0; i < 5; i++)
            {
                values.Add(characterValue.GetValue(0, i));
            }
            MoveSpeed = characterValue.GetValue(0, 3);

            NotifyValueChanged();
        }
    }

    public bool CanBlock()
    {
        if (characterValue == null) return false;

        if (characterValue.Force == 0)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public bool IsPersonBattle()
    {
        if (characterValue == null) return BattleManage.Instance.IsExploreBattle();
        return characterValue.IsPersonBattle || BattleManage.Instance.IsExploreBattle();
    }


    public bool ShowCriticalAnimation()
    {
        int chanceBase = criticalAnimationShowCount;
        bool isShow = UnityEngine.Random.Range(0, chanceBase) == 0;

        if (isShow)
        {
            criticalAnimationShowCount++;
        }

        return isShow;
    }


    public bool ShowBlockAnimation(bool isShowCriticalAnimation)
    {
        if (isShowCriticalAnimation) return true;
        int chanceBase = blockAnimationShowCount;
        bool isShow = UnityEngine.Random.Range(0, chanceBase) == 0;

        if (isShow)
        {
            blockAnimationShowCount++;
        }

        return isShow;
    }

    public bool IsAlive()
    {
        if (characterValue == null) return false;
        if (IsPersonBattle())
        {
            return characterValue.Health > 0;
        } else
        {
            return characterValue.Force > 0;

        }
    }

    public string GetBattleCharacterName()
    {
        string name = characterValue.GetCharacterName();
        string level = $"Lv.{characterValue.GetCurrentLevel()}";

        if (IsPersonBattle())
        {
            return $"<i>{name}</i> {level}";
        }
        else
        {
            return $"<i>{name}</i>";
        }
    }


    private void OnDestroy()
    {
        if (characterValue != null)
        {
            characterValue.OnCharacterChanged -= OnCharacterValueChanged;
        }
    }
}
