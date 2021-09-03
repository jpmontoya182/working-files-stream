using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workfiles.console
{
    public class ProcessedOrderMap : ClassMap<ProcessedOrder>
    {
        public ProcessedOrderMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            // Map(po => po.OrderNumber).Name("OrderNumber");
            Map(po => po.Customer).Name("CustomerNumber");
            Map(po => po.Amount)
                .Name("Quantity")
                .TypeConverter<RomanTypeConverter>();
        }
    }
}
