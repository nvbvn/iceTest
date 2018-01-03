using UnityEngine;
using System.Collections;

public class MathUtil
{

    public static bool ApproximatelyLessThanOrEqual(float val1, float val2) {
        bool res = false;
        if (val1 < val2 || Mathf.Approximately(val1, val2)) {
            res = true;
        }
        return res;
    }

}
