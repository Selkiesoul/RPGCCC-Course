using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGCCC.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            if (Mathf.Approximately(healthComponent.GetFraction(), 0) || Mathf.Approximately(healthComponent.GetFraction(), 1)) 
            // approximately must be used because float calculations are not exact and may vary with different cpus
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}
