using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UOP1.StateMachine;

public class OutOfBounds : MonoBehaviour
{

	[SerializeField] private float _fallHeightLimit = 4;
	[SerializeField] private float _fallOutLimit = -5;
	[SerializeField] private float timeCheck = 2;
	private CharacterController _characterController;
	private List<Transform> _respawnList;
	private float _fallStartY;

	public bool IsOutOfBounds
	{
		get;
		private set;
	}

	private void Awake()
	{
		getRespawnList();
		_characterController = GetComponent<CharacterController>();
	}

	private void getRespawnList()
	{
		_respawnList = GameObject.FindGameObjectsWithTag("Respawn").Select(ob => ob.transform).ToList();
	}

	public void StartCheckFallOut(float fallStartY)
	{
		_fallStartY = fallStartY;
		IsOutOfBounds = false;
		StartCoroutine("CheckFallOut");
	}
	public void PlayerRespawn()
	{

		/*
		 * for avoid disable the character controller
		 * https://forum.unity.com/threads/does-transform-position-work-on-a-charactercontroller.36149/
		 * Edit -> Project Settings -> Physics -> check Auto sync Transforms
		 */
		_characterController.enabled = false;
		Transform newtransform = FindNearPosition();
		transform.position = newtransform.position;
		transform.rotation = newtransform.rotation;
		_characterController.enabled = true;

	}

	private Transform FindNearPosition()
	{
		return (Transform)(_respawnList?.OrderBy(sp => Vector3.Distance(sp.position, transform.position)).First());
	}

	public void CheckHeightFall()
	{
		StopCoroutine("CheckFallOut");
		IsOutOfBounds |= _fallStartY - transform.position.y >= _fallHeightLimit;
	}


	private IEnumerator CheckFallOut()
	{
		while (true)
		{
			yield return new WaitForSeconds(timeCheck);
			IsOutOfBounds = transform.position.y <= _fallOutLimit;
			if (IsOutOfBounds)
			{
				PlayerRespawn();
			}
		}

	}

}
