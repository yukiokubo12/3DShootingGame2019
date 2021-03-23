using UnityEngine;
namespace PolyPerfect
{
    namespace War
    {

        [CreateAssetMenu(menuName = "PolyPerfect/Aerodynamic Curves")]
        public class AerodynamicCurves : ScriptableObject
        {
            // Lift force coefficient for AoA (Angle of Attack)
            public AnimationCurve liftCurve;
            // Drag force coefficient for AoA (Angle of Attack)
            public AnimationCurve dragCurve;
        }
    }
}
