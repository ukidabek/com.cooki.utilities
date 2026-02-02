using UnityEngine;

namespace Utilities.Physic
{
    public class OnCollisionEnterHandler : CollisionHandler<Collision>
    {
        protected virtual void OnCollisionEnter(Collision other) => OnCollision.Invoke(other);
    }
}