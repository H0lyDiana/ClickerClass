﻿using ClickerClass.Buffs;
using ClickerClass.Dusts;
using ClickerClass.Prefixes;
using ClickerClass.Projectiles;
using ClickerClass.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;

namespace ClickerClass.Items
{
	/// <summary>
	/// The class responsible for any clicker item related logic
	/// </summary>
	public class ClickerItemCore : GlobalItem
	{
		public override bool InstancePerEntity => true;

		/// <summary>
		/// A clickers color used for the radius
		/// </summary>
		public Color clickerRadiusColor = Color.White;

		/// <summary>
		/// The clickers effects
		/// </summary>
		public List<string> itemClickEffects = new List<string>();

		/// <summary>
		/// The clickers dust that is spawned on use
		/// </summary>
		public int clickerDustColor = 0;

		/// <summary>
		/// Displays total clicks in the tooltip
		/// </summary>
		public bool isClickerDisplayTotal = false;
		
		/// <summary>
		/// Displays total money generated by given item
		/// </summary>
		public bool isClickerDisplayMoneyGenerated = false;

		/// <summary>
		/// Additional range for this clicker (1f = 100 pixel, 1f by default from the player)
		/// </summary>
		public float radiusBoost = 0f;

		internal float radiusBoostPrefix = 0f;
		internal int clickBoostPrefix = 0;

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			ClickerItemCore myClone = (ClickerItemCore)base.Clone(item, itemClone);
			myClone.clickerRadiusColor = clickerRadiusColor;
			myClone.itemClickEffects = new List<string>(itemClickEffects);
			myClone.clickerDustColor = clickerDustColor;
			myClone.clickBoostPrefix = clickBoostPrefix;
			myClone.isClickerDisplayTotal = isClickerDisplayTotal;
			myClone.isClickerDisplayMoneyGenerated = isClickerDisplayMoneyGenerated;
			myClone.radiusBoost = radiusBoost;
			myClone.radiusBoostPrefix = radiusBoostPrefix;
			return myClone;
		}

		public override float UseTimeMultiplier(Item item, Player player)
		{
			ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();
			
			if (ClickerSystem.IsClickerWeapon(item))
			{
				if (!player.HasBuff(ModContent.BuffType<AutoClick>()))
				{
					//if (player.GetModPlayer<ClickerPlayer>().clickerAutoClick || item.autoReuse) //item.autoReuse: Hard OmniSwing incompatibility
					if (player.UsingAutoswingableItem(item))
					{
						if (clickerPlayer.accHandCream)
						{
							return 6f;
						}
						else if (clickerPlayer.accIcePack)
						{
							return 8f;
						}
						else
						{
							return 10f;
						}
					}
					else
					{
						return 1f;
					}
				}
				else
				{
					return 3f;
				}
			}

			return base.UseTimeMultiplier(item, player);
		}

