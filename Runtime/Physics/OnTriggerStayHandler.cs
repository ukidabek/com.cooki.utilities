using UnityEngine;

namespace Utilities.Physics
{
    public class OnTriggerStayHandler : CollisionHandler<Collider>
    {
        protected virtual void OnTriggerStay(Collider other) => OnCollision.Invoke(other);
    }
}