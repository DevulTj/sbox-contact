namespace Contact;

public partial class PlayerCamera : CameraMode
{
	protected Vector3 CachedPosition;

	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Position = pawn.EyePosition;
		Rotation = pawn.EyeRotation;

		CachedPosition = Position;
	}

	public override void Update()
	{
		var pawn = Local.Pawn;
		if ( !pawn.IsValid() ) return;

		var eyePos = pawn.EyePosition;
		if ( eyePos.Distance( CachedPosition ) < 300f ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
		{
			Position = Vector3.Lerp( eyePos.WithZ( CachedPosition.z ), eyePos, 20f * Time.Delta );
		}
		else
		{
			Position = eyePos;
		}

		Rotation = pawn.EyeRotation;

		Viewer = pawn;
		CachedPosition = Position;
	}
}
