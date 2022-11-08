using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Вызывается в момент, когда игрок взаимодействует с блокером.
        /// </summary>
        public event Action OnBlocked;

        [Header("Logic")]
        [SerializeField]
        [Tooltip("Вертикальная скорость игрока.")]
        private float speed = 1f;
        [SerializeField]
        [Tooltip("Слой на котором находятся блокеры.")]
        private int blockLayer;

        [Header("Components")]
        [SerializeField]
        private CharacterController controller;

        public float Speed
        {
            get { return currentSpeed; }
            set { currentSpeed = Math.Abs(value) * Math.Sign(currentSpeed); }
        }

        private float currentSpeed = 1f;
        private bool isJump;

        private void Awake()
        {
            currentSpeed = speed;
            isJump = false;
        }

        private void FixedUpdate()
        {
            var verticalMove =
                isJump ?
                Time.deltaTime * currentSpeed :
                - Time.deltaTime * currentSpeed;
            var move = new Vector3(0, verticalMove, 0);
            controller.Move(transform.TransformVector(move));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == blockLayer)
            {
                OnBlocked?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isJump = true;
            } else if (context.canceled)
            {
                isJump = false;
            }
        }

        public void ResetSpeed()
        {
            currentSpeed = speed;
            isJump = false;
        }
    }

}
