using EFCache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities
{
    class CacheConfiguration : DbConfiguration
    {
        public CacheConfiguration()
        {
            var transactionHandler = new CacheTransactionHandler(new InMemoryCache());

            AddInterceptor(transactionHandler);

            //var cachingPolicy = new CachingPolicy();
            var cachingPolicy = new myCachingPolicy();

            Loaded +=
                (sender, e) => e.ReplaceService<DbProviderServices>(
                    (s, _) => new CachingProviderServices(s, transactionHandler, cachingPolicy));
        }
    }

    public class myCachingPolicy : CachingPolicy
    {
        protected override bool CanBeCached(System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Entity.Core.Metadata.Edm.EntitySetBase> affectedEntitySets, string sql, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            string[] excludedEntities = {
            "adm_company_module",
            "adm_company_screen",
            "adm_module",
             "adm_screen",
             "adm_dropdown_value",
             "adm_product_mf",
             "adm_product_alternate",
             "adm_product_uom",
             "adm_product_warehouse",
             "adm_party"
            };

            if (affectedEntitySets.Where(x => excludedEntities.Contains(x.Table)).Any())
            {
                return false;
            }
            else
            {
                return base.CanBeCached(affectedEntitySets, sql, parameters);
            }
        }
    }
}
