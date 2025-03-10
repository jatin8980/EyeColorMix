#if UNITY_EDITOR || UNITY_IOS
using UnityEngine;

namespace NativeShareNamespace
{
	public class NSShareResultCallbackiOS : MonoBehaviour
	{
		private static NSShareResultCallbackiOS instance;
		private NativeShareLink.ShareResultCallback callback;

		public static void Initialize( NativeShareLink.ShareResultCallback callback )
		{
			if( instance == null )
			{
				instance = new GameObject( "NSShareResultCallbackiOS" ).AddComponent<NSShareResultCallbackiOS>();
				DontDestroyOnLoad( instance.gameObject );
			}
			else if( instance.callback != null )
				instance.callback( NativeShareLink.ShareResult.Unknown, null );

			instance.callback = callback;
		}

		[UnityEngine.Scripting.Preserve]
		public void OnShareCompleted( string message )
		{
			NativeShareLink.ShareResultCallback _callback = callback;
			callback = null;

			if( _callback != null )
			{
				if( string.IsNullOrEmpty( message ) )
					_callback( NativeShareLink.ShareResult.Unknown, null );
				else
				{
					NativeShareLink.ShareResult result = (NativeShareLink.ShareResult) ( message[0] - '0' ); // Convert first char to digit and then to ShareResult
					string shareTarget = message.Length > 1 ? message.Substring( 1 ) : null;

					_callback( result, shareTarget );
				}
			}
		}
	}
}
#endif