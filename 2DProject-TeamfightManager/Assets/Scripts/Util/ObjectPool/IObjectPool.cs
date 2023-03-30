namespace Util.Pool
{
	public interface IObjectPool<T> where T : class
	{
		// Pool에 존재하는 객체의 개수..
		int CountInactive { get; }

		// Pool에서 객체를 가져온다..
		T Get();
		PooledObject<T> Get( out T value );

		// Pool에 객체를 반환한다..
		void Release( T element );

		// Pool을 정리한다..
		void Clear();
	}
}
