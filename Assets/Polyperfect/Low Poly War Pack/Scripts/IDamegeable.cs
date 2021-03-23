using UnityEngine;
namespace PolyPerfect
{
    namespace War
    {
        public interface IDamageable<T>
        {
            void TakeDamage(T damage);
        }
    }
}
