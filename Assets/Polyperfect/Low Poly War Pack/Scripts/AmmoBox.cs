using System.Collections.Generic;

namespace PolyPerfect
{
    namespace War
    {
        public class AmmoBox : PickableObject
        {
            public static List<AmmoBox> ammoBoxes = new List<AmmoBox>();
            // Number of rounds in box
            public int count;
            // Type of ammo taht is in the box
            [AmmoType]
            public int ammoType;
            private void Awake()
            {
                ammoBoxes.Add(this);
            }
            public AmmoBox(int _count, int _ammoType)
            {
                count = _count;
                ammoType = _ammoType;
            }
            protected override void PickUp(IPickable player)
            {
                base.PickUp(player);
                player.PickAmmo(this);
                ammoBoxes.Remove(this);
            }
        }
    }
}