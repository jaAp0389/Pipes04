//29.5

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    /// <summary>
    /// Manages waterStartSlider
    /// </summary>
    public class SliderManager : MonoBehaviour
    {
        Slider slider;
        void Start()
        {
            slider = this.gameObject.GetComponent<Slider>();
        }
        public bool DecreaseSlider(float sliderSpeed)
        {
            slider.value -= sliderSpeed;
            if (slider.value <= 0)
                return true;
            return false;
        }
        public void ResetSlider()
        {
            slider.value = 1;
        }
        public void DestroySlider()
        {
            Destroy(this.gameObject);
        }
    }

}