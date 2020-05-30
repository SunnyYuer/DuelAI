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

    public string CostFunStr(DuelCard duelcard, int effect)
    {
        return "c" + duelcard.id + "cost" + effect;
    }

    public string EffectFunStr(DuelCard duelcard, int effect)
    {
        return "c" + duelcard.id + "effect" + effect;
    }

    public void Close()
    {
        luaState.Dispose();
    }

    public void Test()
    {
        string code = 
            @"
            function Card71703785()
                print('运行成功')
            end
            ";
        SetCode(code);
        Run("Card71703785");
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
