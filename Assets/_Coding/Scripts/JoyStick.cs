// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
// public class PlayerController : MonoBehaviour
// {
//     [SerializeField] private Rigidbody _rigidbody;
//     [SerializeField] private FixedJoystick _joystick;
//     [SerializeField] private Animator _animator;

//     [SerializeField] private float _moveSpeed = 5f;

//     private void FixedUpdate()
//     {
//         _rigidbody.linearVelocity = new Vector3(
//             _joystick.Horizontal * _moveSpeed, 
//             _rigidbody.linearVelocity.y,                     
//             _joystick.Vertical * _moveSpeed            
//         );

//         // // Optional: Update Animator (Recommended)
//         // if (_animator != null)
//         // {
//         //     float moveAmount = new Vector2(_joystick.Horizontal, _joystick.Vertical).magnitude;
//         //     _animator.SetFloat("Speed", moveAmount);
//         // }
//     }
// }