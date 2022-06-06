using System.ComponentModel;

namespace Contact;

public struct ViewModelSetup
{
	[Category("Positioning")]
	public Vector3 PositionOffset { get; set; }

	[Category( "Positioning" )]
	public Angles RotationOffset { get; set; }

	[Category( "Sway" )]
	public float SwayWeight { get; set; }
	
	[Category( "Sway" )]
	public Vector2 MouseSwayScale { get; set; }
}
