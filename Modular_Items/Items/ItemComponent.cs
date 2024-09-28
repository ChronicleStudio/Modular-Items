using Modular_Items.ComponentProperties;
using Modular_Items.Enums;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;

namespace Modular_Items.Items
{

    public class ItemComponent : Item
	{
		public EnumComponentTypeFlag componentType { get; private set; }

		public string construction;

		public ToolProperties toolProps;				

		public override void OnLoaded(ICoreAPI api)
		{
			
			
			construction = Attributes?["construction"].AsString("metal");
			if (construction == null)
			{
				construction = "metal";
			}

			JsonObject jsonObj = Attributes?["toolProperties"];
			if (jsonObj?.Exists == true)
			{
				try
				{
					toolProps = jsonObj.AsObject<ToolProperties>();
				}
				catch (Exception e){
					api.World.Logger.Error("Failed loading toolProperties for item/block {0}. Created Default. Exception: {1}", Code, e);
					toolProps = new ToolProperties();
				}
			} else
			{
				toolProps = new ToolProperties();
				api.World.Logger.Error("toolProperties for item/block {0} did not exist. Created Default.", Code);
			}

			jsonObj = null;

			jsonObj = Attributes?["componentType"];
			if (jsonObj?.Exists == true)
			{
				try
				{					
					Enum.TryParse(typeof(EnumComponentTypeFlag), jsonObj.AsString(), out var tempComp);
					componentType = (EnumComponentTypeFlag)tempComp;
				}
				catch (Exception e)
				{
					api.World.Logger.Error("Failed loading componentType for item/block {0}. Will ignore. Exception: {1}", Code, e);
					
				}
			}

			base.OnLoaded(api);			
		}

		
		
	}
}
