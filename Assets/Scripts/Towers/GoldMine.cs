using Abstract;
using UnityEngine;

namespace Towers
{
    public class GoldMine : ATower
    {
        [SerializeField] private int goldPerCycle;

        protected override void Start()
        {
            base.Start();

            target = transform; // set target to self, so target is never null
        }

        protected override void LoadShot()
        {
            GameManager.Instance.AddMoney(goldPerCycle, transform.position);
        }
    }
}