﻿using Core.Utilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        IMemoryCache _memoryCache;

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
        }
        //Adapter pattern aslında bu sınıf microsofttan otomatik kulanılabilinir
        //ama  yarın yeni bir service gecilmek isteniğinde bütün kodları değişmesi gerekir
        //biz bu yuzden referans tutatak bu classı yazıyıoruz
        //değiştirmek istediğimizde referansını değiştirerek ototmatik impl yapmıs olcaz

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);

        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public bool IsActive(string key)
        {
            return _memoryCache.TryGetValue(key,out _);//bir şey dönmesin diye _
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
        public void RemoveByPattern(string pattern)
        {

            var cacheEntriesFieldCollectionDefinition = typeof(MemoryCache).GetField("_coherentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var coherentStateValueCollection = cacheEntriesFieldCollectionDefinition.GetValue(_memoryCache);
            var entriesCollectionValueCollection = coherentStateValueCollection.GetType().GetProperty("EntriesCollection",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dynamic cacheEntriesCollection = entriesCollectionValueCollection.GetValue(coherentStateValueCollection);


            List<ICacheEntry> cacheCollectionValues = new List<ICacheEntry>();
            foreach (var cacheItem in cacheEntriesCollection)
            {
                var cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }


            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList();

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }


        }
    }
}

