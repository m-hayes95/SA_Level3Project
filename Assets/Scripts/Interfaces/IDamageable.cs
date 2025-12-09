using UnityEngine;

namespace Interfaces
{
    public interface IDamageable
    {
        // Instigator = Who did the damage (used to ignore damage etc.)
        void Damage(GameObject instigator, float amount);
    }
}
