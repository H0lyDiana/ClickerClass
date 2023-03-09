using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace ClickerClass.Projectiles
{
	public class TheClickerPro : ClickerProjectile
	{
		public override void SetDefaults()
		{
			base.SetDefaults();

			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.aiStyle = -1;
			Projectile.alpha = 255;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 10;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		
		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			//TODO use target.SuperArmor check?
			//Projectile is spawned with 1 damage, so it will always guarantee dealing 1 damage, and we subtract it
			int fixedDamage = (int)(target.lifeMax * 0.01f);
			modifiers.SourceDamage.Flat += fixedDamage - 1;
			modifiers.DamageVariationScale *= 0f;
		}

		public override void Kill(int timeLeft)
		{
			for (int l = 0; l < 7; l++)
			{
				int dustType = 86 + l;
				for (int k = 0; k < 5; k++)
				{
					if (dustType == 91)
					{
						Dust dust = Dust.NewDustDirect(Projectile.Center, 10, 10, 92, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 0, default, 1f);
						dust.shader = GameShaders.Armor.GetSecondaryShader(70, Main.LocalPlayer);
						dust.noGravity = true;
					}
					else
					{
						Dust dust = Dust.NewDustDirect(Projectile.Center, 10, 10, dustType, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 0, default, 1f);
						dust.noGravity = true;
					}
				}
			}
		}
	}
}
