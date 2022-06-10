using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGCCC.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        
        private void Awake() 
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update() 
        {
            GetComponent<Text>().text = experience.GetPoints().ToString();
        }
    }
}
