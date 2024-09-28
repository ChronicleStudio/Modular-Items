using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Modular_Items.Items
{
	public class TestItem : CompositeItem
	{

		public override void OnHeldAttackStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handling)
		{
			handling = EnumHandHandling.PreventDefaultAction;
			
		}

		public override bool OnHeldAttackStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
		{
			if (byEntity.Controls.HandUse != EnumHandInteract.HeldItemAttack || !byEntity.Attributes.GetBool("attackStarted", false))
			{
				if (secondsPassed > 2.0)
				{
					LaunchStrongAttack(slot, byEntity);
				} else if (secondsPassed > 1.0) {
					LaunchWeakAttack(slot, byEntity);
				} else
				{
					return false;
				}
			}

			if (byEntity.Attributes.GetBool("attackStarted", false)){ 
				
			}

			
			
			return base.OnHeldAttackStep(secondsPassed, slot, byEntity, blockSelection, entitySel);
		}

		private void LaunchStrongAttack(ItemSlot slot, EntityAgent byEntity)
		{
			throw new NotImplementedException();
		}

		private void LaunchWeakAttack(ItemSlot slot, EntityAgent byEntity)
		{
			throw new NotImplementedException();
		}

		public override bool OnHeldAttackCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
		{
			return base.OnHeldAttackCancel(secondsPassed, slot, byEntity, blockSelection, entitySel, cancelReason);
		}

		public override void OnHeldAttackStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
		{
			base.OnHeldAttackStop(secondsPassed, slot, byEntity, blockSelection, entitySel);
		}
	}
}
