namespace PolyPerfect
{
    namespace War
    {
        public interface IPickable
        {
            void PickAmmo(AmmoBox ammo);
            void PickUpMedkit(Medkit medkit);
            float currentHealth { get; set; }
            float health { get; set; }
        }
    }
}
