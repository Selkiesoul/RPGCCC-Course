using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCCC.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        void Start()
        {
            
        }

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate<DamageText>(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }

}