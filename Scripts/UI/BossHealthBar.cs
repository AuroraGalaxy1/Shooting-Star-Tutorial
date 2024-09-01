using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : StatsBar_HUD
{
    protected override void SetPercentText()
    {
        percentText.text = targetFillAmount.ToString("P");
        //percentText.text = (targetFillAmount * 100f).ToString("f2") + "%";
    }
}
