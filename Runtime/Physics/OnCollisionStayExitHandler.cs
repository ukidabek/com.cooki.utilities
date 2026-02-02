using UnityEngine;

namespace Utilities.Physics
{
    public class OnCollisionStayExitHandler : CollisionHandler<Collision>
    {
        protected virtual void OnCollisionEnter(Collision other) => OnCollision.Invoke(other);
    }
}