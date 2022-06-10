using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGCCC.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text damageText = null;
        public void SetValue(float amount)
        {
            damageText.text = string.Format("{0:0}", amount); //this is a way of choosing how many decimal points to display
        }
    }

}