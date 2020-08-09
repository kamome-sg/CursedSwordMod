using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Graphics;
using ReLogic.Reflection;
using static Terraria.ModLoader.ModContent;

namespace CursedSword
{
    public class CursedSword : Mod
    {
        public CursedSword()
        {

        }
    }

    public class DarkDisaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Disaster");
            Tooltip.SetDefault("\"Death is inevitable\"");
        }

        public override void SetDefaults()
        {
            item.damage = 1;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.scale = 1.25f;

            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item71;

            item.rare = 66;
            item.expert = true;
        }

        //�c�[���`�b�v�ҏW
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine tooltipLine in tooltips)
            {
                switch (tooltipLine.Name)
                {
                    case "Speed":
                    case "Knockback":
                    case "CritChance":
                    case "Expert":
                        tooltipLine.text = "";
                        break;
                }
            }
        }

        //�_���[�W�A�N���e�B�J���ҏW
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                damage = 1;
                crit = false;
            }
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                damage = 1;
                crit = false;
            }
        }

        //�f�o�t�t�^�i�㏑���Ȃ��j
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (!player.HasBuff(BuffType<Doom>()))
                {
                    player.AddBuff(BuffType<Doom>(), 5400);
                }

                if (!target.HasBuff(BuffType<Doom>()))
                {
                    target.AddBuff(BuffType<Doom>(), 1800);
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (!player.HasBuff(BuffType<Doom>()))
                {
                    player.AddBuff(BuffType<Doom>(), 5400);
                }

                if (!target.HasBuff(BuffType<Doom>()))
                {
                    target.AddBuff(BuffType<Doom>(), 1800);
                }
            }
        }

        //�U�����_�X�g����
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 127, 0, 0, 0, new Color(255, 0, 251));
            }
        }
    }

    public class Doom : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Doom");
            Description.SetDefault("You're doomed");
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //�h��[��
            player.statDefense -= int.MaxValue;
            
            if (player.whoAmI == Main.myPlayer)
            {
                //�_�X�g����
                SpawnDust(player);

                //���ʏI�����A�v���C���[���L��
                if (player.buffTime[buffIndex] == 1)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was doomed."), double.MaxValue, 0, false);
                }
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            //�_�X�g����
            SpawnDust(npc);

            //���ʏI�����ANPC���L��
            if (npc.buffTime[buffIndex] == 1)
            {
                npc.StrikeNPC(int.MaxValue, 0f, -npc.direction, true);
            }
        }

        //50%�̊m���Ń_�X�g����
        public void SpawnDust<T>(T entity) where T : Entity
        {
            if(Main.rand.Next(2) == 0)
            {
                Dust.NewDust(entity.position, entity.width, entity.height, DustID.ApprenticeStorm, default, default, default, new Color(255, 0, 251));
            }
        }
    }

    class DoomCountdown : GlobalNPC
    {
        //�J�E���g�_�E���̃I�[�o�[���C
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if(npc.HasBuff(BuffType<Doom>()))
            {
                var font = Main.fontMouseText;
                spriteBatch.DrawString(font, ((int)(npc.buffTime[npc.FindBuffIndex(BuffType<Doom>())] / 60)).ToString(), npc.Center - Main.screenPosition - FixingPos(npc), Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        //�I�[�o�[���C�\�����̈ʒu�␳�������邽�߂�Vector2
        public Vector2 FixingPos(NPC npc)
        {
            int countdown = npc.buffTime[npc.FindBuffIndex(BuffType<Doom>())] / 60;

            return new Vector2(countdown.ToString().Length * 5 - CountChar(countdown.ToString(), '1') * 2.5f, 10);
        }

        //��������̓���̕����̐��𐔂���
        public int CountChar(string s, char c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }
    }
}