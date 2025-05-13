using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CRPG.DataManagement
{
	class LazyAddresablesCollection<T> : IDisposable
	{
		internal LazyAddresablesCollection(string label)
		{
			_label = label;
		}

		private string _label;
		private AsyncOperationHandle<IList<T>> _operationHandle;
		internal IList<T> Collection
		{
			get
			{
				if (!_operationHandle.IsValid())
				{
					_operationHandle = Addressables.LoadAssetsAsync<T>(_label);
				}
				if (!_operationHandle.IsDone)
				{
					return _operationHandle.WaitForCompletion();
				}
				else
				{
					return _operationHandle.Result;
				}
			}
		}

		public void Dispose()
		{
			_operationHandle.Release();
		}
	}
}