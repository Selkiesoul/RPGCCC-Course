using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCCC.Attributes;
using UnityEngine.UI;

namespace RPGCCC.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health health = null;

        private void Update() 
        {
            health = GameObject.FindWithTag("Player").GetComponent<Fighter>().GetTarget();

            if (health == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            else
            {
                GetComponent<Text>().text = Mathf.RoundToInt(health.GetPercentHealth()) + "%";
            }
        }
    }
}