		public override bool? CanAutoswing(Item item, Player player)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();
				if (clickerPlayer.clickerAutoClick || player.HasBuff(ModContent.BuffType<AutoClick>()))
				{
					return true;
				}
			}
			return base.CanAutoswing(item, player);
		}

		public override bool CanUseItem(Item item, Player player)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();

				if (!clickerPlayer.HasClickEffect(ClickEffect.PhaseReach))
				{
					//collision
					Vector2 motherboardPosition = clickerPlayer.setMotherboardPosition;
					float radiusSQ = clickerPlayer.ClickerRadiusReal;
					radiusSQ *= radiusSQ;
					bool inRange = Vector2.DistanceSquared(Main.MouseWorld, player.Center) < radiusSQ && Collision.CanHit(new Vector2(player.Center.X, player.Center.Y - 12), 1, 1, Main.MouseWorld, 1, 1);
					radiusSQ = clickerPlayer.ClickerRadiusMotherboard;
					radiusSQ *= radiusSQ;
					bool inRangeMotherboard = Vector2.DistanceSquared(Main.MouseWorld, motherboardPosition) < radiusSQ && Collision.CanHit(motherboardPosition, 1, 1, Main.MouseWorld, 1, 1);
					//bool allowMotherboard = player.GetModPlayer<ClickerPlayer>().clickerMotherboardSet && player.altFunctionUse == 2;

					if (inRange || (inRangeMotherboard && player.altFunctionUse != 2))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			return base.CanUseItem(item, player);
		}

		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage, ref float flat)
		{
			if (item.CountsAsClass<ClickerDamage>())
			{
				ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();
				flat += clickerPlayer.clickerDamageFlat;

				if (clickerPlayer.accPortableParticleAccelerator && clickerPlayer.accPortableParticleAccelerator2)
				{
					flat += 8;
				}
			}
		}

		private bool HasAltFunctionUse(Item item, Player player)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				ClickerPlayer clickerPlayer = player.GetModPlayer<ClickerPlayer>();
				return clickerPlayer.setMice || clickerPlayer.setMotherboard;
			}
			return false;
		}

		public override bool AltFunctionUse(Item item, Player player)
		{
			return HasAltFunctionUse(item, player);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (ClickerSystem.IsClickerItem(item))
			{
				Player player = Main.LocalPlayer;
				var clickerPlayer = player.GetModPlayer<ClickerPlayer>();
				int index;

				float alpha = Main.mouseTextColor / 255f;

				if (ClickerConfigClient.Instance.ShowClassTags)
				{
					index = tooltips.FindIndex(tt => tt.mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
					if (index != -1)
					{
						tooltips.Insert(index + 1, new TooltipLine(Mod, "ClickerTag", $"-{LangHelper.GetText("Tooltip.ClickerTag")}-")
						{
							overrideColor = Main.DiscoColor
						});
					}
				}

				if (isClickerDisplayTotal)
				{
					index = tooltips.FindLastIndex(tt => tt.mod.Equals("Terraria") && tt.Name.StartsWith("Tooltip"));

					if (index != -1)
					{
						string color = (new Color(252, 210, 44) * alpha).Hex3();
						tooltips.Insert(index + 1, new TooltipLine(Mod, "TransformationText", $"{LangHelper.GetText("Tooltip.TotalClicks")}: [c/{color}:{clickerPlayer.clickerTotal}]"));
					}
				}
				
				if (isClickerDisplayMoneyGenerated)
				{
					index = tooltips.FindLastIndex(tt => tt.mod.Equals("Terraria") && tt.Name.StartsWith("Tooltip"));

					if (index != -1)
					{
						int currentValue = clickerPlayer.clickerMoneyGenerated;
						string displayValue = " " + PopupText.ValueToName(currentValue);
						string color = (new Color(252, 210, 44) * alpha).Hex3();
						tooltips.Insert(index + 1, new TooltipLine(Mod, "TransformationText", $"{LangHelper.GetText("Tooltip.MoneyGenerated")}:[c/{color}:" + displayValue + "]"));
					}
				}

				if (ClickerSystem.IsClickerWeapon(item))
				{
					TooltipLine tooltip = tooltips.Find(tt => tt.mod.Equals("Terraria") && tt.Name.Equals("Damage"));
					if (tooltip != null)
					{
						string number = tooltip.text.Split(' ')[0];
						tooltip.text = LangHelper.GetText("Tooltip.ClickDamage", number);
					}

					//Show the clicker's effects
					//Then show ones missing through the players enabled effects (respecting overlap, ignoring the currently held clickers effect if its not the same type)
					List<string> effects = new List<string>(itemClickEffects);
					foreach (var name in ClickerSystem.GetAllEffectNames())
					{
						if (clickerPlayer.HasClickEffect(name, out ClickEffect effect) && !effects.Contains(name))
						{
							if (!(player.HeldItem.type != item.type && player.HeldItem.type != ItemID.None && player.HeldItem.GetGlobalItem<ClickerItemCore>().itemClickEffects.Contains(name)))
							{
								effects.Add(name);
							}
						}
					}

					if (effects.Count > 0)
					{
						index = tooltips.FindIndex(tt => tt.mod.Equals("Terraria") && tt.Name.Equals("Knockback"));

						if (index != -1)
						{
							var keys = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[TriggerNames.SmartSelect];
							string key = keys.Count == 0 ? null : keys[0];

							//If has a key, but not pressing it, show the ForMoreInfo text
							//Otherwise, list all effects

							//No tml hooks between controlTorch getting set, and then reset again in SmartSelectLookup, so we have to use the raw data from PlayerInput
							bool showDesc = key == null || PlayerInput.Triggers.Current.SmartSelect;

							foreach (var name in effects)
							{
								if (ClickerSystem.IsClickEffect(name, out ClickEffect effect))
								{
									tooltips.Insert(++index, effect.ToTooltip(clickerPlayer.GetClickAmountTotal(this, name), alpha, showDesc));
								}
							}

							if (!showDesc && ClickerConfigClient.Instance.ShowEffectSuggestion)
							{
								//Add ForMoreInfo as the last line
								index = tooltips.FindLastIndex(tt => tt.mod.Equals("Terraria") && tt.Name.StartsWith("Tooltip"));
								var ttl = new TooltipLine(Mod, "ForMoreInfo", LangHelper.GetText("Tooltip.ForMoreInfo", key))
								{
									overrideColor = Color.Gray
								};

								if (index != -1)
								{
									tooltips.Insert(++index, ttl);
								}
								else
								{
									tooltips.Add(ttl);
								}
							}
						}
					}
				}
				
				if (item.prefix < PrefixID.Count || !ClickerPrefix.ClickerPrefixes.Contains(item.prefix))
				{
					return;
				}

				int ttindex = tooltips.FindLastIndex(t => (t.mod == "Terraria" || t.mod == Mod.Name) && (t.isModifier || t.Name.StartsWith("Tooltip") || t.Name.Equals("Material")));
				if (ttindex != -1)
				{
					if (radiusBoostPrefix != 0)
					{
						TooltipLine tt = new TooltipLine(Mod, "PrefixClickerRadius", (radiusBoostPrefix > 0 ? "+" : "") + LangHelper.GetText("Prefix.PrefixClickerRadius.Tooltip", (int)((radiusBoostPrefix / 2) * 100)))
						{
							isModifier = true,
							isModifierBad = radiusBoostPrefix < 0
						};
						tooltips.Insert(++ttindex, tt);
					}
					if (clickBoostPrefix != 0)
					{
						TooltipLine tt = new TooltipLine(Mod, "PrefixClickBoost", (clickBoostPrefix < 0 ? "" : "+") + LangHelper.GetText("Prefix.PrefixClickBoost.Tooltip", clickBoostPrefix))
						{
							isModifier = true,
							isModifierBad = clickBoostPrefix > 0
						};
						tooltips.Insert(++ttindex, tt);
					}
				}
			}
		}

		public override bool PreReforge(Item item)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				radiusBoostPrefix = 0f;
				clickBoostPrefix = 0;
			}

			return base.PreReforge(item);
		}

		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				if (item.maxStack == 1 && item.useStyle > 0)
				{
					return rand.Next(ClickerPrefix.ClickerPrefixes);
				}
			}
			return base.ChoosePrefix(item, rand);
		}

		public override bool? UseItem(Item item, Player player)
		{
			if (player.altFunctionUse == 2 && HasAltFunctionUse(item, player))
			{
				//Right click 
				var clickerPlayer = player.GetModPlayer<ClickerPlayer>();
				if (clickerPlayer.setAbilityDelayTimer <= 0)
				{
					//Mice armor 
					if (clickerPlayer.setMice)
					{
						bool canTeleport = false;
						if (!clickerPlayer.HasClickEffect(ClickEffect.PhaseReach))
						{
							//collision
							float radiusSQ = clickerPlayer.ClickerRadiusReal;
							radiusSQ *= radiusSQ;
							if (player.DistanceSQ(Main.MouseWorld) < radiusSQ && Collision.CanHitLine(player.Center, 1, 1, Main.MouseWorld, 1, 1))
							{
								canTeleport = true;
							}
						}
						else
						{
							canTeleport = true;
						}

						if (canTeleport)
						{
							SoundEngine.PlaySound(SoundID.Item, (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 115);

							player.ClickerTeleport(Main.MouseWorld);

							NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
							clickerPlayer.setAbilityDelayTimer = 60;

							float num102 = 50f;
							int num103 = 0;
							while ((float)num103 < num102)
							{
								Vector2 vector12 = Vector2.UnitX * 0f;
								vector12 += -Vector2.UnitY.RotatedBy((double)((float)num103 * (MathHelper.TwoPi / num102)), default(Vector2)) * new Vector2(2f, 2f);
								vector12 = vector12.RotatedBy((double)Vector2.Zero.ToRotation(), default(Vector2));
								int num104 = Dust.NewDust(Main.MouseWorld, 0, 0, ModContent.DustType<MiceDust>(), 0f, 0f, 0, default(Color), 2f);
								Main.dust[num104].noGravity = true;
								Main.dust[num104].position = Main.MouseWorld + vector12;
								Main.dust[num104].velocity = Vector2.Zero * 0f + vector12.SafeNormalize(Vector2.UnitY) * 4f;
								int num = num103;
								num103 = num + 1;
							}
						}
					}
					else if (clickerPlayer.setMotherboard)
					{
						SoundEngine.PlaySound(SoundID.Camera, (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 0);

						Vector2 sensorLocation = player.Center + clickerPlayer.CalculateMotherboardPosition(clickerPlayer.ClickerRadiusReal);

						if (sensorLocation.DistanceSQ(Main.MouseWorld) < 20 * 20)
						{
							//Clicked onto the sensor
							clickerPlayer.ResetMotherboardPosition();
						}
						else
						{
							clickerPlayer.SetMotherboardRelativePosition(Main.MouseWorld);
						}

						clickerPlayer.setAbilityDelayTimer = 60;
					}
				}
				return false;
			}

			return base.UseItem(item, player);
		}

		public override bool CanShoot(Item item, Player player)
		{
			if (player.altFunctionUse == 2 && HasAltFunctionUse(item, player))
			{
				return false;
			}
			return base.CanShoot(item, player);
		}

		public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				position = Main.MouseWorld;
			}
		}

		public override bool Shoot(Item item, Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (ClickerSystem.IsClickerWeapon(item))
			{
				var clickerPlayer = player.GetModPlayer<ClickerPlayer>();

				//Base 
				SoundEngine.PlaySound(SoundID.MenuTick, player.position);
				clickerPlayer.AddClick();

				bool hasAutoClick = player.HasBuff(ModContent.BuffType<AutoClick>());
				if (!hasAutoClick)
				{
					clickerPlayer.AddClickAmount();
				}

				//TODO dire: maybe "PreShoot" hook wrapping around the next NewProjectile

				//Spawn normal click damage
				Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);

				//Portable Particle Accelerator acc
				if (clickerPlayer.accPortableParticleAccelerator && clickerPlayer.accPortableParticleAccelerator2)
				{
					Vector2 vec = Main.MouseWorld;
					float num102 = 25f;
					int num103 = 0;
					while ((float)num103 < num102)
					{
						Vector2 vector12 = Vector2.UnitX * 0f;
						vector12 += -Vector2.UnitY.RotatedBy((double)((float)num103 * (6.28318548f / num102)), default(Vector2)) * new Vector2(4f, 4f);
						vector12 = vector12.RotatedBy((double)player.velocity.ToRotation(), default(Vector2));
						int num104 = Dust.NewDust(vec, 0, 0, 229, 0f, 0f, 0, default(Color), 1f);
						Main.dust[num104].noGravity = true;
						Main.dust[num104].position = vec + vector12;
						Main.dust[num104].velocity = player.velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * 1f;
						int num = num103;
						num103 = num + 1;
					}
				}
				
				//Mouse Trap
				if (clickerPlayer.accMouseTrap)
				{
					if (Main.rand.NextBool(50))
					{
						SoundEngine.PlaySound(2, (int)player.position.X, (int)player.position.Y, 153);
						player.AddBuff(BuffID.Cursed, 60, false);
					}
				}

				//Hot Keychain 
				if (clickerPlayer.accHotKeychain2)
				{
					Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<HotKeychainPro>(), (int)(damage * 0.25f), knockback, player.whoAmI);
				}

				int overclockType = ModContent.BuffType<OverclockBuff>();
				//Overclock armor set bonus
				if (clickerPlayer.clickAmount % 100 == 0 && clickerPlayer.setOverclock)
				{
					SoundEngine.PlaySound(2, (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 94);
					player.AddBuff(overclockType, 180, false);
					for (int i = 0; i < 25; i++)
					{
						int num6 = Dust.NewDust(player.position, 20, 20, 90, 0f, 0f, 150, default(Color), 1.35f);
						Main.dust[num6].noGravity = true;
						Main.dust[num6].velocity *= 0.75f;
						int num7 = Main.rand.Next(-50, 51);
						int num8 = Main.rand.Next(-50, 51);
						Dust dust = Main.dust[num6];
						dust.position.X = dust.position.X + (float)num7;
						Dust dust2 = Main.dust[num6];
						dust2.position.Y = dust2.position.Y + (float)num8;
						Main.dust[num6].velocity.X = -(float)num7 * 0.075f;
						Main.dust[num6].velocity.Y = -(float)num8 * 0.075f;
					}
				}

				bool overclock = player.HasBuff(overclockType);

				if (!hasAutoClick)
				{
					foreach (var name in ClickerSystem.GetAllEffectNames())
					{
						if (clickerPlayer.HasClickEffect(name, out ClickEffect effect))
						{
							//Find click amount
							int clickAmountTotal = clickerPlayer.GetClickAmountTotal(this, name);
							bool reachedAmount = clickerPlayer.clickAmount % clickAmountTotal == 0;

							if (reachedAmount || overclock || (clickerPlayer.accTriggerFinger && clickerPlayer.OutOfCombat))
							{
								effect.Action?.Invoke(player, source, position, type, damage, knockback);
								
								if (clickerPlayer.accTriggerFinger)
								{
									//TODO looks like a hack
									clickerPlayer.outOfCombatTimer = ClickerPlayer.OutOfCombatTimeMax;
								}
							}
						}
					}
				}

				return false;
			}
			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}
	}
}
