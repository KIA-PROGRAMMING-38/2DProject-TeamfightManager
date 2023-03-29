public interface IWalkable
{
	public float speed { get; set; }

	public void Move(UnityEngine.Vector3 direction);
	public void OnMoveEnd();
}
