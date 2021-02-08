using UnityEngine;
using UOP1.StateMachine;
using UOP1.StateMachine.ScriptableObjects;

[CreateAssetMenu(menuName = "State Machines/Actions/Play Land Particles")]
public class PlayLandParticlesActionSO : StateActionSO<PlayLandParticlesAction> { }

public class PlayLandParticlesAction : StateAction
{
	//Component references
	private DustParticlesController _dustController;
	private Transform _transform;

	private float _coolDown = 0.3f;
	private float t = 0f;

	private float _fallStartY = 0f;
	private float _fallEndY = 0f;
	private float _maxFallDistance = 4f; //Used to adjust particle emission intensity
	private OutOfBounds _outOfBounds;
	public override void Awake(StateMachine stateMachine)
	{
		_dustController = stateMachine.GetComponent<DustParticlesController>();
		_outOfBounds = stateMachine.GetComponent<OutOfBounds>();
		_transform = stateMachine.transform;
	}

	public override void OnStateEnter()
	{
		_fallStartY = _transform.position.y;
		_outOfBounds.InitCheckFallOut(_fallStartY);
	}

	public override void OnStateExit()
	{
		_fallEndY = _transform.position.y;
		float dY = Mathf.Abs(_fallStartY - _fallEndY);
		if (!_outOfBounds.IsOutOfBounds)
		{
			float fallIntensity = Mathf.InverseLerp(0, _maxFallDistance, dY);

			if (Time.time >= t + _coolDown)
			{
				_dustController.PlayLandParticles(fallIntensity);
				t = Time.time;
			}
		}
		else
		{
			_outOfBounds.PlayerRespawn();
		}
	}

	public override void OnUpdate(){
		if (!_outOfBounds.IsOutOfBounds)
		{
			_outOfBounds.CheckFallBounds();
		}
	}
}
