using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ClickerClass.Items.Consumables
{
	public class ClickerEffectHairDye : HairDyeBase
	{
		public override Color LegacyShaderMethod(Player player, Color newColor, ref bool lighting)
		{
			ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();
			if (clickerPlayer.clickerSelected)
			{
				Color color = clickerPlayer.clickerRadiusColor;
				return color;
			}

			return newColor;
		}

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(230, 230, 200),
				new Color(250, 250, 250),
				new Color(255, 255, 255)
			};
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			//TODO price/rarity
			Item.rare = 2;
			Item.value = Item.buyPrice(0, 5);
		}
	}
}
