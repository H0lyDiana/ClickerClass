using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace ClickerClass.Items.Weapons.Clickers
{
	public class PlatinumClicker : ClickerWeapon
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			SetRadius(Item, 1.65f);
			SetColor(Item, new Color(125, 150, 175));
			SetDust(Item, 11);
			AddEffect(Item, ClickEffect.DoubleClick2);

			Item.damage = 5;
			Item.width = 30;
			Item.height = 30;
			Item.knockBack = 1f;
			Item.value = 13500;
			Item.rare = 0;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1).AddIngredient(ItemID.PlatinumBar, 8).AddTile(TileID.Anvils).Register();
		}
	}
}
