using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MentoratNetCore.Extensions
{
    public class KendoDataSourceResult : Kendo.Mvc.UI.DataSourceResult
    {

        public Kendo.Mvc.UI.DataSourceResult Parent { get; set; }

        public KendoDataSourceResult() { }
        public KendoDataSourceResult(Kendo.Mvc.UI.DataSourceResult parent)
        {
            Parent = parent;

            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }


        public string MessageDataSource { get; set; }
    }
}