using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceCompare : MonoBehaviour
{
    public aiyaya[] xxx;
    public List<huayang> huayangList = new List<huayang>(), SingleHuayang;

    public List<byte> WinPlayers;
    private void Start()
    {
        //CheckHuayang(new byte[] { 4, 5, 4, 2, 2 });
        //CheckHuayang(xxx[0].dices, 1);
        Compare(xxx);
    }

    public void caonima()
    {
        //CheckHuayang(xxx[0].dices, 1);
        List<byte> xx = Compare(xxx);

        for (int k = 0; k < xx.Count; k++)
        {
            Debug.LogFormat("获胜的玩家有：{0}", xx[k]);
        }

    }
    public List<byte> Compare(aiyaya[] xxx)
    {
        //roomState.MyActors[i]
        //CheckHuayang(dices, actorNr);
        huayangList = new List<huayang>();
        for (int i = 0; i < xxx.Length; i++)
        {
            //if (hivegame.RoomState.MyActors[i] == null)
            //    continue;

            //byte[] dices = xxx[i];
            byte actorNr = (byte)(i + 1);
            CheckHuayang(xxx[i].dices, actorNr);
            huayangList.AddRange(SingleHuayang);
        }

        huayangList = huayangList.OrderByDescending(x => (byte)x.mingtang).ThenByDescending(x => x.num).ThenByDescending(x => x.num2).ToList();
        //再根据剩余的色子排序一次
        //huayangList.Sort(new ItemInfoCompare());
        byte index = 0;

        WinPlayers = new List<byte>();
        //一样的花样

        huayangList.RemoveAll(x => x.mingtang != huayangList[0].mingtang || x.num != huayangList[0].num || x.num2 != huayangList[0].num2);
        if (huayangList.Count >= 2)
        {
            if (huayangList[0].ResidueNum.Length > 0)
            {
                WinPlayers = CompareResidureDice(huayangList);
            }
            //那都赢了
            else
            {
                for (int i = 0; i < huayangList.Count; i++)
                {
                    WinPlayers.Add(huayangList[i].actorNr);
                }
            }
        }
        else
        {
            WinPlayers.Add(huayangList[0].actorNr);
        }

        return WinPlayers;

    }

    public void CheckHuayang(byte[] dices, byte actorNr)
    {
        List<byte> DicesList = new List<byte>(dices);
        SingleHuayang = new List<huayang>();
        DicesList = DicesList.OrderByDescending(x => x).ToList();
        byte SameCount = 0;
        byte MaxDice = 0;
        int i = 0;
        //for (int i = DicesList.Count - 1; i >= 0; i--)
        while (DicesList.Count > i)
        {
            int count = DicesList.FindAll(x => x == DicesList[i]).Count;

            if (DicesList[i] > MaxDice)
            {
                MaxDice = DicesList[i];

            }
            SameCount = (byte)count;

            if (!SingleHuayang.Exists(x => x.num == DicesList[i]))
            {
                Mingtang xx = 0;
                switch (SameCount)
                {
                    case 1:
                        xx = Mingtang.nul;
                        break;
                    case 2:
                        {
                            xx = Mingtang.duizi;
                            byte num = DicesList[i];
                            DicesList.RemoveAll(x => x == num);
                            i = -1;
                            byte[] residueNum = DicesList.ToArray();
                            huayang h = new huayang() { mingtang = xx, num = num, ResidueNum = residueNum, actorNr = actorNr };
                            SingleHuayang.Add(h);

                        }
                        break;
                    case 3:
                        {
                            xx = Mingtang.santiao;
                            byte num = DicesList[i];
                            DicesList.RemoveAll(x => x == num);
                            i = -1;
                            byte[] residueNum = DicesList.ToArray();
                            huayang h = new huayang() { mingtang = xx, num = num, ResidueNum = residueNum, actorNr = actorNr };

                            //前面3个后面一对
                            if (residueNum.Length > 1)
                                if (residueNum[0] == residueNum[1])
                                {
                                    h = new huayang() { mingtang = Mingtang.hulu, num = num, ResidueNum = null, actorNr = actorNr };
                                    DicesList.RemoveAll(x => x == residueNum[0]);
                                    i = -1;
                                }
                            //后面3个，前面一对
                            if (SingleHuayang.Exists(x => x.mingtang == Mingtang.duizi))
                            {
                                SingleHuayang.RemoveAll(x => x.mingtang == Mingtang.duizi);
                                DicesList.Clear();
                                i = -1;
                                h = new huayang() { mingtang = Mingtang.hulu, num = num, ResidueNum = null, actorNr = actorNr };
                            }

                            SingleHuayang.Add(h);
                        }
                        break;
                    case 4:
                        {
                            xx = Mingtang.jinggang;
                            byte num = dices[i];
                            DicesList.RemoveAll(x => x == dices[i]);
                            i = -1;
                            huayang h = new huayang() { mingtang = xx, num = num, ResidueNum = new byte[1] { DicesList[0] }, actorNr = actorNr };
                            SingleHuayang.Add(h);
                        }
                        break;
                    case 5:
                        {
                            xx = Mingtang.baozi;
                            huayang h = new huayang() { mingtang = xx, num = DicesList[i], actorNr = actorNr };
                            SingleHuayang.Add(h);
                        }
                        break;
                }
            }
            i++;
        }
        //顺子
        if (SingleHuayang.Count == 0)
        {
            int cout = 0;


            while (Mathf.Abs(DicesList[cout] - DicesList[cout + 1]) == 1)
            {
                cout++;
                if (cout == 4)
                    break;
            }

            if (cout == 4)
            {
                huayang h = new huayang() { mingtang = Mingtang.shunzi, num = DicesList[0], actorNr = actorNr };
                SingleHuayang.Add(h);
            }
        }
        //两对
        if (SingleHuayang.FindAll(x => x.mingtang == Mingtang.duizi).Count == 2)
        {
            SingleHuayang = SingleHuayang.OrderByDescending(x => x.num).ToList();
            byte num1 = SingleHuayang[0].num;
            byte num2 = SingleHuayang[1].num;
            i = -1;
            huayang h = new huayang() { mingtang = Mingtang.liangdui, num = num1, num2 = num2, ResidueNum = new byte[1] { DicesList[0] }, actorNr = actorNr };
            SingleHuayang.Clear();
            SingleHuayang.Add(h);
        }
        if (SingleHuayang.Count == 0)
        {
            huayang h = new huayang() { mingtang = Mingtang.nul, num = MaxDice, actorNr = actorNr };
            SingleHuayang.Add(h);
        }
        //huayangList = huayangList.OrderByDescending(x => x.mingtang).ThenByDescending(x => x.num).ToList();
    }
    public List<byte> CompareResidureDice(List<huayang> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            for (int k = 0; k < list[0].ResidueNum.Length; k++)
            {
                if (list[0].ResidueNum[k] > list[i].ResidueNum[k])
                {
                    list.Remove(list[i]);
                    break;
                }
                else if (list[0].ResidueNum[k] < list[i].ResidueNum[k])
                {
                    list[0] = list[i];
                    list.Remove(list[i]);
                    break;
                }
            }
        }
        return list.Select(x => x.actorNr).ToList();
    }

    [System.Serializable]
    public struct huayang
    {
        public Mingtang mingtang;
        //花样的那个色子
        //对3
        public byte num;
        //如果有两对就要用到这个参数
        public byte num2;
        //剩下的色子
        public byte[] ResidueNum;
        public byte actorNr;
    }

    public enum Mingtang
    {
        nul = 0,
        duizi = 1,
        liangdui = 2,
        santiao = 3,
        hulu = 4,
        shunzi = 5,
        jinggang = 6,
        baozi = 7,
    }
    [System.Serializable]
    public class aiyaya
    {
        public byte[] dices;
    }
    
}