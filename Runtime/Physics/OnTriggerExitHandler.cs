using UnityEngine;

namespace Utilities.Physics
{
    public class OnTriggerExitHandler : CollisionHandler<Collider>
    {
        protected virtual void OnTriggerExit(Collider other) => OnCollision.Invoke(other);
    }
}