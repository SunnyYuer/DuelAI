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

    public void Run(string code)
    {
        try
        {
            luaState.DoFile("test.lua");
            //luaState.DoString(code);
            LuaFunction func = luaState.GetFunction("Card71703785");
            func.Call();
            func.Dispose();
        }
        catch(Exception e)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            File.WriteAllText("error.log", e.ToString());
#elif UNITY_ANDROID
            File.WriteAllText(Main.rulePath+"/error.log", e.ToString());
#endif
        }
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
        Run(code);
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
