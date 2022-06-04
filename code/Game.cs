global using Sandbox;
global using System;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Collections.Generic;

namespace Contact;

public partial class Game : Sandbox.Game
{
	public static new Game Current => Sandbox.Game.Current as Game;

	public Game()
	{
	}

	protected void SetupDefaultPawn( Client client )
	{
		client.Pawn?.Delete();

		// Create a pawn for this client to play with
		var pawn = new Player();
		client.Pawn = pawn;

		pawn.Respawn();
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		SetupDefaultPawn( client );
	}

	[ConCmd.Server( "contact_respawn" )]
	public static void ForceRespawn()
	{
		var cl = ConsoleSystem.Caller;

		Current.SetupDefaultPawn( cl );
	}
}
