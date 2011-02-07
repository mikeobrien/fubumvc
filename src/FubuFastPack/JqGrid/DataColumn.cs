﻿using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class DataColumn<T> : IGridColumn where T : DomainEntity
    {
        private readonly Accessor _accessor;

        public DataColumn()
        {
            _accessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
        }

        // TODO -- UT this
        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>{
                {"name", "data"},
                {"index", "data"},
                {"sortable", false},
                {"hidden", true}
            };
        }

        public Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls)
        {
            var getter = data.GetterFor(_accessor);
            return dto => dto["Id"] = getter().ToString();
        }

        public IEnumerable<FilterDTO> PossibleFilters(IQueryService queryService)
        {
            yield break;
        }

        public IEnumerable<Accessor> SelectAccessors()
        {
            yield return _accessor;
        }

        public IEnumerable<Accessor> AllAccessors()
        {
            return SelectAccessors();
        }

        public string GetHeader()
        {
            return "Data";
        }
    }
}