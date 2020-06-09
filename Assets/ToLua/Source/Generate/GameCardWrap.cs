﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameCardWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GameCard), typeof(System.Object));
		L.RegFunction("New", _CreateGameCard);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegConstant("name", 30);
		L.RegConstant("type", 31);
		L.RegConstant("series", 32);
		L.RegConstant("attribute", 33);
		L.RegConstant("level", 34);
		L.RegConstant("race", 35);
		L.RegConstant("atk", 36);
		L.RegConstant("def", 37);
		L.RegConstant("mean", 38);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGameCard(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				GameCard obj = new GameCard();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: GameCard.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

