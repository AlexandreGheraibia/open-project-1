using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

	public void InitCheckFallOut(float fallStartY)
	{
		_fallStartY = fallStartY;
		IsOutOfBounds = false;
	}

	public void PlayerRespawn()
	{
		Transform newtransform = FindNearPosition();
		/*
		 * for avoid disable the character controller
		 * https://forum.unity.com/threads/does-transform-position-work-on-a-charactercontroller.36149/
		 * Edit -> Project Settings -> Physics -> check Auto sync Transforms
		 */
		_characterController.enabled = false;
		transform.position = newtransform.position;
		transform.rotation = newtransform.rotation;
		_characterController.enabled = true;

	}

	private Transform FindNearPosition()
	{
		return (Transform)(_respawnList?.OrderBy(sp => Vector3.Distance(sp.position, transform.position)).First()) ?? throw new Exception("No respawn set.");
	}

	public void CheckFallBounds()
	{
		IsOutOfBounds = transform.position.y <= _fallOutLimit || _fallStartY - transform.position.y >= _fallHeightLimit;
		if (IsOutOfBounds)
		{
			PlayerRespawn();
		}
	}

}
