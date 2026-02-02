using UnityEngine;

namespace Utilities.Physic
{
    public class OnTriggerStayHandler : CollisionHandler<Collider>
    {
        protected virtual void OnTriggerStay(Collider other) => OnCollision.Invoke(other);
    }
}