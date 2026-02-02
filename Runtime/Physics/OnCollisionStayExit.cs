using UnityEngine;

namespace Utilities.Physics
{
    public class OnCollisionStayExit : CollisionHandler<Collision>
    {
        protected virtual void OnCollisionEnter(Collision other) => OnCollision.Invoke(other);
    }
}