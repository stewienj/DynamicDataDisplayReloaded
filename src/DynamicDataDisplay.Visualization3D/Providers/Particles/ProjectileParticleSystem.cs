using System;

namespace DynamicDataDisplay.Visualization3D.Particles
{
	class ProjectileParticleSystem : ParticleSystem
	{
		public ProjectileParticleSystem(DX3DHost host, Visualization3DDataSource dataSource) :
			base(host, dataSource)
		{

		}

		protected override void InitializeSettings(ParticleSettings settings)
		{
			settings.TextureName = "smoke.png";

			settings.MaxParticles = 1000;

			settings.Duration = TimeSpan.FromSeconds(0.2);

			settings.DurationRandomness = 1.5f;

			settings.EmitterVelocitySensitivity = 0.5f;

			settings.MinHorizontalVelocity = 0;
			settings.MaxHorizontalVelocity = 1;

			settings.MinVerticalVelocity = -1;
			settings.MaxVerticalVelocity = 1;

			settings.MinRotateSpeed = -4;
			settings.MaxRotateSpeed = 4;

			settings.MinStartSize = 3;
			settings.MaxStartSize = 3;

			settings.MinEndSize = 5;
			settings.MaxEndSize = 5;
		}
	}
}
