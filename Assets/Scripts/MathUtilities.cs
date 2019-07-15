using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Utilities
{
    public static class Vector2Extension {
     
        public static Vector2 Rotate(this Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
    public static class MathUtilities
    {
        static public float RewriteSecondTimer(float mainValue, float multiply = 1)
        {
            mainValue += Time.deltaTime*multiply;
            if (mainValue > 1)
            {
                mainValue -= 1;
            }

            return mainValue;
        }
        
        static public float GetNormalizeTime(float timeMultiply = 1.0f)
        {
            float mainTime = Time.time*timeMultiply;
            mainTime = mainTime - (int) mainTime;

            return mainTime;
        }

        public static float AngleNormalize(float val)
        {
            int count = (int)(Mathf.Abs(val) / 360);
            if (val > 360)
            {
                val = val - count*360;
            }else if (val < 0)
            {
                val = count*360 + val;
            }

            return val;
        }
        
        public static float AngleNormalizeSimlpe(float val)
        {
            if (val > 360)
            {
                val = val - 360;
            }else if (val < 0)
            {
                val = 360 + val;
            }

            return val;
        }

        public static float AngleToAngle(float curA, float tarA, float speed)
        {
            tarA = AngleNormalize(tarA);
            curA = AngleNormalize(curA);
            
            if (curA - 180 > tarA)
            {
                tarA += 360;
            }
            
            if (curA + 180 < tarA)
            {
                tarA -= 360;
            }
            return (curA+Mathf.Clamp(tarA - curA, -speed, speed));


        }
        
        
    }
}
