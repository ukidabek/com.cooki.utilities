using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.Physic
{
    public class CollisionHandler<T> : MonoBehaviour
    {
        public UnityEvent<T> OnCollision;
    }
}