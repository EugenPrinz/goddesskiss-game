using System.Collections.Generic;
using UnityEngine;

public class SWP_TimedGameObject : SWP_InternalTimedGameObject
{
	[SerializeField]
	public bool UseEventsForMecanim;

	private Animation aniLegacy;

	private Animator aniMecanim;

	private ParticleSystem psShuriken;

	private ParticleEmitter psLegacy;

	private Rigidbody rbRigidbody;

	private Rigidbody2D rbRigidbody2D;

	private AudioSource asAudio;

	private Vector3 vecSavedSpeed;

	private Vector2 vecSavedSpeed2D;

	private Vector3 vecSavedSpin;

	private float vecSavedSpin2D;

	public static Dictionary<int, SWP_InternalTimedGameObject> timedObjects;

	private void Awake()
	{
		if (timedObjects == null)
		{
			timedObjects = new Dictionary<int, SWP_InternalTimedGameObject>();
		}
		if (SearchObjects)
		{
			aniLegacy = GetComponent<Animation>();
			if (!UseEventsForMecanim)
			{
				aniMecanim = GetComponent<Animator>();
			}
			psShuriken = GetComponent<ParticleSystem>();
			psLegacy = GetComponent<ParticleEmitter>();
			rbRigidbody = GetComponent<Rigidbody>();
			rbRigidbody2D = GetComponent<Rigidbody2D>();
			asAudio = GetComponent<AudioSource>();
		}
	}

	private void OnEnable()
	{
		timedObjects.Add(GetInstanceID(), this);
	}

	private void OnDisable()
	{
		timedObjects.Remove(GetInstanceID());
	}

	protected override void ClearAssignedObjects()
	{
		aniLegacy = null;
		aniMecanim = null;
		psShuriken = null;
		psLegacy = null;
		rbRigidbody = null;
		rbRigidbody2D = null;
		asAudio = null;
	}

	protected override void SetSpeedLooping(float _fNewSpeed, float _fCurrentSpeedPercent, float _fCurrentSpeedZeroToOne)
	{
		if (AssignedObjects == null)
		{
			return;
		}
		for (int i = 0; i < AssignedObjects.Length; i++)
		{
			if (!(AssignedObjects[i] == null))
			{
				if (AssignedObjects[i].GetType() == typeof(Animation))
				{
					aniLegacy = (Animation)AssignedObjects[i];
				}
				else if (AssignedObjects[i].GetType() == typeof(Animator))
				{
					aniMecanim = (Animator)AssignedObjects[i];
				}
				else if (AssignedObjects[i].GetType() == typeof(ParticleSystem))
				{
					psShuriken = (ParticleSystem)AssignedObjects[i];
				}
				else if (AssignedObjects[i].GetType() == typeof(ParticleEmitter))
				{
					psLegacy = (ParticleEmitter)AssignedObjects[i];
				}
				else if (AssignedObjects[i].GetType() == typeof(Rigidbody))
				{
					rbRigidbody = (Rigidbody)AssignedObjects[i];
				}
				else if (AssignedObjects[i].GetType() == typeof(Rigidbody2D))
				{
					rbRigidbody2D = (Rigidbody2D)AssignedObjects[i];
				}
				else if (AssignedObjects[i].GetType() == typeof(AudioSource))
				{
					asAudio = (AudioSource)AssignedObjects[i];
				}
				SetSpeedAssigned(_fNewSpeed, _fCurrentSpeedPercent, _fCurrentSpeedZeroToOne);
			}
		}
		ClearAssignedObjects();
	}

	protected override void SetSpeedAssigned(float _fNewSpeed, float _fCurrentSpeedPercent, float _fCurrentSpeedZeroToOne)
	{
		if (aniLegacy != null)
		{
			foreach (AnimationState item in aniLegacy)
			{
				aniLegacy[item.name].speed = _fCurrentSpeedZeroToOne;
			}
		}
		if (aniMecanim != null)
		{
			aniMecanim.speed = _fCurrentSpeedZeroToOne;
		}
		if (psShuriken != null)
		{
			psShuriken.playbackSpeed = _fCurrentSpeedZeroToOne;
		}
		if (psLegacy != null)
		{
			if (_fNewSpeed == 0f)
			{
				psLegacy.enabled = false;
			}
			else
			{
				psLegacy.enabled = true;
				psLegacy.worldVelocity = new Vector3(GetNewSpeedFromPercentage(psLegacy.worldVelocity.x), GetNewSpeedFromPercentage(psLegacy.worldVelocity.y), GetNewSpeedFromPercentage(psLegacy.worldVelocity.z));
				psLegacy.localVelocity = new Vector3(GetNewSpeedFromPercentage(psLegacy.localVelocity.x), GetNewSpeedFromPercentage(psLegacy.localVelocity.y), GetNewSpeedFromPercentage(psLegacy.localVelocity.z));
				psLegacy.rndVelocity = new Vector3(GetNewSpeedFromPercentage(psLegacy.rndVelocity.x), GetNewSpeedFromPercentage(psLegacy.rndVelocity.y), GetNewSpeedFromPercentage(psLegacy.rndVelocity.z));
				psLegacy.emitterVelocityScale = GetNewSpeedFromPercentage(psLegacy.emitterVelocityScale);
				psLegacy.angularVelocity = GetNewSpeedFromPercentage(psLegacy.angularVelocity);
				psLegacy.rndAngularVelocity = GetNewSpeedFromPercentage(psLegacy.rndAngularVelocity);
				Particle[] particles = psLegacy.particles;
				for (int i = 0; i < particles.Length; i++)
				{
					particles[i].velocity = new Vector3(GetNewSpeedFromPercentage(particles[i].velocity.x), GetNewSpeedFromPercentage(particles[i].velocity.y), GetNewSpeedFromPercentage(particles[i].velocity.z));
				}
				psLegacy.particles = particles;
			}
		}
		if (rbRigidbody != null)
		{
			if (_fNewSpeed == 0f)
			{
				vecSavedSpeed = rbRigidbody.velocity;
				vecSavedSpin = rbRigidbody.angularVelocity;
				rbRigidbody.isKinematic = true;
			}
			else if (_fNewSpeed != 100f)
			{
				rbRigidbody.isKinematic = false;
				rbRigidbody.velocity = vecSavedSpeed;
				rbRigidbody.angularVelocity = vecSavedSpin;
			}
			else
			{
				rbRigidbody.isKinematic = false;
				rbRigidbody.velocity = vecSavedSpeed;
				rbRigidbody.angularVelocity = vecSavedSpin;
			}
		}
		if (rbRigidbody2D != null)
		{
			if (_fNewSpeed == 0f)
			{
				vecSavedSpeed2D = rbRigidbody2D.velocity;
				vecSavedSpin2D = rbRigidbody2D.angularVelocity;
				rbRigidbody2D.isKinematic = true;
			}
			else if (_fNewSpeed != 100f)
			{
				rbRigidbody2D.isKinematic = false;
				rbRigidbody2D.velocity = vecSavedSpeed2D;
				rbRigidbody2D.angularVelocity = vecSavedSpin2D;
			}
			else
			{
				rbRigidbody2D.isKinematic = false;
				rbRigidbody2D.velocity = vecSavedSpeed2D;
				rbRigidbody2D.angularVelocity = vecSavedSpin2D;
			}
		}
		if (asAudio != null)
		{
			asAudio.pitch = Mathf.Clamp(_fCurrentSpeedZeroToOne, 0.15f, 2f);
		}
	}
}
