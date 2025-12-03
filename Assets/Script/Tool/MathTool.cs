using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTool : MonoBehaviour
{
    static class LevelAndForceConstants
    {
        public const int FirstLevel = 1;
        public const int SecondLevel = 30;
        public const int ThirdLevel = 100;
        public const int FourthLevel = 200;
        public const int FifthLevel = 999;

        public const int FirstForce = 1;
        public const int SecondForce = 100;
        public const int ThirdForce = 1000;
        public const int FourthForce = 9999;
        public const int FifthForce = 99999;


    }


    private static readonly (int input, int output)[] LevelAndForceMappingPoints = new[]
    {
        (LevelAndForceConstants.FirstLevel, LevelAndForceConstants.FirstForce),
        (LevelAndForceConstants.SecondLevel, LevelAndForceConstants.SecondForce),
        (LevelAndForceConstants.ThirdLevel, LevelAndForceConstants.ThirdForce),
        (LevelAndForceConstants.FourthLevel, LevelAndForceConstants.FourthForce),
        (LevelAndForceConstants.FifthLevel, LevelAndForceConstants.FifthForce)
    };

    public static int LevelAndForceMapValue(int input)
    {
        if (input > LevelAndForceConstants.FifthLevel)
            return LevelAndForceConstants.FifthForce + input ;  

        if (input <= LevelAndForceMappingPoints[0].input)
            return LevelAndForceMappingPoints[0].output;

        if (input >= LevelAndForceMappingPoints[^1].input)
            return LevelAndForceMappingPoints[^1].output;

        for (int i = 0; i < LevelAndForceMappingPoints.Length - 1; i++)
        {
            var point1 = LevelAndForceMappingPoints[i];
            var point2 = LevelAndForceMappingPoints[i + 1];

            if (input >= point1.input && input <= point2.input)
            {
                float t = (float)(input - point1.input) / (point2.input - point1.input);
                return Mathf.RoundToInt(Mathf.Lerp(point1.output, point2.output, t));
            }
        }

        return LevelAndForceMappingPoints[^1].output;
    }
}
