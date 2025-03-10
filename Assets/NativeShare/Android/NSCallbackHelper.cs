#if UNITY_EDITOR || UNITY_ANDROID
using System.Collections;
using UnityEngine;

namespace NativeShareNamespace
{
	public class NSCallbackHelper : MonoBehaviour
	{
		public NativeShareLink.ShareResultCallback callback;

		private NativeShareLink.ShareResult result = NativeShareLink.ShareResult.Unknown;
		private string shareTarget = null;

		private bool resultReceived;

		private void Awake()
		{
			DontDestroyOnLoad( gameObject );
		}

		private void Update()
		{
			if( resultReceived )
			{
				resultReceived = false;

				try
				{
					if( callback != null )
						callback( result, shareTarget );
				}
				finally
				{
					Destroy( gameObject );
				}
			}
		}

		private IEnumerator OnApplicationFocus( bool focus )
		{
			if( focus )
			{
				// Share sheet is closed and now Unity activity is running again. Send Unknown result if OnShareCompleted wasn't called
				yield return null;
				resultReceived = true;
			}
		}

		public void OnShareCompleted( int resultRaw, string shareTarget )
		{
			NativeShareLink.ShareResult shareResult = (NativeShareLink.ShareResult) resultRaw;

			if( result == NativeShareLink.ShareResult.Unknown )
			{
				result = shareResult;
				this.shareTarget = shareTarget;
			}
			else if( result == NativeShareLink.ShareResult.NotShared )
			{
				if( shareResult == NativeShareLink.ShareResult.Shared )
				{
					result = NativeShareLink.ShareResult.Shared;
					this.shareTarget = shareTarget;
				}
				else if( shareResult == NativeShareLink.ShareResult.NotShared && !string.IsNullOrEmpty( shareTarget ) )
					this.shareTarget = shareTarget;
			}
			else
			{
				if( shareResult == NativeShareLink.ShareResult.Shared && !string.IsNullOrEmpty( shareTarget ) )
					this.shareTarget = shareTarget;
			}

			resultReceived = true;
		}
	}
}
#endif