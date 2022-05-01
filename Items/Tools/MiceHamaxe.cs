using ClickerClass.DrawLayers;
using ClickerClass.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClickerClass.Items.Tools
{
	public class MiceHamaxe : ModItem
	{
		public static Asset<Texture2D> glowmask;

		public override void Unload()
		{
			glowmask = null;
		}

		public override void SetStaticDefaults()
		{
			if (!Main.dedServ)
			{
				glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");

				HeldItemLayer.RegisterData(Item.type, new DrawLayerData()
				{
					Texture = glowmask,
					Color = (PlayerDrawSet drawInfo) => new Color(255, 255, 255, 50) * 0.7f
				});
			}

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 60;
			Item.DamageType = DamageClass.Melee;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 7;
			Item.useAnimation = 28;
			Item.tileBoost = 4;
			Item.axe = 30;
			Item.hammer = 100;
			Item.useStyle = 1;
			Item.knockBack = 7f;
			Item.rare = 10;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
		}

		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, 0.1f, 0.1f, 0.3f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Item.BasicInWorldGlowmask(spriteBatch, glowmask.Value, new Color(255, 255, 255, 50) * 0.7f, rotation, scale);
		}

		public override void AddRecipes()
		{
			CreateRecipe(1).AddIngredient(ModContent.ItemType<MiceFragment>(), 14).AddIngredient(ItemID.LunarBar, 12).AddTile(TileID.LunarCraftingStation).Register();
		}
	}
}
