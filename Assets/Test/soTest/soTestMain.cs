using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class soTestMain : MonoBehaviour
{
    [SerializeField]
    private Button btn;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(clickListener);
    }

    private void clickListener() {
        ScriptableObject.CreateInstance<TestSO>();
      //  AssetDataB
    }
}
