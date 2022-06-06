namespace Contact;

public partial class ViewModel : BaseViewModel
{
	public BaseWeapon Weapon { get; set; }

	protected WeaponDefinition WeaponDef => Weapon?.WeaponDefinition;
	protected ViewModelSetup Setup => WeaponDef?.ViewModelSetup ?? default;

	protected Rotation NewRotation;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		var mouseX = Input.MouseDelta.x * Setup.MouseSwayScale.x;
		var mouseY = Input.MouseDelta.y * Setup.MouseSwayScale.y;

		var rotationX = Rotation.FromAxis( Vector3.Right, mouseY );
		var rotationY = Rotation.FromAxis( Vector3.Up, mouseX );
		var targetRotation = rotationX * rotationY;

		LocalPosition += Owner.EyeRotation.Forward * Setup.PositionOffset.x;
		LocalPosition += Owner.EyeRotation.Left * Setup.PositionOffset.y;
		LocalPosition += Owner.EyeRotation.Up * Setup.PositionOffset.z;

		LocalRotation *= Rotation.From( Setup.RotationOffset );

		NewRotation = Rotation.Slerp( NewRotation, targetRotation, Time.Delta * Setup.SwayWeight );
		LocalRotation *= NewRotation;
	}
}
