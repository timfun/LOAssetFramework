// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOAssetFramework
{
	internal sealed class LOAssetCache
	{
		#region 包裹缓存机制
		//创建缓存字典
		private static Dictionary<string, LOAssetBundle> assetBundleCache;
		//缓存字典属性
		private static Dictionary<string, LOAssetBundle> BundleCache
		{
			get{ 
				if (assetBundleCache == null) {
					assetBundleCache = new Dictionary<string, LOAssetBundle> ();
				}
				
				return assetBundleCache;
			}
		}
		
		//创建缓存WWW对象
		private static Dictionary<string, WWW> wwwCache;
		//创建缓存WWW对象属性
		private static Dictionary<string, WWW> WwwCache{
			get{ 
				if (wwwCache == null) {
					wwwCache = new Dictionary<string, WWW> ();
				}
				
				return wwwCache;
			}
		}
		//创建依赖缓存对象
		private static Dictionary<string, string[]> dependCache;
		//创建依赖缓存属性
		private static Dictionary<string, string[]> DependCache
		{
			get{ 
				if (dependCache == null) {
					dependCache = new Dictionary<string, string[]> ();
				}
				return dependCache;
			}
		}
		
		private static Dictionary<string, string> errorCache;
		private static Dictionary<string,string> ErrorCache{
			get{ 
				if (errorCache == null) {
					errorCache = new Dictionary<string, string> ();
				}
				return errorCache;
			}
		}

		/// <summary>
		/// Ins the cache.
		/// </summary>
		/// <returns><c>true</c>, if cache was ined, <c>false</c> otherwise.</returns>
		/// <param name="assetbundlename">Assetbundlename.</param>
		internal static bool InCache(string assetbundlename)
		{
			return LOAssetCache.BundleCache.ContainsKey(assetbundlename);
		}

		internal static bool InWWWCache(string assetbundlename)
		{
			return LOAssetCache.WwwCache.ContainsKey(assetbundlename);
		}

		#endregion


		#region 卸载系列函数
		
		/// <summary>
		/// 卸载资源包和依赖包
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public static void UnloadAssetBundle(string assetBundleName)
		{
			UnloadAssetBundleInternal (assetBundleName);
			UnloadDependencies (assetBundleName);
		}
		internal static void UnloadDependencies(string assetBundleName)
		{
			string[] dependencies = null;
			//获取所有的依赖包名称
			if (!LOAssetCache.DependCache.TryGetValue(assetBundleName, out dependencies) )
				return;
			
			//卸载依赖包
			foreach(var dependency in dependencies)
			{
				UnloadAssetBundleInternal(dependency);
			}
			//删除依赖缓存策略
			LOAssetCache.DependCache.Remove(assetBundleName);
		}
		
		internal static void UnloadAssetBundleInternal(string assetBundleName)
		{
			LOAssetBundle bundle;
			LOAssetCache.BundleCache.TryGetValue(assetBundleName,out bundle);

			if (bundle == null)
			{
				return;
			}
			bundle.Release ();
		}
		#endregion

		#region Setter`Getter
		internal static WWW GetWWWCache(string key)
		{
			WWW www;
			
			LOAssetCache.WwwCache.TryGetValue(key,out www);
			
			return www;
		}
		internal static void SetWWWCache(string key,WWW value)
		{
			LOAssetCache.WwwCache.Add(key,value);
		}
		
		internal static LOAssetBundle GetBundleCache(string key)
		{
			LOAssetBundle ab;
			
			LOAssetCache.BundleCache.TryGetValue(key,out ab);
			
			return ab;
		}
		internal static void SetBundleCache(string key,LOAssetBundle value)
		{
			LOAssetCache.BundleCache.Add(key,value);
		}

		internal static string[] GetDependCache(string key)
		{
			string[] depends;
			
			LOAssetCache.DependCache.TryGetValue(key,out depends);
			
			return depends;
		}
		internal static void SetDependCache(string key,string[] value)
		{
			LOAssetCache.DependCache.Add(key,value);
		}

		internal static string GetErrorCache(string key)
		{
			string error;
			
			LOAssetCache.ErrorCache.TryGetValue(key,out error);
			
			return error;
		}
		internal static void SetErrorCache(string key,string value)
		{
			LOAssetCache.ErrorCache.Add(key,value);
		}
		#endregion

		internal static void FreeBundle(string key)
		{
			LOAssetCache.BundleCache.Remove(key);
		}

		#region Update

		internal static void Update()
		{
			// Collect all the finished WWWs.
			var keysToRemove = new List<string>();
			foreach (var keyValue in LOAssetCache.WwwCache)
			{
				WWW download = keyValue.Value;

				string m_bundleName = keyValue.Key;
				
				// 下载失败
				if (download.error != null)
				{
					LOAssetCache.ErrorCache.Add(m_bundleName, download.error);
					
					keysToRemove.Add(m_bundleName);
					
					continue;
				}
				
				// 下载成功
				if(download.isDone)
				{
					
					LOAssetCache.BundleCache.Add(m_bundleName, new LOAssetBundle(download.assetBundle,m_bundleName));
					
					keysToRemove.Add(m_bundleName);
				}
			}
			
			// 删除下载成功的WWW对象
			foreach( var key in keysToRemove)
			{
				WWW download = LOAssetCache.WwwCache[key];
				
				LOAssetCache.WwwCache.Remove(key);
				
				download.Dispose();
			}
		}

		#endregion
	}
}

