using Modular_Items.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Modular_Items.EntityBehaviors
{
	public class EntityBehaviorCombatReplacement : EntityBehavior
	{
		public EntityBehaviorCombatReplacement(Entity entity) : base(entity)
		{
		}

		public override string PropertyName()
		{
			return "CombatReplacement";
		}

		public override void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition, EnumInteractMode mode, ref EnumHandling handled)
		{
			if(mode == EnumInteractMode.Interact) { return; }
			handled = EnumHandling.PreventDefault;
			EnumDamageType damageType = EnumDamageType.BluntAttack;

			double rnd = 1.0d;

			if (byEntity.Api.Side == EnumAppSide.Server)
			{
				rnd = Math.Max(0.8d, (byEntity.Api as ICoreServerAPI).World.Rand.NextDouble() + 0.2d);
			} 

			
			float damage = (((itemslot.Itemstack == null) ? 0.5f : itemslot.Itemstack.Collectible.GetAttackPower(itemslot.Itemstack))) * (float)rnd;
			int damagetier = ((itemslot.Itemstack != null) ? itemslot.Itemstack.Collectible.ToolTier : 0);
			damage *= byEntity.Stats.GetBlended("meleeWeaponsDamage");
			
			JsonObject attributes = entity.Properties.Attributes;
			if (attributes != null && attributes["isMechanical"].AsBool())
			{
				damage *= byEntity.Stats.GetBlended("mechanicalsDamage");
			}
			IPlayer byPlayer = null;
			if (byEntity is EntityPlayer)
			{
				byPlayer = (byEntity as EntityPlayer).Player;
				entity.World.PlaySoundAt(new AssetLocation("sounds/player/slap"), entity.ServerPos.X, entity.ServerPos.Y, entity.ServerPos.Z, byPlayer);
				itemslot?.Itemstack?.Collectible.OnAttackingWith(byEntity.World, byEntity, entity, itemslot);
				if(itemslot?.Itemstack?.Item is CompositeItemTool)
				{
					damageType = (itemslot.Itemstack.Item as CompositeItemTool).GetDamageType(itemslot.Itemstack);
				}
				
			}
			
			DamageSource dmgSource = new DamageSource
			{
				Source = (((byEntity as EntityPlayer).Player != null) ? EnumDamageSource.Player : EnumDamageSource.Entity),
				SourcePos = byEntity.Pos.XYZFloat.ToVec3d(),
				SourceEntity = byEntity,
				Type = damageType,
				HitPosition = hitPosition,
				DamageTier = damagetier
				
			};
			if (entity.ReceiveDamage(dmgSource, damage))
			{
				byEntity.DidAttack(dmgSource, entity as EntityAgent);
			}		
			
			

		}
	}
}
