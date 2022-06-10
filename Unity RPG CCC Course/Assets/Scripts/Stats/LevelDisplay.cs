using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGCCC.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;
        void Start()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        void Update()
        {
            GetComponent<Text>().text = "Level: " + baseStats.GetLevel().ToString();
        }
    }
}
