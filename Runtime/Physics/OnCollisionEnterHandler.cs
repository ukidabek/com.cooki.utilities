using UnityEngine;

namespace Utilities.Physics
{
    public class OnCollisionEnterHandler : CollisionHandler<Collision>
    {
        protected virtual void OnCollisionEnter(Collision other) => OnCollision.Invoke(other);
    }
}