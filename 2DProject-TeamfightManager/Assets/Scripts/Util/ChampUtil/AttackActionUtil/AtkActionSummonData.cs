using UnityEngine;

public enum SummonObjectType
{
	Champion,
	Projectile,
	Structure,
	LivingObject
}

[System.Serializable]
public class AtkActionSummonData
{
	public SummonObjectType summonObjectType;
	public string summonObjectName;
	public bool isSummonOnce;
	public float tickTime;
	public Vector3 offsetPosition;
}