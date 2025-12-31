using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    private Property currentproperty;

    public Image healthImage;
    public Image powerImage;

    private bool isReaviveing;


    private void Update()
    {
        if (isReaviveing)
        {
            float percentage = currentproperty.currentSlidePower / currentproperty.maxSlidePower;
            powerImage.fillAmount = percentage;

            if(percentage >= 1)
            {
                isReaviveing = false;
                return;
            }
        }
    }

    ///<summary>
    ///接收血量百分比
    ///</summary>
    ///<porm name="percentage">百分比:current/max</pormname>
    public void OnHealthChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }

    public void OnPowerChange(Property property)
    {
        isReaviveing = true;
        currentproperty = property;
    }
}
