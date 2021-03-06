﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class EffectTypeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(EffectType), typeof(System.Object));
		L.RegFunction("New", _CreateEffectType);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegConstant("startup", 1);
		L.RegConstant("cantrigger", 2);
		L.RegConstant("musttrigger", 3);
		L.RegConstant("cantriggerinstant", 4);
		L.RegConstant("musttriggerinstant", 5);
		L.RegConstant("activate", 6);
		L.RegConstant("continuous", 7);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateEffectType(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				EffectType obj = new EffectType();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: EffectType.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

