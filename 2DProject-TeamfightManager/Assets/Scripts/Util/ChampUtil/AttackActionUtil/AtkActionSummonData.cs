public enum SummonObjectType
{
	Champion,
	Projectile,
	Structure,
}

[System.Serializable]
public class AtkActionSummonData
{
	public SummonObjectType summonObjectType;
	public string summonObjectName;
	public bool isSummonOnce;
	public float tickTime;
}