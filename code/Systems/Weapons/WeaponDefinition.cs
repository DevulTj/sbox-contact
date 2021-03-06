using System.ComponentModel;

namespace Contact;

[GameResource( "Weapon Definition", "cwpn", "" )]
public partial class WeaponDefinition : GameResource
{
	public static BaseWeapon CreateWeapon( WeaponDefinition def )
	{
		var weapon = new BaseWeapon();
		weapon.WeaponDefinition = def;

		return weapon;
	}

	public static BaseWeapon CreateWeapon( string identifier )
	{
		if ( Index.TryGetValue( identifier, out var weaponDef ) )
			return CreateWeapon( weaponDef );

		return null;
	}

	public static Dictionary<string, WeaponDefinition> Index = new();
	public static List<WeaponDefinition> All = new();

	[Category( "Setup" )]
	public string WeaponName { get; set; } = "Weapon";

	[Category( "Setup" )]
	public string WeaponShortName { get; set; } = "";

	[Category( "Setup" )]
	public WeaponSlot Slot { get; set; } = WeaponSlot.Primary;

	[Category( "Setup" ), ResourceType( "vmdl" )]
	public string Model { get; set; }

	public Model CachedModel;

	[Category( "Setup" ), ResourceType( "vmdl" )]
	public string ViewModel { get; set; }

	public Model CachedViewModel;

	[Category( "Setup" ), ResourceType( "jpg" )]
	public string Icon { get; set; }

	[Category( "View Model" )]
	public ViewModelSetup ViewModelSetup { get; set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		Log.Info( $"Contact: Registering weapon definition ({Name}, {WeaponName})" );

		if ( !All.Contains( this ) )
			All.Add( this );

		if ( !Index.ContainsKey( Name ) )
			Index.Add( Name, this );

		if ( !string.IsNullOrEmpty( Model ) )
			CachedModel = Sandbox.Model.Load( Model );

		if ( !string.IsNullOrEmpty( ViewModel ) )
			CachedViewModel = Sandbox.Model.Load( ViewModel );
	}
}
