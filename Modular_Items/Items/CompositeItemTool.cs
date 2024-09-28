using Modular_Items.ComponentProperties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace Modular_Items.Items
{
	public class CompositeItemTool : CompositeItem
	{
		ICoreAPI coreAPI;

		bool onlyOnTarget = true;
		public EnumHandInteract strikeSoundHandInteract = EnumHandInteract.HeldItemAttack;		
		

		public override void OnLoaded(ICoreAPI api)
		{
			coreAPI = api;	
			base.OnLoaded(api);
		}
		public static float getHitDamageAtFrame(EntityAgent byEntity, string animCode)
		{
			if (byEntity.Properties.Client.AnimationsByMetaCode.TryGetValue(animCode, out var animdata))
			{
				if (animdata.Attributes?["damageAtFrame"].Exists == true)
				{
					return animdata.Attributes["damageAtFrame"].AsFloat(-1) / animdata.AnimationSpeed;
				}

			}
			return -1;
		}

		public static float getSoundAtFrame(EntityAgent byEntity, string animCode)
		{
			if (byEntity.Properties.Client.AnimationsByMetaCode.TryGetValue(animCode, out var animdata))
			{
				if (animdata.Attributes?["soundAtFrame"].Exists == true)
				{
					return animdata.Attributes["soundAtFrame"].AsFloat(-1) / animdata.AnimationSpeed;
				}
			}
			return -1;
		}		


		public override string GetHeldTpHitAnimation(ItemSlot slot, Entity byEntity)
		{
			string chosenAnim = "falx";
			var props = GetToolProps(slot.Itemstack);
			if (props == null) return chosenAnim;
			foreach (var prop in props)
			{
				if (prop == null) continue;
				if (prop.HeldTpHitAnimation == "") continue;
				chosenAnim = prop.HeldTpHitAnimation;
			}
			if (chosenAnim == "") chosenAnim = "falx";

			return chosenAnim;
		}

		public override float GetAttackPower(IItemStack itemstack)
		{
			float attackPower = 0.0f;
			float attackPowerMult = 1.0f;
			var props = GetToolProps(itemstack);
			if(props == null) return attackPower;
			foreach ( var prop in props )
			{	
				if(prop == null ) continue;
				attackPower += prop.attackPower;
				attackPowerMult += prop.attackPowerMult;				
			}
			attackPower *= attackPowerMult;		

			return attackPower;
		}

		public override float GetAttackRange(IItemStack itemstack)
		{
			float attackRange = 0.0f;
			float attackRangeMult = 1.0f;
			var props = GetToolProps(itemstack);
			if (props == null) return 0.5f;
			foreach (var prop in props)
			{
				if(prop == null) continue;
				attackRange += prop.attackRange;
				attackRangeMult += prop.attackRangeMult;
			}
			attackRange *= attackRangeMult;	

			
			return attackRange;
		}

		public override int GetMaxDurability(ItemStack itemstack)
		{
			int maxDurability = 0;
			int maxDurabilityMult = 1;
			var props = GetToolProps(itemstack);
			if (props == null) return 1;
			foreach (var prop in props)
			{
				if(prop == null) continue;
				maxDurability += prop.maxDurability;
				maxDurabilityMult += prop.maxDurabilityMult;
			}
			maxDurability *= maxDurabilityMult;
			return maxDurability;			
		}

		/// <summary>
		/// Adds lines to the HeldItemInfo to show what the final item was made from
		/// </summary>
		/// <param name="inSlot"></param>
		/// <param name="dsc"></param>
		/// <param name="world"></param>
		/// <param name="withDebugInfo"></param>
		public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
		{
			dsc.AppendLine("Made with the following components:");
			var stacks = GetStacks(inSlot.Itemstack);
			foreach ( var stack in stacks)
			{
				dsc.AppendLine(Lang.Get(stack.GetName()));
			}			
			dsc.AppendLine();

			base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
		}

		/// <summary>
		/// Should return a simple name for the weapon: Sword, Axe, Spear, Pickaxe, etc...
		/// </summary>
		/// <param name="itemStack"></param>
		/// <returns></returns>
		public override string GetHeldItemName(ItemStack itemStack)
		{
			string name = "compositetool-";
			var components = GetComponents(itemStack);
			foreach( var component in components)
			{
				if(component.componentType == Enums.EnumComponentTypeFlag.Head)
				{
					 return Lang.Get(component.Code.Domain + ":" + name + component.Code.Path.ToString());
				}
			}

			return "tool-unknown";
		}

		/// <summary>
		/// Should combine the mining speeds of all the components used to make the tool or if the components operate independantly of eachother,
		/// this should only account for certain tool modes
		/// </summary>
		/// <param name="itemstack"></param>
		/// <param name="blockSel"></param>
		/// <param name="block"></param>
		/// <param name="forPlayer"></param>
		/// <returns></returns>
		public override float GetMiningSpeed(IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer forPlayer)
		{
			float miningSpeed = 1.0f;
			float miningSpeedMult = 1.0f;

			ITreeAttribute miningSpeedTree = itemstack.Attributes.GetOrAddTreeAttribute("miningSpeeds");			

			EnumBlockMaterial blockMaterial = block.GetBlockMaterial(api.World.BlockAccessor, blockSel.Position);
						
			if (blockMaterial == EnumBlockMaterial.Ore || blockMaterial == EnumBlockMaterial.Stone)
			{
				miningSpeedMult = forPlayer.Entity.Stats.GetBlended("miningSpeedMul");
			}

			if (miningSpeedTree.HasAttribute(blockMaterial.ToString()))
			{
				miningSpeed = miningSpeedTree.GetFloat(blockMaterial.ToString());
			}
			else
			{
				var props = GetToolProps(itemstack);
				if (props == null) return miningSpeed;
				foreach (var prop in props)
				{
					if (prop == null) continue;
					if (prop.MiningSpeed != null && prop.MiningSpeed.ContainsKey(blockMaterial))
					{
						miningSpeed += prop.MiningSpeed[blockMaterial];
					}
				}

				miningSpeedTree.SetFloat(blockMaterial.ToString(), miningSpeed);
			}
			if (miningSpeed == 1.0f) {
				return miningSpeed;
			}			

			return miningSpeed * miningSpeedMult * GlobalConstants.ToolMiningSpeedModifier;
			
		}		

		/// <summary>
		/// Should start custom attack
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="byEntity"></param>
		/// <param name="blockSel"></param>
		/// <param name="entitySel"></param>
		/// <param name="handling"></param>
		public override void OnHeldAttackStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handHandling)
		{
			if (onlyOnTarget && entitySel == null) return;
			
			StartAttack(slot, byEntity);
			handHandling = EnumHandHandling.PreventDefault;
			
			
		}

		

		/// <summary>
		/// Called every step of the attack sequence
		/// </summary>
		/// <param name="secondsPassed"></param>
		/// <param name="slot"></param>
		/// <param name="byEntity"></param>
		/// <param name="blockSelection"></param>
		/// <param name="entitySel"></param>
		/// <returns></returns>
		public override bool OnHeldAttackStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
		{			
			return StepAttack(slot, byEntity);
		
		}

		/// <summary>
		/// Called after completing a custom attack
		/// </summary>
		/// <param name="secondsPassed"></param>
		/// <param name="slot"></param>
		/// <param name="byEntity"></param>
		/// <param name="blockSelection"></param>
		/// <param name="entitySel"></param>
		public override void OnHeldAttackStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
		{		

			return;
		}

		/// <summary>
		/// Return false to cancel the attack, run code inside this method to decide if this attack can be cancelled
		/// </summary>
		/// <param name="secondsPassed"></param>
		/// <param name="slot"></param>
		/// <param name="byEntity"></param>
		/// <param name="blockSelection"></param>
		/// <param name="entitySel"></param>
		/// <param name="cancelReason"></param>
		/// <returns></returns>
		public override bool OnHeldAttackCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
		{			
			return false;
		}
		public void StartAttack(ItemSlot slot, EntityAgent byEntity)
		{

			string anim = GetHeldTpHitAnimation(slot, byEntity);

			byEntity.Attributes.SetInt("didattack", 0);

			byEntity.AnimManager.RegisterFrameCallback(new AnimFrameCallback()
			{
				Animation = anim,  //anim,
				Frame = getSoundAtFrame(byEntity, anim),
				Callback = () => PlayStrikeSound(byEntity)
			});

			byEntity.AnimManager.RegisterFrameCallback(new AnimFrameCallback()
			{
				Animation = anim,
				Frame = getHitDamageAtFrame(byEntity, anim),
				Callback = () => HitEntity(byEntity)
			});
		}
		public bool StepAttack(ItemSlot slot, EntityAgent byEntity)
		{
			string animCode = GetHeldTpHitAnimation(slot, byEntity);			

			return byEntity.AnimManager.IsAnimationActive(animCode);
		}

		private void PlayStrikeSound(EntityAgent byEntity)
		{
			IPlayer byPlayer = (byEntity as EntityPlayer).Player;
			if (byPlayer == null) return;

			if (byEntity.Controls.HandUse == strikeSoundHandInteract)
			{
				byPlayer.Entity.World.PlaySoundAt(GetStrikeSound(byEntity.ActiveHandItemSlot, byEntity), byPlayer.Entity, byPlayer, 0.9f + (float)byEntity.World.Rand.NextDouble() * 0.2f, 16, 0.35f);
			}
		}

		private void HitEntity(EntityAgent byEntity)
		{
			EnumHandling handling = EnumHandling.PassThrough;
			//OnBeginHitEntity?.Invoke(byEntity, ref handling);
			if (handling != EnumHandling.PassThrough) return;


			var entitySel = (byEntity as EntityPlayer)?.EntitySelection;

			if (byEntity.World.Side == EnumAppSide.Client)
			{
				IClientWorldAccessor world = byEntity.World as IClientWorldAccessor;

				if (byEntity.Attributes.GetInt("didattack") == 0)
				{
					if (entitySel != null) world.TryAttackEntity(entitySel);
					byEntity.Attributes.SetInt("didattack", 1);
					world.AddCameraShake(0.25f);
				}
			}
			else
			{
				if (byEntity.Attributes.GetInt("didattack") == 0 && entitySel != null)
				{

					byEntity.Attributes.SetInt("didattack", 1);
				}
			}
		}

		private AssetLocation GetStrikeSound(ItemSlot slot, EntityAgent byEntity)
		{
			var components = (slot.Itemstack.Item as CompositeItemTool).GetComponents(slot.Itemstack);

			foreach (var component in components)
			{
				if (component.toolProps.strikeSound != null) return AssetLocation.Create(component.toolProps.strikeSound);
			}

			return AssetLocation.Create("game:sounds/player/strike1");
		}


		private ToolProperties[] GetToolProps(IItemStack itemstack)
		{
			List<ToolProperties> toolProps = new List<ToolProperties>();

			ITreeAttribute tree = itemstack.Attributes.GetOrAddTreeAttribute("components");
			foreach (var item in tree.Values)
			{
				ItemStack stack = (ItemStack)item.GetValue();
				stack.ResolveBlockOrItem(coreAPI.World);							
				toolProps.Add((stack.Item as ItemComponent).toolProps);
			}

			return toolProps.ToArray();
		}

		private ItemStack[] GetStacks(IItemStack itemstack)
		{
			List<ItemStack> stacks = new List<ItemStack>();

			ITreeAttribute tree = itemstack.Attributes.GetOrAddTreeAttribute("components");
			foreach (var item in tree.Values)
			{
				ItemStack stack = (ItemStack)item.GetValue();
				stack.ResolveBlockOrItem(coreAPI.World);
				stacks.Add(stack);
			}

			return stacks.ToArray();

		}

		public ItemComponent[] GetComponents(IItemStack itemstack)
		{
			List<ItemComponent> itemComponents = new List<ItemComponent>();

			ITreeAttribute tree = itemstack.Attributes.GetOrAddTreeAttribute("components");
			foreach (var item in tree.Values)
			{
				ItemStack stack = (ItemStack)item.GetValue();
				stack.ResolveBlockOrItem(coreAPI.World);
				itemComponents.Add(stack.Item as ItemComponent);
			}

			return itemComponents.ToArray();
		}

		public EnumDamageType GetDamageType(ItemStack itemstack)
		{
			var components = GetComponents(itemstack);
			EnumDamageType damageType = EnumDamageType.BluntAttack;

			foreach(var component in components) {
				if (damageType == component.toolProps.damageType) continue;
				else
				{
					return component.toolProps.damageType;
				}
			}

			return damageType;
		}
		
	}
}
