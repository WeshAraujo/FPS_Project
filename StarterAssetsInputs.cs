using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public float scroll;
		public bool jump;
		public bool sprint;
		public bool aiming;
		public bool shooting;
		public bool crouching;
		public bool paused;
		public bool reload;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		//public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
			else
			{
				LookInput(Vector2.zero);
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
            Debug.Log("LeuPulo");
        }

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}
        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }
		public void OnChangeWeapon(InputValue value)
		{
			ChangeWeaponInput(value.Get<float>());
		}
		public void OnCrouch(InputValue value)
		{
			CrouchInput(value.isPressed);
		}
        public void OnPause(InputValue value)
        {
			PauseInput(value.isPressed);
        }
		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}
#endif

		public void ChangeWeaponInput(float newChangeWeapon)
		{
			scroll = newChangeWeapon;
		}
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void AimInput(bool newAimState)
		{
			aiming = newAimState;
		}
		public void ShootInput(bool newShootState)
		{
			shooting = newShootState;
		}
		public void ScrollInput(float newScroll)
		{
			scroll = newScroll;
		}
		public void CrouchInput(bool newCrouch)
		{
			crouching = newCrouch;
		}
        public void PauseInput(bool newPause)
        {
            paused = newPause;
        }
        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }
    }