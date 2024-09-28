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
using Vintagestory.GameContent;

namespace Modular_Items.EntityBehaviors
{
	public class EntityBehaviorGore : EntityBehavior
	{
		public EntityBehaviorGore(Entity entity) : base(entity)
		{
		}

		public override string PropertyName()
		{
			return "Gore";
		}
		
		public override void OnEntityReceiveDamage(DamageSource dmgSource, ref float damage)
		{			
			if (entity.Api.Side == EnumAppSide.Client && damage > 0.5f /* && !entity.IsActivityRunning("invulnerable") */ && (dmgSource.Type == EnumDamageType.SlashingAttack || dmgSource.Type == EnumDamageType.PiercingAttack || dmgSource.Type == EnumDamageType.BluntAttack))
			{
				JsonObject attributes = entity.Properties.Attributes;

				Vec3d vec3d = entity.SidedPos.XYZ + dmgSource.HitPosition;
				Vec3d minPos = vec3d.AddCopy(-0.15, -0.15, -0.15);
				Vec3d maxPos = vec3d.AddCopy(0.15, 0.15, 0.15);
				int textureSubId = entity.Properties.Client.FirstTexture.Baked.TextureSubId;
				Vec3f tmp = new Vec3f();
				int particleQuantity;
				int color;

				IPlayer byPlayer = null;
				if (dmgSource.Source == EnumDamageSource.Player)
				{
					byPlayer = (dmgSource.SourceEntity as EntityPlayer).Player;
				}

				if (entity.Properties.Attributes["isMechanical"].Exists || entity.Class == "EntityDrifter")
				{
					color = (entity.Api as ICoreClientAPI).EntityTextureAtlas.GetRandomColor(textureSubId);
					particleQuantity = (dmgSource.Type == EnumDamageType.SlashingAttack) ? 2 : (dmgSource.Type == EnumDamageType.PiercingAttack) ? 1 : 5;
				}
				else
				{
					color = ColorUtil.ToRgba(230, 120, 0, 0);
					particleQuantity = (dmgSource.Type == EnumDamageType.SlashingAttack) ? 5 : (dmgSource.Type == EnumDamageType.PiercingAttack) ? 2 : 1;
				}
				for (int i = 0; i < damage; i++)
				{
					tmp.Set(1f - 2f * (float)entity.World.Rand.NextDouble(), 2f * (float)entity.World.Rand.NextDouble(), 1f - 2f * (float)entity.World.Rand.NextDouble());
					entity.World.SpawnParticles(particleQuantity, color, minPos, maxPos, tmp, tmp, 3.0f, 1f, 0.5f + (float)entity.World.Rand.NextDouble() * 0.25f * 2 / particleQuantity, EnumParticleModel.Cube, byPlayer);
				}
			
			}
		}
	}
}
