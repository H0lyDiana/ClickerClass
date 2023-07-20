using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ClickerClass.Items.Weapons.Clickers
{
	public class CopperClicker : ClickerWeapon
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			SetRadius(Item, 1.1f);
			SetColor(Item, new Color(255, 150, 75));
			SetDust(Item, 9);
			AddEffect(Item, ClickEffect.DoubleClick);

			Item.damage = 3;
			Item.width = 30;
			Item.height = 30;
			Item.knockBack = 0.5f;
			Item.value = Item.sellPrice(0, 0, 0, 90);
			Item.rare = ItemRarityID.White;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1).AddIngredient(ItemID.CopperBar, 8).AddTile(TileID.Anvils).Register();
		}
	}
}
