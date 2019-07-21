using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CodeCompiler
{
    public void TestCSharpCode()
    {
        string code =
                "using System;" +
                "public class CardScript" +
                "{" +
                "   public string Card71703785()" +
                "   {" +
                "       return TimeTool.GetDateTime(0).ToString();" +
                "   }" +
                "}";
        CSharpCompile(code);
    }

    /// <summary>
    /// C#动态编译
    /// 需要Api兼容级别 .NET 4.x
    /// 打包后需要安装mono才能运行
    /// </summary>
    /// <param name="code"></param>
    public void CSharpCompile(string code)
    {
        /*
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        CompilerParameters parameters = new CompilerParameters();

        //添加当前程序集
        //Assembly.GetExecutingAssembly().Location
        //typeof(CodeCompiler).Assembly.Location    推荐
        parameters.ReferencedAssemblies.Add(typeof(CodeCompiler).Assembly.Location);
        parameters.GenerateExecutable = false;
        parameters.GenerateInMemory = true;
        CompilerResults results = null;
        try
        {
            results = codeProvider.CompileAssemblyFromSource(parameters, code);
        }
        catch (Exception e)
        {
            File.WriteAllText("log.txt", e.ToString());
        }
        if (results.Errors.HasErrors)
        {
            foreach (CompilerError err in results.Errors)
            {
                Debug.Log(err.ToString());
                File.WriteAllText("log.txt", err.ToString());
            }
        }
        else
        {// 通过反射，调用实例
            Assembly assembly = results.CompiledAssembly;
            object objHelloWorld = assembly.CreateInstance("CardScript");
            MethodInfo methodInfo = objHelloWorld.GetType().GetMethod("Card71703785");
            Debug.Log(methodInfo.Invoke(objHelloWorld, null));
        }
        */
    }
}
