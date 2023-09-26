using Modding;
using System.Reflection;
using UnityEngine;
using Vasi;
namespace RandomCharm
{
    public class RandomCharm : Mod, ITogglableMod
    {
        public override string GetVersion()
        {
            return VersionUtil.GetVersion<RandomCharm>();
        }
        private readonly System.Random _rand = new();
        private List<int> collectCharms = new();
        private List<int> equippedCharms = new();
        public override void Initialize()
        {
            On.GameManager.BeginScene += ChangeCharm;
            On.PlayMakerFSM.Start += SendInv;
        }

        private void SendInv(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            orig(self);
            if (self is
                {
                    name: "Health",
                    FsmName: "Blue Health Control"
                })
            {
                self.GetState("Wait").AddTransition(HutongGames.PlayMaker.FsmEvent.Finished, "Hive Check");
            }
        }

        private void ChangeCharm(On.GameManager.orig_BeginScene orig, GameManager self)
        {
            orig(self);
            CountCollect();
            RandomCollect();
            SetEquipped();
            DisplayEquipped();
        }

        private void CountCollect()
        {
            collectCharms.Clear();
            for (int i = 1; i <= 100; i++)
            {
                if (PlayerData.instance.GetBool($"gotCharm_{i}"))
                {
                    collectCharms.Add(i);
                }
            }
        }
        private void RandomCollect()
        {
            for (int i = collectCharms.Count - 1; i > 0; i--)
            {
                int pos = _rand.Next(i + 1);
                int temp = collectCharms[i];
                collectCharms[i] = collectCharms[pos];
                collectCharms[pos] = temp;

            }
        }
        private void SetEquipped()
        {
            equippedCharms.Clear();
            for (int i = 0; i <= 100; i++)
            {
                PlayerData.instance.SetBool($"equippedCharm_{i}", false);
                PlayerData.instance.UnequipCharm(i);
            }
            PlayerData.instance.SetBool("overcharmed", false);
            foreach (int num in collectCharms)
            {
                PlayerData.instance.CalculateNotchesUsed();
                if (num == 36)
                {
                    if (PlayerData.instance.royalCharmState < 3)
                    {
                        continue;
                    }
                }
                if (PlayerData.instance.GetInt("charmSlots") - 1 > PlayerData.instance.GetInt("charmSlotsFilled"))
                {
                    PlayerData.instance.SetBool($"equippedCharm_{num}", true);
                    PlayerData.instance.EquipCharm(num);
                    equippedCharms.Add(num);
                    PlayerData.instance.CalculateNotchesUsed();
                    if (PlayerData.instance.GetInt("charmSlotsFilled") > PlayerData.instance.GetInt("charmSlots"))
                    {
                        PlayerData.instance.SetBool("overcharmed", true);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (HeroController.instance != null)
            {
                CustomUpdate.CharmUpdate();
                GameObject health = GameObject.Find("Health");

                health.LocateMyFSM("Blue Health Control").SetState("Init");
                if (!GameManager.instance.sceneName.Contains("GG_Gruz_Mother"))//It didnt wake
                {
                    PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
                    PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
                }
                health.transform.Find("OC Backboard").gameObject.SetActive(PlayerData.instance.overcharmed);
            }
        }

        private void DisplayEquipped()
        {
            var holder = GameObject.Find("charm_dispaly_holder");
            if (holder != null)
            {
                Log("Found existing holder.");
                GameObject.DestroyObject(holder);
            }
            holder = new GameObject("charm_dispaly_holder");
            GameObject _GameCameras = null;
            foreach (var g in HeroController.instance.gameObject.scene.GetRootGameObjects())
            {
                if (g.name == "_GameCameras")
                {
                    _GameCameras = g;
                }
            }
            var HudCamera = _GameCameras.transform.Find("HudCamera").gameObject;
            var Inventory = HudCamera.transform.Find("Inventory").gameObject;
            var Charms = Inventory.transform.Find("Charms").gameObject;
            var Collected_Charms = Charms.transform.Find("Collected Charms").gameObject;
            float x = 4;
            foreach (var num in equippedCharms)
            {
                var c = Collected_Charms.transform.Find(num.ToString());
                var newC = GameObject.Instantiate(c, holder.transform);
                newC.transform.localScale = new Vector3(1, 1, 1);
                var p = newC.transform.localPosition;
                newC.transform.localPosition = new Vector3(x, 6.3f, p.z);
                newC.name = c.name;
                x += 1.5f;
                newC.GetComponentInChildren<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
                newC.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
        }

        public void Unload()
        {
            On.GameManager.BeginScene -= ChangeCharm;
            On.PlayMakerFSM.Start -= SendInv;
        }
    }
}
