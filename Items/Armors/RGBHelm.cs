using ClickerClass.DrawLayers;
using ClickerClass.Projectiles;
using ClickerClass.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace ClickerClass.Items.Armors
{
	[AutoloadEquip(EquipType.Head)]
	public class RGBHelm : ClickerItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			if (!Main.dedServ)
			{
				HeadLayer.RegisterData(Item.headSlot, new DrawLayerData()
				{
					Texture = ModContent.Request<Texture2D>(Texture + "_Head_Glow"),
					Color = (PlayerDrawSet drawInfo) => new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 75) * 0.8f 
				});
			}
			
			ClickEffect.ChromaticBurst = ClickerSystem.RegisterClickEffect(Mod, "ChromaticBurst", null, null, 20, () => Color.Lerp(Color.White, Main.DiscoColor, 0.5f), delegate (Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, int type, int damage, float knockBack)
			{
				bool spawnEffects = true;
				int chromatic = ModContent.ProjectileType<RGBPro>();
				
				float total = 7f;
				int i = 0;
				while (i < total)
				{
					float hasSpawnEffects = spawnEffects ? 1f : 0f;
					Vector2 toDir = Vector2.UnitX * 0f;
					toDir += -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / total)) * new Vector2(10f, 10f);
					int damageAmount = (int)(damage * 0.33f);
					damageAmount = damageAmount < 1 ? 1 : damageAmount;
					Projectile.NewProjectile(source, Main.MouseWorld, toDir.SafeNormalize(Vector2.UnitY) * 10f, chromatic, damageAmount, 1f, Main.myPlayer, 0f, hasSpawnEffects);
					i++;
					spawnEffects = false;
				}
			});
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = 40000;
			Item.rare = 4;
			Item.defense = 5;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetModPlayer<ClickerPlayer>().clickerBonus += 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<RGBBreastplate>() && legs.type == ModContent.ItemType<RGBGreaves>();
		}

		public override void UpdateArmorSet(Player player)
		{
			ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();
			player.setBonus = LangHelper.GetText("SetBonus.RGB");
			clickerPlayer.setRGB = true;
			clickerPlayer.EnableClickEffect(ClickEffect.ChromaticBurst);
		}

		public override void UpdateVanitySet(Player player)
		{
			Lighting.AddLight(player.Center, Main.DiscoColor.ToVector3() * 0.75f);
		}
		
		public override void AddRecipes()
		{
			CreateRecipe(1).AddIngredient(ItemID.RainbowBrick, 25).AddIngredient(ItemID.CrystalShard, 15).AddTile(TileID.Anvils).Register();
		}
	}
}
