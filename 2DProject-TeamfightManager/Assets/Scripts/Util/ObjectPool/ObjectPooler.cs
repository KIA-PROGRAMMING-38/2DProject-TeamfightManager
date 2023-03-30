using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util.Pool
{
	public class ObjectPooler<T> : IDisposable, IObjectPool<T> where T : class
	{
		private Queue<T> _objectContainer;		// ���� ��ü�� ������ �����̳�..
		private Func<T> _createFunction;        // ��ü�� ������ �� ������ �ݹ�..
		private Action<T> _actionOnGet;         // Get() �޼ҵ尡 ȣ��� �� ������ �ݹ�..
		private Action<T> _actionOnRelease;     // Release() �޼ҵ尡 ȣ��� �� ������ �ݹ�..
		private Action<T> _actionOnDestroy;		// ��ü�� ������ �� �޼ҵ尡 ȣ��� �� ������ �ݹ�..

		private int _maxSize = 0;

		public int CountAll { get; private set; }
		public int CountInactive { get { return _objectContainer.Count; } }
		public int CountActive { get { return CountAll - CountInactive; } }

		public ObjectPooler( Func<T> cretaeFunc, Action<T> actionInGet = null, Action<T> actionInRelease = null, Action<T> actionInDestroy = null, int defaultCapacity = 10, int defaultCreateCount = 10, int maxSize = 10000 )
		{
			if ( null == cretaeFunc)
			{
				throw new ArgumentNullException( "createFunction" );
			}

			if(0 >= maxSize)
			{
				throw new ArgumentException( "Max size nust be greater than 0" );
			}

			_maxSize = maxSize;

			_createFunction = cretaeFunc;
			_actionOnGet = actionInGet;
			_actionOnRelease = actionInRelease;
			_objectContainer = new Queue<T>( defaultCapacity );

			CreatePoolObject( defaultCreateCount );
		}

		private void CreatePoolObject( int createCount )
		{
			for ( int i = 0; i < createCount; ++i )
			{
				Release( _createFunction() );
				++CountAll;
			}
		}

		public void Clear()
		{
			if ( null != _actionOnDestroy )
			{
				foreach ( T item in _objectContainer )
				{
					_actionOnDestroy( item );
				}
			}

			_objectContainer.Clear();
			CountAll = 0;
		}

		public T Get()
		{
			if ( _objectContainer.Count == 0 )
			{
				CreatePoolObject( 1 );
			}

			T outObject = _objectContainer.Dequeue();
			Debug.Assert( null != outObject );

			_actionOnGet?.Invoke( outObject );

			return outObject;
		}

		public void Release( T element )
		{
			Debug.Assert( null != element );

			_actionOnRelease?.Invoke( element );

			if ( CountInactive < _maxSize )
			{
				_objectContainer.Enqueue( element );
			}
			else
			{
				_actionOnDestroy?.Invoke( element );
			}
		}

		public void Dispose()
		{
			Clear();
		}

		public PooledObject<T> Get( out T value )
		{
			return new PooledObject<T>( value = Get(), this );
		}
	}
}