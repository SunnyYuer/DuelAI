using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LuaCode
{
    private static LuaState luaState;

    public LuaCode()
    {
        InitLoader();
        luaState = new LuaState();
        LuaBinder.Bind(luaState);
        luaState.Start();
    }

    protected LuaFileUtils InitLoader()
    {
        return new LuaResLoader();
    }

    public void SetCode(string code)
    {
        luaState.DoFile("test.lua");
        //luaState.DoString(code);
    }

    public void Run(string function)
    {
        try
        {
            LuaFunction func = luaState.GetFunction(function);
            func.Call();
            func.Dispose();
        }
        catch(Exception e)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            File.AppendAllText("error.log", e.ToString());
#elif UNITY_ANDROID
            File.WriteAllText(Main.rulePath+"/error.log", e.ToString());
#endif
        }
    }

    public string CostFunStr(ChainableEffect chainableEffect)
    {
        return "c" + chainableEffect.card + "cost" + chainableEffect.effect;
    }

    public string EffectFunStr(ChainableEffect chainableEffect)
    {
        return "c" + chainableEffect.card + "effect" + chainableEffect.effect;
    }

    public void Close()
    {
        luaState.Dispose();
    }

    public void Test()
    {
        //每行必须加\n
        string code =
            "function Card71703785()\n" +
            "print(\"运行成功\")\n" +
            "end\n";
        SetCode(code);
        Run("Card71703785");
        Close();
    }

    public static void TestLog(object msg)
    {
        Debug.Log(msg.ToString());
#if UNITY_EDITOR || UNITY_STANDALONE
        File.WriteAllText("1.log", "运行成功");
#elif UNITY_ANDROID
        File.WriteAllText(Main.rulePath + "/1.log", "运行成功");
#endif
    }
}
