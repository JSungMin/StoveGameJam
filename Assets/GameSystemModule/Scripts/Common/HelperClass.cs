using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class EnumHelper
{
    public static int CountOfElement<T>()
    {
        return (typeof(T).IsEnum) ? Enum.GetNames(typeof(T)).Length : -1;
    }
    public static string GetValue<T>(int e)
    {
        if (!typeof(T).IsEnum)
            return "";
        var arr = Enum.GetValues(typeof(T));
        var val = arr.GetValue(e);
        return val.ToString();
    }
}
public class ColorHelper
{
    public static Color GetColorFromHex(string r,string g, string b)
    {
        int ir = System.Convert.ToInt32(r, 16);
        int ig = System.Convert.ToInt32(g, 16);
        int ib = System.Convert.ToInt32(b, 16);
        return new Color((float)ir/255f, (float)ig/255f, (float)ib/255f);
    }
    public static Color GetColorFromHex(string rgb)
    {
        string r = rgb.Substring(0,2);
        string g = rgb.Substring(2,2);
        string b = rgb.Substring(4,2);
        return GetColorFromHex(r,g,b);
    }
}
public static class TypeHelper
{
    public static IEnumerable<Type> GetBaseTypes(this Type type)
    {
        return type.BaseType == typeof(object)
            ? type.GetInterfaces()
            : Enumerable
                .Repeat(type.BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.BaseType.GetBaseTypes())
                .Distinct();
    }
    public static bool HaveBaseType(Type cur, Type super)
    {
        return GetBaseTypes(cur)
            .Count(x => x == super) > 0 || cur == super;
    }
    public static T CreateInstance<T>()
    {
        var t = typeof(T);
        if (t.GetConstructor(Type.EmptyTypes) == null)
        {
            return default(T);
        }
        var instance = (T)Activator.CreateInstance(t);
        return instance;
    }
    public static object CreateInstance(Type t)
    {
        var instance = Activator.CreateInstance(t);
        if (t.GetConstructor(Type.EmptyTypes) == null)
        {
            return null;
        }
        return instance;
    }
    public static object CreateInstance(Type t, params object[] args)
    {
        var instance = Activator.CreateInstance(t, args);
        return instance;
    }
    public static Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains("."))
        {
            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;

        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {

            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;
    }
}
public class PathHelper
{
    public static string Bind(string path01, string path02, bool isRSlash = false)
    {
        if (isRSlash)
            return path01 + "\\" + path02;
        return path01 + "/" + path02;
    }
    public static string GetParentPath(string path)
    {
        if (path.Length == 0)
            return "";
        return path.Substring(0, path.LastIndexOf('/'));
    }
    public static string AbsoluteToPersistentpath(string absPath)
    {
        var indx = absPath.IndexOf("Assets");
        if (indx < 0)
            return "";
        return absPath.Substring(indx);
    }
#if UNITY_EDITOR
    public static T[] FindAssets<T>(params string[] searchFolders) where T : UnityEngine.Object
    {
        var t = typeof(T).Name;
        var assetGUIDS = UnityEditor.AssetDatabase.FindAssets("t:" + t);
        string[] pathes = new string[assetGUIDS.Length];
        T[] results = new T[pathes.Length];
        for (int i = 0; i < assetGUIDS.Length; i++)
        {
            pathes[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGUIDS[i]);
            results[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(pathes[i]);
        }
        return results;
    }
    public static UnityEngine.Object[] FindAssets(Type t, params string[] searchFolders)
    {
        var assetGUIDS = UnityEditor.AssetDatabase.FindAssets("t:" + t);
        string[] pathes = new string[assetGUIDS.Length];
        UnityEngine.Object[] results = new UnityEngine.Object[pathes.Length];
        for (int i = 0; i < assetGUIDS.Length; i++)
        {
            pathes[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGUIDS[i]);
            results[i] = UnityEditor.AssetDatabase.LoadAssetAtPath(pathes[i], t);
        }
        return results;
    }
    //  If already have folder on same path then happen nothing
    public static void MakeSurePath (string path)
    {
        if (UnityEditor.AssetDatabase.IsValidFolder(path))
            return;
        var data = path.Split('/');
        var tmpPath = data[0];
        for (int i = 1; i < data.Length; i++)
        {
            if (data[i].Contains("."))
            {
                continue;
            }
            if (!UnityEditor.AssetDatabase.IsValidFolder(tmpPath + "/" + data[i]))
            {
                UnityEditor.AssetDatabase.CreateFolder(tmpPath, data[i]);
                UnityEditor.AssetDatabase.Refresh();
            }
            tmpPath = Bind(tmpPath, data[i]);
        }
    }
#endif
}