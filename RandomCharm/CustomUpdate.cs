using Modding;
using UnityEngine;
using Vasi;
namespace RandomCharm
{
    public class CustomUpdate
    {
        public static HeroController hc = HeroController.instance;
        public static void CharmUpdate()
        {
            if (HeroController.instance.playerData.GetBool("equippedCharm_26"))
            {
                ReflectionHelper.SetField(hc, "nailChargeTime", hc.NAIL_CHARGE_TIME_CHARM);
            }
            else
            {
                ReflectionHelper.SetField(hc, "nailChargeTime", hc.NAIL_CHARGE_TIME_DEFAULT);
            }
            if (hc.playerData.GetBool("equippedCharm_23") && !hc.playerData.GetBool("brokenCharm_23"))
            {
                hc.playerData.SetInt("maxHealth", hc.playerData.GetInt("maxHealthBase") + 2);
                hc.playerData.SetInt("prevHealth", hc.playerData.GetInt("health"));
                hc.playerData.SetInt("health", hc.playerData.GetInt("health") + hc.playerData.GetInt("maxHealth") - hc.playerData.GetInt("maxHealthBase"));
                hc.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
                PlayerData.instance.UpdateBlueHealth();
            }
            else
            {
                hc.playerData.SetInt("maxHealth", hc.playerData.GetInt("maxHealthBase"));
                hc.playerData.SetInt("prevHealth", hc.playerData.GetInt("health"));
                hc.playerData.SetInt("health", hc.playerData.GetInt("health") + hc.playerData.GetInt("maxHealth") - hc.playerData.GetInt("maxHealthBase"));
                if (PlayerData.instance.health > PlayerData.instance.maxHealth)
                {
                    hc.playerData.SetInt("health", hc.playerData.GetInt("maxHealth"));
                }
                hc.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
                PlayerData.instance.UpdateBlueHealth();
            }
            if (hc.playerData.GetBool("equippedCharm_27"))
            {
                Count = 0;
                ModHooks.TakeHealthHook -= CountHeath;
                if (hc.playerData.GetBool("equippedCharm_8"))
                {
                    Count -= 2;
                }
                if (hc.playerData.GetBool("equippedCharm_9"))
                {
                    Count -= 4;
                }
                if (hc.playerData.GetBool("equippedCharm_23") && !hc.playerData.GetBool("brokenCharm_23"))
                {
                    Count -= 3;
                }
                hc.playerData.SetInt("joniHealthBlue", (int)((float)hc.playerData.GetInt("maxHealth") * 1.4f));
                hc.playerData.SetInt("maxHealth", 1);
                hc.playerData.SetInt("prevHealth", hc.playerData.GetInt("health"));
                hc.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
                PlayerData.instance.UpdateBlueHealth();
                hc.playerData.SetInt("health", 1);
                ReflectionHelper.SetField(hc, "joniBeam", true);
                ModHooks.TakeHealthHook += CountHeath;

            }
            else
            {
                if (Count < 0)
                {
                    Count = 0;
                }
                if (PlayerData.instance.joniHealthBlue > 0)
                {
                    hc.playerData.SetInt("health", hc.playerData.GetInt("maxHealth") - (int)(Count / 1.4f));
                    ModHooks.TakeHealthHook -= CountHeath;
                    Count = 0;
                }
                hc.playerData.SetInt("joniHealthBlue", 0);
            }
            if (hc.playerData.GetBool("equippedCharm_40") && hc.playerData.GetInt("grimmChildLevel") == 5)
            {
                hc.carefreeShieldEquipped = true;
            }
            else
            {
                hc.carefreeShieldEquipped = false;
            }
            hc.playerData.UpdateBlueHealth();
            UpdateHealth();
        }

        private static int CountHeath(int damage)
        {
            Count += damage;
            Modding.Logger.Log("[RandomCharm]HitCount." + Count.ToString());
            return damage;
        }

        private static void UpdateHealth()
        {
            GameObject Health = GameCameras.instance.hudCanvas.gameObject.Child("Health");
            for (int i = 1; i <= hc.playerData.GetInt("maxHealth"); i++)
            {
                GameObject healthx = Health.Child($"Health {i}");
                healthx.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        public static int Count = 0;
    }
}
