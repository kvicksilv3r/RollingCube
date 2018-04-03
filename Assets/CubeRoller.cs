using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CubeRoller : MonoBehaviour
{

	[SerializeField]
	[Range(0.1f, 2)]
	[Tooltip("Time in seconds")]
	private float rollingSpeed;
	float actualRollSpeed;
	bool isRolling = false;
	Vector3 inputVec, inputCopy;
	Quaternion endRotation;
	Vector3 endPosition;
	float cubeExtent;
	Vector3 rotationPoint;
	public bool debug;
	Vector3 rotationAxis;
	float rotationChange;
	float rotationAmount;
	Vector3 traceEnd;
	Ray ray;
	RaycastHit rayHit;

	// Use this for initialization
	void Start()
	{
		cubeExtent = GetComponent<MeshFilter>().mesh.bounds.extents.x;
	}

	// Update is called once per frame
	void Update()
	{
		if (!isRolling)
		{
			if (Input.GetAxisRaw("Horizontal") != 0)
			{
				inputVec = (Vector3.right * Input.GetAxisRaw("Horizontal")).normalized;
				CheckMovement();
			}

			else if (Input.GetAxisRaw("Vertical") != 0)
			{
				inputVec = (Vector3.forward * Input.GetAxisRaw("Vertical")).normalized;
				CheckMovement();
			}
		}
	}

	IEnumerator RollCube()
	{
		rotationAxis = Vector3.Cross(Vector3.up, inputVec);

		endRotation = Quaternion.AngleAxis(rotationAmount, rotationAxis) * transform.rotation;

		isRolling = true;

		rotationChange = 0;

		while (Mathf.Abs(rotationChange) <= rotationAmount)
		{
			float _r = (rotationAmount / actualRollSpeed) * Time.deltaTime;
			rotationChange += _r;
			transform.RotateAround(rotationPoint, rotationAxis, _r);

			yield return new WaitForSeconds(0);
		}

		transform.rotation = endRotation;
		transform.position = endPosition;
		isRolling = false;
	}

	//Checks for objects that are in the way for the cube 
	void CheckMovement()
	{
		inputCopy = inputVec;
		//if there is anyting above the cube Dont move.
		if (!RaycastUp(transform.position))
		{
			if (!RaycastFlat(traceEnd))
			{
				if (!RaycastDown(traceEnd))
				{
					if (!RaycastDown(traceEnd))
					{
						if (RaycastDown(traceEnd))
						{
							if (!RaycastFlat(traceEnd))
							{
								if (!RaycastUp(traceEnd))
								{
									RollDescend();
								}
							}
						}
					}
					else
					{
						RollFlat();
					}
				}
				else
				{
					if (!RaycastUp(traceEnd))
					{
						if (!RaycastFlatReverse(transform.position))
						{
							RollClimb();
						}
					}
				}
			}
		}
	}

	void RollFlat()
	{
		print("Roll Flat");
		rotationAmount = 90f;
		rotationPoint = transform.position - (Vector3.up * cubeExtent) + (inputCopy * cubeExtent);
		endPosition = transform.position + inputCopy * (2 * cubeExtent);
		actualRollSpeed = rollingSpeed;
		StartCoroutine("RollCube");
	}

	void RollClimb()
	{
		print("Roll Climb");
		rotationAmount = 180f;
		rotationPoint = transform.position + (Vector3.up * cubeExtent) + (inputCopy * cubeExtent);
		endPosition = transform.position + (inputCopy * 2 * cubeExtent) + (2 * cubeExtent * Vector3.up);
		actualRollSpeed = 2 * rollingSpeed;
		StartCoroutine("RollCube");
	}

	void RollDescend()
	{
		print("Roll Descend");
		rotationAmount = 180f;
		rotationPoint = transform.position - (Vector3.up * cubeExtent) + (inputCopy * cubeExtent);
		endPosition = transform.position + (inputCopy * 2 * cubeExtent) - (2 * cubeExtent * Vector3.up);
		actualRollSpeed = 1.75f * rollingSpeed;
		StartCoroutine("RollCube");
	}

	bool RaycastFlat(Vector3 startPos)
	{
		if (debug)
		{
			Debug.DrawLine(startPos, startPos + inputCopy * cubeExtent * 2, Color.red, 2f);
		}

		if (Physics.Raycast(startPos, inputCopy, out rayHit, cubeExtent * 2))
		{
			return true;
		}
		else
		{
			traceEnd = startPos + (cubeExtent * 2 * inputCopy);
			return false;
		}
	}

	bool RaycastFlatReverse(Vector3 startPos)
	{
		if (debug)
		{
			Debug.DrawLine(startPos, startPos - inputCopy * cubeExtent * 2, Color.red, 2f);
		}

		if (Physics.Raycast(startPos, -inputCopy, out rayHit, cubeExtent * 2))
		{
			return true;
		}
		else
		{
			traceEnd = startPos - (cubeExtent * 2 * inputCopy);
			return false;
		}
	}

	bool RaycastDown(Vector3 startPos)
	{
		if (debug)
		{
			Debug.DrawLine(startPos, startPos + Vector3.down * cubeExtent * 2, Color.red, 2f);
		}

		if (Physics.Raycast(startPos, Vector3.down, out rayHit, cubeExtent * 2))
		{
			return true;
		}
		else
		{
			traceEnd = startPos + Vector3.down * cubeExtent * 2;
			return false;
		}
	}

	bool RaycastUp(Vector3 startPos)
	{
		if (debug)
		{
			Debug.DrawLine(startPos, startPos + Vector3.up * cubeExtent * 2, Color.red, 2f);
		}

		if (Physics.Raycast(startPos, Vector3.up, out rayHit, cubeExtent * 2))
		{
			return true;
		}
		else
		{
			traceEnd = startPos + Vector3.up * cubeExtent * 2;
			return false;
		}
	}
}
