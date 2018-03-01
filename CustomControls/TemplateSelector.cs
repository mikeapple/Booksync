using System;
using Xamarin.Forms;

namespace BookSync.CustomControls
{
    public class TableDataTemplateSelector : IDataTemplateSelector
    {
        public DataTemplate MainTemplate { get; set; }
        public DataTemplate SubTemplate { get; set; }

        public DataTemplate SelectTemplate(object view, object dataItem)
        {
            if (!string.IsNullOrEmpty(dataItem.ToString()))
            {
                return MainTemplate;
            }
            else
            {
                return SubTemplate;
            }
        }
    }
}
