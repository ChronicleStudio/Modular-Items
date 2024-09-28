using Modular_Items.EntityBehaviors;
using Modular_Items.Enums;
using Modular_Items.Items;
using Modular_Items.Utils;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Modular_Items
{
	public class Modular_ItemsModSystem : ModSystem
	{
		// Called on server and client
		// Useful for registering block/entity classes on both sides
		public override void Start(ICoreAPI api)
		{
			api.RegisterItemClass("ItemComponent", typeof(ItemComponent));
			api.RegisterItemClass("CompositeItem", typeof(CompositeItem));
			api.RegisterItemClass("CompositeItemTool", typeof(CompositeItemTool));

			//api.RegisterCollectibleBehaviorClass("ModularAnimation", typeof(CBModularAnimation));

			api.Logger.Notification("Hello from template mod: " + api.Side);
		}

		public override void StartServerSide(ICoreServerAPI api)
		{
			api.Event.OnEntityLoaded += AddEntityBehaviors;
			api.Event.OnEntitySpawn += AddEntityBehaviors;
			

			api.Logger.Notification("Hello from template mod server side: " + Lang.Get("modular_items:hello"));
		}

		private void AddEntityBehaviors(Entity entity)
		{
			if(entity != null && !entity.HasBehavior<EntityBehaviorCombatReplacement>()) {
				entity.AddBehavior(new EntityBehaviorCombatReplacement(entity));
			}

			if (entity != null && !entity.HasBehavior<EntityBehaviorGore>())
			{
				entity.AddBehavior(new EntityBehaviorGore(entity));
			}

		}

		public override void StartClientSide(ICoreClientAPI api)
		{
			api.Event.OnEntityLoaded += AddEntityBehaviors;
			api.Event.OnEntitySpawn += AddEntityBehaviors;

			api.Logger.Notification("Hello from template mod client side: " + Lang.Get("modular_items:hello"));
		}
		
	}
}
