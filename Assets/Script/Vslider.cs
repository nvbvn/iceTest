using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vslider : MonoBehaviour
{
    [SerializeField]
    public Slider slider;

    private static Vslider instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void showValue(float val) {
        instance.slider.value = val;
    }
}
