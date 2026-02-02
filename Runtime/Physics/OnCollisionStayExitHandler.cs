using UnityEngine;

namespace Utilities.Physic
{
    public class OnCollisionStayExitHandler : CollisionHandler<Collision>
    {
        protected virtual void OnCollisionEnter(Collision other) => OnCollision.Invoke(other);
    }
}