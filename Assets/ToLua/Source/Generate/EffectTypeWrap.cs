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
		L.RegConstant("continuous", 2);
		L.RegConstant("trigger", 3);
		L.RegConstant("cantrigger", 4);
		L.RegConstant("musttrigger", 5);
		L.RegConstant("triggerinstant", 6);
		L.RegConstant("cantriggerinstant", 7);
		L.RegConstant("musttriggerinstant", 8);
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

