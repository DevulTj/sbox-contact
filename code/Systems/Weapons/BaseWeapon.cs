namespace Contact;

[Title( "Weapon" ), Icon( "luggage" )]
public partial class BaseWeapon : AnimatedEntity
{
	public virtual WeaponSlot Slot => WeaponSlot.Primary;
	public virtual string ViewModelPath => null;

	public BaseViewModel ViewModelEntity { get; protected set; }

	[Net, Change( nameof( OnWeaponDefinitionChanged ) )]
	protected WeaponDefinition _WeaponDefinition { get; set; }
	public WeaponDefinition WeaponDefinition
	{
		get
		{
			return _WeaponDefinition;
		}
		set
		{
			_WeaponDefinition = value;
			InitializeWeapon( _WeaponDefinition );
		}
	}

	protected void OnWeaponDefinitionChanged( WeaponDefinition oldDef, WeaponDefinition newDef )
	{
		InitializeWeapon( newDef );
	}

	protected void InitializeWeapon( WeaponDefinition def )
	{
		Log.Info( $"set up this weapon {def}" );

		if ( Host.IsClient )
		{
			CreateViewModel();
			Log.Info( def.CachedViewModel );
			ViewModelEntity.Model = def.CachedViewModel;
		}

		Model = def.CachedModel;
	}

	public override void Spawn()
	{
		base.Spawn();

		MoveType = MoveType.Physics;
		CollisionGroup = CollisionGroup.Interactive;
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public virtual bool CanCarry( Entity carrier )
	{
		return true;
	}

	public virtual void OnCarryStart( Entity carrier )
	{
		if ( IsClient ) return;

		SetParent( carrier, true );
		Owner = carrier;
		MoveType = MoveType.None;
		EnableAllCollisions = false;
		EnableDrawing = false;
	}

	public virtual void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 1 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
		anim.SetAnimParameter( "holdtype_handedness", 0 );
	}

	public virtual void OnCarryDrop( Entity dropper )
	{
		if ( IsClient ) return;

		SetParent( null );
		Owner = null;
		MoveType = MoveType.Physics;
		EnableDrawing = true;
		EnableAllCollisions = true;
	}

	/// <summary>
	/// This entity has become the active entity. This most likely
	/// means a player was carrying it in their inventory and now
	/// has it in their hands.
	/// </summary>
	public virtual void ActiveStart( Entity ent )
	{
		EnableDrawing = true;

		if ( ent is Player player )
		{
			var animator = player.GetActiveAnimator();
			if ( animator != null )
			{
				SimulateAnimator( animator );
			}
		}

		//
		// If we're the local player (clientside) create viewmodel
		// and any HUD elements that this weapon wants
		//
		if ( IsLocalPawn )
		{
			DestroyViewModel();
			DestroyHudElements();

			CreateViewModel();
			CreateHudElements();
		}
	}

	/// <summary>
	/// This entity has stopped being the active entity. This most
	/// likely means a player was holding it but has switched away
	/// or dropped it (in which case dropped = true)
	/// </summary>
	public virtual void ActiveEnd( Entity ent, bool dropped )
	{
		//
		// If we're just holstering, then hide us
		//
		if ( !dropped )
		{
			EnableDrawing = false;
		}

		if ( IsClient )
		{
			DestroyViewModel();
			DestroyHudElements();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( IsClient && ViewModelEntity.IsValid() )
		{
			DestroyViewModel();
			DestroyHudElements();
		}
	}

	/// <summary>
	/// Create the viewmodel. You can override this in your base classes if you want
	/// to create a certain viewmodel entity.
	/// </summary>
	public virtual void CreateViewModel()
	{
		Host.AssertClient();

		if ( !ViewModelEntity.IsValid() )
			ViewModelEntity = new();

		ViewModelEntity.Position = Position;
		ViewModelEntity.Owner = Owner;
		ViewModelEntity.EnableViewmodelRendering = true;
	}

	/// <summary>
	/// We're done with the viewmodel - delete it
	/// </summary>
	public virtual void DestroyViewModel()
	{
		ViewModelEntity?.Delete();
		ViewModelEntity = null;
	}

	public virtual void CreateHudElements()
	{
	}

	public virtual void DestroyHudElements()
	{

	}

	/// <summary>
	/// Utility - return the entity we should be spawning particles from etc
	/// </summary>
	public virtual ModelEntity EffectEntity => (ViewModelEntity.IsValid() && IsFirstPersonMode) ? ViewModelEntity : this;

}
