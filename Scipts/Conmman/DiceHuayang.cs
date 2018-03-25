using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceHuayang : MonoBehaviour
{
    public byte[] dices;
    public static List<huayang> SingleHuayang;
    public void Start()
    {
        CheckHuayang(dices);
    }
    //public static byte CheckHuayang(byte[] dices)
    //{
    //List<byte> DicesList = new List<byte>(dices);
    //List<huayang> SingleHuayang = new List<huayang>();
    //DicesList.Sort();
    //byte SameCount = 0;
    //byte MaxDice = 0;

    //for (int i = 0; i < dices.Length; i++)
    //{
    //    int count = DicesList.FindAll(x => x == dices[i]).Count;

    //    if (dices[i] > MaxDice)
    //    {
    //        MaxDice = dices[i];

    //    }

    //    SameCount = (byte)count;

    //    if (!SingleHuayang.Exists(x => x.num == dices[i]))
    //    {
    //        byte xx = 0;
    //        switch (SameCount)
    //        {
    //            case 1:
    //                xx = Mingtang.nul;
    //                break;
    //            case 2:
    //                xx = Mingtang.duizi;
    //                break;
    //            case 3:
    //                xx = Mingtang.santiao;
    //                break;
    //            case 4:
    //                xx = Mingtang.jinggang;
    //                break;
    //            case 5:
    //                xx = Mingtang.baozi;
    //                break;
    //        }

    //        if (xx != Mingtang.nul)
    //        {
    //            huayang h = new huayang() { mingtang = xx, num = dices[i], actorNr = 0 };
    //            SingleHuayang.Add(h);
    //        }
    //    }
    //}

    //if (SingleHuayang.Count == 0)
    //{
    //    int cout = 0;

    //    while (Mathf.Abs(DicesList[cout] - DicesList[cout + 1]) == 1)
    //    {
    //        cout++;
    //        if (cout == 4)
    //            break;
    //    }

    //    if (cout == 4)
    //    {
    //        huayang h = new huayang() { mingtang = Mingtang.shunzi, num = 1, actorNr = 0 };
    //        SingleHuayang.Add(h);
    //    }
    //}
    //if (SingleHuayang.Count > 0)
    //{
    //    SingleHuayang.OrderBy(x => x.mingtang);
    //    print(SingleHuayang[0].mingtang);
    //    return SingleHuayang[0].mingtang;
    //}
    //return 0;
    //}

    public static byte CheckHuayang(byte[] dices)
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
                            huayang h = new huayang() { mingtang = xx, num = num, ResidueNum = residueNum };
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
                            huayang h = new huayang() { mingtang = xx, num = num, ResidueNum = residueNum };

                            //前面3个后面一对
                            if (residueNum.Length > 1)
                                if (residueNum[0] == residueNum[1])
                                {
                                    h = new huayang() { mingtang = Mingtang.hulu, num = num, ResidueNum = null };
                                    DicesList.RemoveAll(x => x == residueNum[0]);
                                    i = -1;
                                }
                            //后面3个，前面一对
                            if (SingleHuayang.Exists(x => x.mingtang == Mingtang.duizi))
                            {
                                SingleHuayang.RemoveAll(x => x.mingtang == Mingtang.duizi);
                                DicesList.Clear();
                                i = -1;
                                h = new huayang() { mingtang = Mingtang.hulu, num = num, ResidueNum = null };
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
                            huayang h = new huayang() { mingtang = xx, num = num, ResidueNum = new byte[1] { DicesList[0] } };
                            SingleHuayang.Add(h);
                        }
                        break;
                    case 5:
                        {
                            xx = Mingtang.baozi;
                            huayang h = new huayang() { mingtang = xx, num = DicesList[i] };
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
                huayang h = new huayang() { mingtang = Mingtang.shunzi, num = DicesList[0] };
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
            huayang h = new huayang() { mingtang = Mingtang.liangdui, num = num1, num2 = num2, ResidueNum = new byte[1] { DicesList[0] } };
            SingleHuayang.Clear();
            SingleHuayang.Add(h);
        }
        if (SingleHuayang.Count == 0)
        {
            huayang h = new huayang() { mingtang = Mingtang.nul, num = MaxDice };
            SingleHuayang.Add(h);
        }
        //print(SingleHuayang[0].mingtang);
        return (byte)SingleHuayang[0].mingtang;
    }
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
